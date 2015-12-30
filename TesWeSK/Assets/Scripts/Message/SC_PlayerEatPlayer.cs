using System;
namespace ShitMan
{
    [Serializable]
    public class SC_PlayerEatPlayer : MessageBase
    {
        public long eatGuid;
        public long eatedGuid;

        public SC_PlayerEatPlayer()
        {
            type = MessageTypeEnum.SC_PlayerEatPlayer;
        }
    }
}
