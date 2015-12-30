using ShitMan;

class SessionManager
{
    private static SessionManager m_instance;
    public static SessionManager instance
    {
        get
        {
            if (m_instance == null)
                m_instance = new SessionManager();
            return m_instance;
        }
    }

    public void ConnectToServer()
    {
        WebSocket.instance.Connect(GameConfig.serverAddress);
    }
    
    public void Send(MessageBase msg)
    {
        WebSocket.instance.SendMsg(msg);
    }

    public void SendAction(int action)
    {
        CS_Action msg = new CS_Action();
        msg.action = action;
        Send(msg);
    }

    public void CloseSocket()
    {
        WebSocket.instance.Close();
    }
}
