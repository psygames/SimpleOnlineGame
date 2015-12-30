using System;
namespace ShitMan
{
    [Serializable]
    public class CS_Action : MessageBase
    {
        public CS_Action()
        {
            type = MessageTypeEnum.CS_Action;
        }
        public int action;
    }
}
