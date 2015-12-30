using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using ShitMan;
using System.Runtime.InteropServices;
using System;

public class WebSocket : MonoBehaviour
{
    public static WebSocket instance;
    void Start()
    {
        instance = this;
    }

    public void Connect(string str)
    {
#if UNITY_WEBGL
        Connect_2(str);
#else
        Connect_1(str);
#endif
    }

    public void SendMsg(MessageBase msg)
    {
#if UNITY_WEBGL
        SendMsg_2(msg);
#else
        SendMsg_1(msg);
#endif

    }

    public void Close()
    {
#if UNITY_WEBGL
        Close_2();
#else
        Close_1();
#endif
    }

    public void JSAlert(string str)
    {
#if UNITY_WEBGL
        AlertJS(str);
#endif
    }

    private void OnOpen()
    {
        Debug.Log("OnOpen");
        MessageEventManager.instance.BroadcastMessage(MessageType.WesocketOpen);
    }

    public void OnMessage(byte[] msg)
    {
        MessageEventManager.instance.BroadcastMessage(MessageType.WesocketOnMessage, msg);
    }

    private void OnClose()
    {
        Debug.Log("SocketClosed");
    }


    /*---------------- WebGl Platform ---------------------*/
    [DllImport("__Internal")]
    private static extern void ConnectJS(string str);
    [DllImport("__Internal")]
    private static extern void SendMsgJS(byte[] data,int length);
    [DllImport("__Internal")]
    private static extern void CloseJS();
    [DllImport("__Internal")]
    private static extern void AlertJS(string str);

    private void Connect_2(string str)
    {
        ConnectJS(str);
    }

    private void SendMsg_2(MessageBase msg)
    {
        byte[] data = SerializeMsg(msg);
        SendMsgJS(data,data.Length);
    }

    private void Close_2()
    {
        CloseJS();
    }

    private void OnOpen_2()
    {
        OnOpen();
    }


    private void OnMessage_2(string msg)
    {
        string []byteIntS = msg.Split(' ');
        byte[] data = new byte[byteIntS.Length];
        for (int i = 0; i < byteIntS.Length; i++)
        {
            data[i] = (byte)int.Parse(byteIntS[i]);
        }
        OnMessage(data);
    }

    private void OnClose_2()
    {
        OnClose();
    }



#if !UNITY_WEBGL
    /*----------------- Other Platform --------------------*/
    private WebSocketSharp.WebSocket sock = null;
    private void Connect_1(string addr)
    {
        if (sock != null && sock.IsConnected)
        {
            MessageEventManager.instance.BroadcastMessage(MessageType.ReLogin);
            return;
        }

        sock = new WebSocketSharp.WebSocket(addr);
        sock.OnOpen += OnPen_1;
        sock.OnMessage += OnMessage_1;
        sock.OnClose += OnClose_1;
        sock.Connect();
    }

    public void SendMsg_1(MessageBase msg)
    {
        byte[] data = SerializeMsg(msg);
        sock.SendAsync(data, null);
    }

    private void Close_1()
    {
        if (sock != null)
            sock.Close();
    }

    private void OnPen_1(object sender, System.EventArgs e)
    {
        OnOpen();
    }

    private void OnMessage_1(object sender, WebSocketSharp.MessageEventArgs e)
    {
        OnMessage(e.RawData);
    }

    private void OnClose_1(object sender, WebSocketSharp.CloseEventArgs e)
    {
        OnClose();
    }
#endif

    /*--------------------- Serialze ------------------------*/
    BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器
    private byte[] SerializeMsg(MessageBase msg)
    {
        MemoryStream stream = new MemoryStream();
        binFormat.Serialize(stream, msg);
        return stream.GetBuffer();
    }
}