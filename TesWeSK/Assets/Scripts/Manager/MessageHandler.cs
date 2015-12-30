using ShitMan;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

class MessageHandler
{
    private static MessageHandler m_instance;
    public static MessageHandler instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new MessageHandler();
            return m_instance;
        }
    }

    private BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
    private List<byte[]> msgQueue = new List<byte[]>();

    public MessageHandler()
    {
        MessageEventManager.instance.AddEventHandler(MessageType.WesocketOpen, OnLogin);
        MessageEventManager.instance.AddEventHandler(MessageType.ReLogin, OnLogin);
        MessageEventManager.instance.AddEventHandler(MessageType.WesocketOnMessage, OnMessage);
    }

    public void OnLogin()
    {
        string name = LoginCtrl.instance.inputName;
        CS_Login msg = new CS_Login();
        msg.name = name;
        SessionManager.instance.Send(msg);
    }

    public void OnMessage(object dataObj)
    {
        byte[] data = (byte[])dataObj;
        msgQueue.Add(data);
    }

    public void UpdateActive()
    {
        // flush msg queue
        lock (msgQueue)
        {
            for (int i = 0; i < msgQueue.Count; i++)
            {
                try
                {
                    HandleMsg(msgQueue[i]);
                }
                catch (Exception e)
                {
                    Debug.Log("Handle Message Error : " + e.StackTrace);
                }
            }
            msgQueue.Clear();
        }
    }

    void HandleMsg(byte[] data)
    {
        MemoryStream stream = new MemoryStream(data);
        binFormat.Binder = new UBinder();
        MessageBase msg = binFormat.Deserialize(stream) as MessageBase;
        switch (msg.type)
        {
            case MessageTypeEnum.SC_PlayerJoin:
                SC_PlayerJoinHandle(msg as SC_PlayerJoin);
                break;
            case MessageTypeEnum.SC_FoodSync:
                SC_FoodSyncHandle(msg as SC_FoodSync);
                break;
            case MessageTypeEnum.SC_PlayerEatFood:
                SC_PlayerEatFoodHandle(msg as SC_PlayerEatFood);
                break;
            case MessageTypeEnum.SC_PlayerEatPlayer:
                SC_PlayerEatPlayerHandle(msg as SC_PlayerEatPlayer);
                break;
            case MessageTypeEnum.SC_MoveSyncOther:
                SC_MoveSyncOtherHandle(msg as SC_MoveSyncOther);
                break;
            case MessageTypeEnum.SC_PlayerJoinOther:
                SC_PlayerJoinOtherHandle(msg as SC_PlayerJoinOther);
                break;
            case MessageTypeEnum.SC_PlayerExitOther:
                SC_PlayerExitOtherHandle(msg as SC_PlayerExitOther);
                break;
            case MessageTypeEnum.SC_Action:
                SC_ActionHandle(msg as SC_Action);
                break;
        }
    }






    /*---------------- message handles ------------*/
    void SC_PlayerJoinHandle(SC_PlayerJoin msg)
    {
        LoginCtrl.instance.Hide();
        EntryManager.instance.MainPlayerJoin(msg.guid, msg.name, msg.bornPos);
    }

    void SC_FoodSyncHandle(SC_FoodSync msg)
    {
        EntryManager.instance.SyncFoods(msg.foodStateList);
    }

    void SC_PlayerEatFoodHandle(SC_PlayerEatFood msg)
    {
        EntryManager.instance.PlayerEatFood(msg.playerGuid, msg.foodGuid);
    }

    void SC_PlayerEatPlayerHandle(SC_PlayerEatPlayer msg)
    {
        EntryManager.instance.PlayerEatPlayer(msg.eatGuid, msg.eatedGuid);
    }

    void SC_MoveSyncOtherHandle(SC_MoveSyncOther msg)
    {
        EntryManager.instance.PlayerSync(msg.playerStateList);
    }

    void SC_PlayerJoinOtherHandle(SC_PlayerJoinOther msg)
    {
        EntryManager.instance.PlayerJoinOther(msg.guid, msg.name);
    }

    void SC_PlayerExitOtherHandle(SC_PlayerExitOther msg)
    {
        EntryManager.instance.PlayerExitOther(msg.guid);
    }

    void SC_ActionHandle(SC_Action msg)
    {
        EntryManager.instance.PlayerActionSync(msg);
    }
}























