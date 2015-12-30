using System;
namespace ShitMan
{
    [Serializable]
    public class SC_MoveSyncOther : MessageBase
    {
        public PlayerState[] playerStateList;
        public SC_MoveSyncOther()
        {
            type = MessageTypeEnum.SC_MoveSyncOther;
        }
    }

    [Serializable]
    public class PlayerState
    {
        public long guid;
        public Vector3 pos;
        public Vector3 dir;
        public float radius;
    }
}
