using System;
namespace ShitMan
{
    [Serializable]
    public class SC_PlayerJoin : MessageBase
    {
        public long guid;
        public string name;
        public Vector3 bornPos;

        public SC_PlayerJoin()
        {
            type = MessageTypeEnum.SC_PlayerJoin;
        }
    }
}