public class UBinder : SerializationBinder
{
    public override Type BindToType(string assemblyName, string typeName)
    {
        //System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
        //return ass.GetType(typeName);
        Type result = null;
        switch (typeName)
        {
            case "ShitMan.CS_Action":
                result = typeof(ShitMan.CS_Action);
                break;
            case "ShitMan.CS_Action[]":
                result = typeof(ShitMan.CS_Action[]);
                break;
            case "ShitMan.CS_Login":
                result = typeof(ShitMan.CS_Login);
                break;
            case "ShitMan.CS_Login[]":
                result = typeof(ShitMan.CS_Login[]);
                break;
            case "ShitMan.CS_MoveSync":
                result = typeof(ShitMan.CS_MoveSync);
                break;
            case "ShitMan.CS_MoveSync[]":
                result = typeof(ShitMan.CS_MoveSync[]);
                break;
            case "ShitMan.MessageBase":
                result = typeof(ShitMan.MessageBase);
                break;
            case "ShitMan.MessageBase[]":
                result = typeof(ShitMan.MessageBase[]);
                break;
            case "ShitMan.MessageTypeEnum":
                result = typeof(ShitMan.MessageTypeEnum);
                break;
            case "ShitMan.MessageTypeEnum[]":
                result = typeof(ShitMan.MessageTypeEnum[]);
                break;
            case "ShitMan.SC_Action":
                result = typeof(ShitMan.SC_Action);
                break;
            case "ShitMan.SC_Action[]":
                result = typeof(ShitMan.SC_Action[]);
                break;
            case "ShitMan.SC_FoodSync":
                result = typeof(ShitMan.SC_FoodSync);
                break;
            case "ShitMan.SC_FoodSync[]":
                result = typeof(ShitMan.SC_FoodSync[]);
                break;
            case "ShitMan.FoodState":
                result = typeof(ShitMan.FoodState);
                break;
            case "ShitMan.FoodState[]":
                result = typeof(ShitMan.FoodState[]);
                break;
            case "ShitMan.SC_MoveSyncOther":
                result = typeof(ShitMan.SC_MoveSyncOther);
                break;
            case "ShitMan.SC_MoveSyncOther[]":
                result = typeof(ShitMan.SC_MoveSyncOther[]);
                break;
            case "ShitMan.PlayerState":
                result = typeof(ShitMan.PlayerState);
                break;
            case "ShitMan.PlayerState[]":
                result = typeof(ShitMan.PlayerState[]);
                break;
            case "ShitMan.SC_PlayerEatFood":
                result = typeof(ShitMan.SC_PlayerEatFood);
                break;
            case "ShitMan.SC_PlayerEatFood[]":
                result = typeof(ShitMan.SC_PlayerEatFood[]);
                break;
            case "ShitMan.SC_PlayerEatPlayer":
                result = typeof(ShitMan.SC_PlayerEatPlayer);
                break;
            case "ShitMan.SC_PlayerEatPlayer[]":
                result = typeof(ShitMan.SC_PlayerEatPlayer[]);
                break;
            case "ShitMan.SC_PlayerExitOther":
                result = typeof(ShitMan.SC_PlayerExitOther);
                break;
            case "ShitMan.SC_PlayerExitOther[]":
                result = typeof(ShitMan.SC_PlayerExitOther[]);
                break;
            case "ShitMan.SC_PlayerJoin":
                result = typeof(ShitMan.SC_PlayerJoin);
                break;
            case "ShitMan.SC_PlayerJoin[]":
                result = typeof(ShitMan.SC_PlayerJoin[]);
                break;
            case "ShitMan.SC_PlayerJoinOther":
                result = typeof(ShitMan.SC_PlayerJoinOther);
                break;
            case "ShitMan.SC_PlayerJoinOther[]":
                result = typeof(ShitMan.SC_PlayerJoinOther[]);
                break;
            case "ShitMan.Vector3":
                result = typeof(ShitMan.Vector3);
                break;
            case "ShitMan.Vector3[]":
                result = typeof(ShitMan.Vector3[]);
                break;
        }
        return result;
    }
}