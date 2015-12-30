using System;
namespace ShitMan
{
    [Serializable]
    public class SC_PlayerEatFood : MessageBase
    {
        public long playerGuid;
        public long foodGuid;

        public SC_PlayerEatFood()
        {
            type = MessageTypeEnum.SC_PlayerEatFood;
        }
    }
}
