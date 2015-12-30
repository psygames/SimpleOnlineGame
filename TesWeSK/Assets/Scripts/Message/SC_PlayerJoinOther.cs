using System;
namespace ShitMan
{
    [Serializable]
    public class SC_PlayerJoinOther : MessageBase
    {
        public long guid;
        public string name;

        public SC_PlayerJoinOther()
        {
            type = MessageTypeEnum.SC_PlayerJoinOther;
        }
    }
}
