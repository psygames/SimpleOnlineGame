using ShitMan;
using System.Collections.Generic;
using UnityEngine;
class EntryManager
{
    private static EntryManager m_instance;
    public static EntryManager instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new EntryManager();
            return m_instance;
        }
    }

    private Dictionary<long, FoodCtrl> m_allFoods = new Dictionary<long, FoodCtrl>();
    private Dictionary<long, NetPlayer> m_allNetPlayers = new Dictionary<long, NetPlayer>();
    private MainPlayerCtrl mainPlayer = null;

    public List<PlayerBaseCtrl> GetAllPlayerSorted()
    {
        List<PlayerBaseCtrl> ctrls = new List<PlayerBaseCtrl>();
        var itr = m_allNetPlayers.GetEnumerator();
        while (itr.MoveNext())
        {
            ctrls.Add(itr.Current.Value);
        }
        if (mainPlayer != null)
            ctrls.Add(mainPlayer);

        for (int i = 0; i < ctrls.Count; i++)
        {
            for (int j = i + 1; j < ctrls.Count; j++)
            {
                if (ctrls[i].radius < ctrls[j].radius)
                {
                    PlayerBaseCtrl tmp = ctrls[i];
                    ctrls[i] = ctrls[j];
                    ctrls[j] = tmp;
                }
            }
        }

        return ctrls;
    }

    public void SyncFoods(FoodState[] foodList)
    {
        for (int i = 0; i < foodList.Length; i++)
        {
            FoodState food = foodList[i];
            if (!m_allFoods.ContainsKey(food.guid))
            {
                TableFoodPos tFood = TableManager.instance.GetPropertiesById<TableFoodPos>(food.tableId);
                GameObject go = (GameObject)Object.Instantiate(Resources.Load(tFood.modelPath));
                FoodCtrl _ctrl = go.GetComponent<FoodCtrl>();
                if (_ctrl == null)
                    _ctrl = go.AddComponent<FoodCtrl>();
                _ctrl.name = "Food_" + food.guid;
                _ctrl.guid = food.guid;
                m_allFoods.Add(food.guid, _ctrl);
            }
            m_allFoods[food.guid].Sync(food);
        }
    }

    public void MainPlayerJoin(long guid, string name, ShitMan.Vector3 bornPos)
    {
        GameObject go = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GamePlay/Player"));
        MainPlayerCtrl _ctrl = go.GetComponent<MainPlayerCtrl>();
        _ctrl.name = "MainPlayer_" + name;
        _ctrl.transform.position = MathHelper.TransplantToVector3(bornPos);
        _ctrl.playerName = name;
        _ctrl.guid = guid;
        _ctrl.radius = 0.5f;
        mainPlayer = _ctrl;
    }

    public void PlayerEatFood(long playerGuid, long foodGuid)
    {
        if ((playerGuid != mainPlayer.guid && !m_allNetPlayers.ContainsKey(playerGuid)) || !m_allFoods.ContainsKey(foodGuid))
        {
            Debug.LogError("Eat Food Error : " + playerGuid + " ---> " + foodGuid);
            return;
        }

        if (playerGuid == mainPlayer.guid)
        {
            mainPlayer.OnEatFood(foodGuid, m_allFoods[foodGuid].radius);
            Debug.Log("You Eat Food : " + foodGuid);
        }
        else
        {
            Debug.Log(m_allNetPlayers[playerGuid].playerName + " Eat Food : " + foodGuid);
        }

        m_allFoods[foodGuid].OnDestroy();
        m_allFoods.Remove(foodGuid);
    }

    public void PlayerEatPlayer(long eatGuid, long eatedGuid)
    {
        if (eatGuid == eatedGuid
            || (eatGuid != mainPlayer.guid && !m_allNetPlayers.ContainsKey(eatGuid))
            || (eatedGuid != mainPlayer.guid && !m_allNetPlayers.ContainsKey(eatedGuid)))
        {
            Debug.LogError("Eat Player Error : " + eatGuid + " ---> " + eatedGuid);
            return;
        }
        if (mainPlayer.guid == eatGuid)
        {
            Debug.Log("You Eat Player : " + m_allNetPlayers[eatedGuid]);
            mainPlayer.OnEatPlayer(eatedGuid, m_allNetPlayers[eatedGuid].radius);
            m_allNetPlayers[eatedGuid].OnDestroy();
            m_allNetPlayers.Remove(eatedGuid);
        }
        else if (mainPlayer.guid == eatedGuid)
        {
            Debug.Log("You Eatet By Player : " + m_allNetPlayers[eatGuid]);
            mainPlayer.OnDestroy();
            LoginCtrl.instance.Show();
        }
        else
        {
            Debug.Log(m_allNetPlayers[eatGuid].playerName + " Eat Player : " + m_allNetPlayers[eatedGuid].playerName);
            m_allNetPlayers[eatedGuid].OnDestroy();
            m_allNetPlayers.Remove(eatedGuid);
        }
    }

    public void PlayerJoinOther(long guid, string name)
    {
        if (m_allNetPlayers.ContainsKey(guid))
        {
            m_allNetPlayers[guid].playerName = name;
            m_allNetPlayers[guid].name = "NetPlayer_" + name;
            Debug.Log("Already Exist NetPlayer: " + guid + " ChangeName(" + m_allNetPlayers[guid].playerName + "," + name + ")");
            return;
        }
        GameObject go = (GameObject)Object.Instantiate(Resources.Load("Prefabs/GamePlay/NetPlayer"));
        NetPlayer _ctrl = go.GetComponent<NetPlayer>();
        _ctrl.name = "NetPlayer_" + name;
        _ctrl.playerName = name;
        _ctrl.guid = guid;
        _ctrl.radius = 0.5f;
        m_allNetPlayers.Add(guid, _ctrl);
    }

    public void PlayerSync(PlayerState[] players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            PlayerState player = players[i];
            if (m_allNetPlayers.ContainsKey(player.guid))
            {
                m_allNetPlayers[player.guid].Sync(player);
            }
        }
    }

    public void PlayerExitOther(long guid)
    {
        if (!m_allNetPlayers.ContainsKey(guid))
        {
            Debug.LogError("Player Exit Error :" + guid);
        }
        m_allNetPlayers[guid].OnDestroy();
        m_allNetPlayers.Remove(guid);
    }

    public void PlayerActionSync(SC_Action act)
    {
        if (!m_allNetPlayers.ContainsKey(act.guid))
        {
            Debug.LogError("Player Action Error :" + act.guid);
        }

        m_allNetPlayers[act.guid].SetAction(act.action);
    }
}
