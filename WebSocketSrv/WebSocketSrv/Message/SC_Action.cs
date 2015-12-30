using System;
namespace ShitMan
{
    [Serializable]
    public class SC_Action : MessageBase
    {
        public long guid;
        public int action;
        
        public SC_Action()
        {
            type = MessageTypeEnum.SC_Action;
        }
    }
}
