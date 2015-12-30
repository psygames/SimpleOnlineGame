using System;
namespace ShitMan
{
    [Serializable]
    public class CS_Login : MessageBase
    {
        public CS_Login()
        {
            type = MessageTypeEnum.CS_Login;
        }
        public string name;
    }
}
