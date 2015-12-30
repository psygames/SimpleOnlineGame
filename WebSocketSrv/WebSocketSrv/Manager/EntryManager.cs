using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShitMan
{
    class EntryManager
    {
        private static EntryManager s_instance;
        public static EntryManager Instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new EntryManager();
                }
                return s_instance;
            }
        }

        private Dictionary<long, Food> m_allFoods;
        private Dictionary<long, Player> m_allPlayers;
        public float deltaTime;
        public float fixedDeltaTime;

        private List<Food> GetFoodList()
        {
            List<Food> list = new List<Food>();
            foreach (var itr in m_allFoods)
            {
                list.Add(itr.Value);
            }
            return list;
        }

        private List<Player> GetPlayerList()
        {
            List<Player> list = new List<Player>();
            foreach (var itr in m_allPlayers)
            {
                list.Add(itr.Value);
            }
            return list;
        }


        public EntryManager()
        {
            m_allFoods = new Dictionary<long, Food>();
            m_allPlayers = new Dictionary<long, Player>();
        }

        public void PlayerJoin(long guid, string name)
        {
            if (m_allPlayers.ContainsKey(guid))
            {
                HallManager.Instance.Print("Already Exist Player : " + guid);
            }
            else
            {
                Player player = new Player();
                player.level = 1;
                player.guid = guid;
                player.name = name;
                player.radius = 0.5f;
                player.pos = Vector3.zero;
                player.dir = Vector3.zero;
                m_allPlayers.Add(guid, player);
            }

            List<Player> playerList = GetPlayerList();

            for (int i = playerList.Count - 1; i >= 0; i--)
            {
                Player player = playerList[i];
                if (player.guid != guid)
                {
                    SC_PlayerJoinOther snd = new SC_PlayerJoinOther();
                    snd.guid = player.guid;
                    snd.name = player.name;
                    SessionManager.Instance.SendTo(guid, snd);
                }
            }
            ActiveFoodPosSync();
        }

        public void PlayerExit(long guid)
        {
            if (!m_allPlayers.ContainsKey(guid))
            {
                HallManager.Instance.Print("Not Exist Player : " + guid);
            }
            else
            {
                SC_PlayerExitOther msg = new SC_PlayerExitOther();
                msg.guid = guid;
                SessionManager.Instance.SendToOthers(guid, msg);
                m_allPlayers.Remove(guid);
            }
        }

        public void MoveSync(long guid, Vector3 pos, Vector3 dir)
        {
            m_allPlayers[guid].pos = pos;
            m_allPlayers[guid].dir = dir;
        }

        public void Update()
        {
            UpdatePlayerMoveSyncOther();
            UpdateGenerateFood();
        }

        // 物理碰撞检测
        public void FixedUpdate()
        {
            CheckEatFood();
            CheckEatPlayer();
            CheckFoodAliveTime();
        }

        /*----------------- Physics Update ---------------*/
        void CheckFoodAliveTime()
        {
            bool isChanged = false;
            List<Food> foodList = GetFoodList();
            for (int i = foodList.Count - 1; i >= 0; i--)
            {
                Food food = foodList[i];
                food.aliveTime += fixedDeltaTime;
                if (food.aliveTime > GameConfig.maxFoodAliveTime)
                {
                    isChanged = true;
                    m_allFoods[food.guid].radius += 0.1f;
                    m_allFoods[food.guid].aliveTime = 0;
                }
            }
            if (isChanged)
                ActiveFoodPosSync();
        }

        void CheckEatFood()
        {
            List<Food> foodList = GetFoodList();
            List<Player> playerList = GetPlayerList();

            for (int i = playerList.Count - 1; i >= 0; i--)
            {
                Player player = playerList[i];
                for (int j = foodList.Count - 1; j >= 0; j--)
                {
                    Food food = foodList[j];
                    // player eat food 
                    if (player.IsCollisionWith(food))
                    {
                        OnPlayerEatFood(player.guid, food.guid);
                    }
                }
            }
        }


        void OnPlayerEatFood(long playerGuid, long foodGuid)
        {
            if (!m_allFoods.ContainsKey(foodGuid))
                return;
            m_allPlayers[playerGuid].IncreaseRaius(m_allFoods[foodGuid].radius);
            lock (m_allFoods)
            {
                m_allFoods.Remove(foodGuid);
            }
            SC_PlayerEatFood msg = new SC_PlayerEatFood();
            msg.foodGuid = foodGuid;
            msg.playerGuid = playerGuid;
            SessionManager.Instance.SendToAll(msg);
        }

        void CheckEatPlayer()
        {
            List<Player> playerList = GetPlayerList();
            for (int i = playerList.Count - 1; i >= 0; i--)
            {
                Player player = playerList[i];
                for (int j = playerList.Count - 1; j >= 0; j--)
                {
                    Player player2 = playerList[j];
                    if (player.guid != player2.guid
                        && player.level - player2.level >= GameConfig.PlayerCanBeEatLevelDist
                        && player.IsCollisionWith(player2))
                    {
                        OnPlayerEatPlayer(player.guid, player2.guid);
                    }
                }
            }
        }

        void OnPlayerEatPlayer(long playerGuid, long player2Guid)
        {
            m_allPlayers[playerGuid].IncreaseRaius(m_allPlayers[player2Guid].radius);
            m_allPlayers.Remove(player2Guid);
            SC_PlayerEatPlayer msg = new SC_PlayerEatPlayer();
            msg.eatGuid = playerGuid;
            msg.eatedGuid = player2Guid;
            SessionManager.Instance.SendToAll(msg);
        }

        /*------------------ Logic Update --------------*/
        private float m_playerMoveSyncTime = 0;
        void UpdatePlayerMoveSyncOther()
        {
            m_playerMoveSyncTime += deltaTime;
            if (m_playerMoveSyncTime < GameConfig.PlayerMoveSyncInterval)
                return;
            m_playerMoveSyncTime -= GameConfig.PlayerMoveSyncInterval;

            List<Player> playerList = GetPlayerList();
            for (int i = playerList.Count - 1; i >= 0; i--)
            {
                Player player = playerList[i];
                SC_MoveSyncOther msg = new SC_MoveSyncOther();
                List<PlayerState> tmpList = new List<PlayerState>();
                for (int j = playerList.Count - 1; j >= 0; j--)
                {
                    Player otherPlayer = playerList[j];
                    if (otherPlayer.guid != player.guid)
                    {
                        FrameState fState = FrameSyncManager.Instance.GetPlayerState(otherPlayer.guid);
                        PlayerState pState = new PlayerState();
                        pState.guid = otherPlayer.guid;
                        pState.pos = fState.pos;
                        pState.dir = fState.dir;
                        pState.radius = otherPlayer.radius;
                        tmpList.Add(pState);
                    }
                }

                msg.playerStateList = tmpList.ToArray<PlayerState>();
                if (msg.playerStateList.Length > 0)
                    SessionManager.Instance.SendTo(player.guid, msg);
            }
        }

        private float m_foodGeneCD = 0;
        void UpdateGenerateFood()
        {
            m_foodGeneCD = Math.Max(m_foodGeneCD - deltaTime, 0);
            if (m_allFoods.Count < GameConfig.MaxFoodCount && m_foodGeneCD <= 0)
            {
                int gCount = Math.Min(GameConfig.MaxFoodCount - m_allFoods.Count, GameConfig.FoodGeneratePerDurationCount);
                for (int i = 0; i < gCount; i++)
                {
                    int randTabId = MathHelper.RandomInt(1, TableManager.instance.GetAlllPropertiesByType<TableFoodPos>().Count + 1);
                    TableFoodPos tabFood = TableManager.instance.GetPropertiesById<TableFoodPos>(randTabId);
                    Food food = new Food();
                    food.guid = MathHelper.LongGUID;
                    food.tableId = tabFood.id;
                    food.pos = tabFood.pos;
                    food.dir = tabFood.dir;
                    food.radius = 0.3f;
                    lock (m_allFoods)
                    {
                        m_allFoods.Add(food.guid, food);
                    }
                }
                m_foodGeneCD = GameConfig.FoodGenerateDuration;

                ActiveFoodPosSync();
            }
        }

        /*---------------------- active call ------------------*/
        void ActiveFoodPosSync()
        {
            SC_FoodSync msg = new SC_FoodSync();
            List<Food> foodList = GetFoodList();
            msg.foodStateList = new FoodState[foodList.Count];
            for (int i = foodList.Count - 1; i >= 0; i--)
            {
                FoodState foodState = new FoodState();
                foodState.guid = foodList[i].guid;
                foodState.radius = foodList[i].radius;
                foodState.tableId = foodList[i].tableId;
                msg.foodStateList[i] = foodState;
            }
            SessionManager.Instance.SendToAll(msg);
        }
    }
}
