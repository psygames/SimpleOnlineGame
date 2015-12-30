using System;
namespace ShitMan
{
    [Serializable]
    public class SC_PlayerExitOther : MessageBase
    {
        public long guid;

        public SC_PlayerExitOther()
        {
            type = MessageTypeEnum.SC_PlayerExitOther;
        }
    }
}
