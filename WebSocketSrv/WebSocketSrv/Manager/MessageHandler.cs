using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ShitMan
{
    class MessageHandler
    {
        private static MessageHandler m_instance;
        public static MessageHandler Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new MessageHandler();
                return m_instance;
            }
        }

        BinaryFormatter binFormat = new BinaryFormatter();//创建二进制序列化器

        public void Handle(long guid, byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            binFormat.Binder = new UBinder();
            MessageBase msg = binFormat.Deserialize(stream) as MessageBase;
            HandleMsg(guid, msg);
        }

        void HandleMsg(long guid, MessageBase msg)
        {
            switch (msg.type)
            {
                case MessageTypeEnum.CS_Login:
                    CS_LoginHandle(guid, msg as CS_Login);
                    break;
                case MessageTypeEnum.CS_MoveSync:
                    CS_MoveSyncHandle(guid, msg as CS_MoveSync);
                    break;
                case MessageTypeEnum.CS_Action:
                    CS_ActionHandle(guid, msg as CS_Action);
                    break;
            }
        }











        /*----------- Handles -----------*/
        void CS_LoginHandle(long guid, CS_Login msg)
        {
            SC_PlayerJoin snd = new SC_PlayerJoin();
            snd.bornPos = MathHelper.RandomPos(15,40);
            snd.bornPos.y = 10;
            snd.guid = guid;
            snd.name = msg.name;
            SessionManager.Instance.SendTo(guid, snd);

            SC_PlayerJoinOther snd2 = new SC_PlayerJoinOther();
            snd2.guid = guid;
            snd2.name = msg.name;
            SessionManager.Instance.SendToOthers(guid, snd2);

            HallManager.Instance.Print(guid + " ---> " + msg.name);
            EntryManager.Instance.PlayerJoin(guid, msg.name);
            FrameSyncManager.Instance.FrameAdd(guid,new FrameState(snd.bornPos,Vector3.zero));

        }

        void CS_MoveSyncHandle(long guid, CS_MoveSync msg)
        {
            EntryManager.Instance.MoveSync(guid, msg.pos, msg.dir);
            FrameState frameState = new FrameState(msg.pos, msg.dir);
            FrameSyncManager.Instance.FrameAdd(guid, frameState);
        }

        void CS_ActionHandle(long guid, CS_Action msg)
        {
            SC_Action action = new SC_Action();
            action.guid = guid;
            action.action = msg.action;
            SessionManager.Instance.SendToOthers(guid, action); // 直接转发Action
        }
    }
















    public class UBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
            return ass.GetType(typeName);
        }
    }
}
