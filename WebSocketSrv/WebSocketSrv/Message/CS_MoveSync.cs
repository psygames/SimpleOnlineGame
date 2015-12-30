using System;
namespace ShitMan
{
    [Serializable]
    public class CS_MoveSync : MessageBase
    {
        public Vector3 pos;
        public Vector3 dir;
        public CS_MoveSync()
        {
            type = MessageTypeEnum.CS_MoveSync;
        }
    }
}
