using System;
using UnityEngine;
using System.Collections.Generic;
//消息事件注册和处理,注册和移除监听事件由注册人自己管理
public sealed class MessageEventManager
{
    private static MessageEventManager m_instance;
    public static MessageEventManager instance
    {
        get { return m_instance; }
    }
    private Dictionary<MessageType, List<Action>> messageHandlers_;
    private Dictionary<MessageType, List<Action<object>>> paramMessageHandlers_;
    public static void Create()
    {
        if (m_instance != null)
            m_instance.ClearAll();
        m_instance = new MessageEventManager();
    }
    private MessageEventManager()
    {
        messageHandlers_ = new Dictionary<MessageType, List<Action>>();
        paramMessageHandlers_ = new Dictionary<MessageType, List<Action<object>>>();
    }

    public void AddEventHandler(MessageType eventName, Action handler)
    {
        if (!messageHandlers_.ContainsKey(eventName))
        {
            messageHandlers_.Add(eventName, new List<Action>());
            messageHandlers_[eventName].Add(handler);
        }
        else
        {
            List<Action> handlers = messageHandlers_[eventName];
            if (handlers.Contains(handler))
            {
                Debug.LogError(string.Format("eventName: {0} And handler: {1} is exist", eventName, handler.Method.Name));
            }
            else
            {
                handlers.Add(handler);
            }
        }
    }
    public void AddEventHandler(MessageType eventName, Action<object> handler)
    {
        if (!paramMessageHandlers_.ContainsKey(eventName))
        {
            paramMessageHandlers_.Add(eventName, new List<Action<object>>());
            paramMessageHandlers_[eventName].Add(handler);
        }
        else
        {
            List<Action<object>> handlers = paramMessageHandlers_[eventName];
            if (handlers.Contains(handler))
            {
                Debug.LogError(string.Format("eventName: {0} And handler: {1} is exist", eventName, handler.Method.Name));
            }
            else
            {
                handlers.Add(handler);
            }
        }
    }
    public void RemoveEventHandler(MessageType eventName, Action handler)
    {
        if (messageHandlers_.ContainsKey(eventName))
        {
            messageHandlers_[eventName].Remove(handler);
        }
    }
    public void RemoveEventHandler(MessageType eventName, Action<object> handler)
    {
        if (paramMessageHandlers_.ContainsKey(eventName))
        {
            paramMessageHandlers_[eventName].Remove(handler);
        }
    }

    public void BroadcastMessage(MessageType eventName)
    {
        if (messageHandlers_.ContainsKey(eventName))
        {
            List<Action> handlers = messageHandlers_[eventName];
            for (int i = 0; i < handlers.Count; i++)
            {
                handlers[i]();
            }
        }
    }

    public void BroadcastMessage(MessageType eventName, object param)
    {
        if (paramMessageHandlers_.ContainsKey(eventName))
        {
            List<Action<object>> handlers = paramMessageHandlers_[eventName];
            for (int i = 0; i < handlers.Count; i++)
            {
                handlers[i](param);
            }
        }
    }

    public void ClearAll()
    {
        messageHandlers_.Clear();
        paramMessageHandlers_.Clear();

    }
}
