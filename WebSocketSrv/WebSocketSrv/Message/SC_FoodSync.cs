using System;
using System.Collections.Generic;
namespace ShitMan
{
    [Serializable]
    public class SC_FoodSync : MessageBase
    {
        public FoodState[] foodStateList;
        public SC_FoodSync()
        {
            type = MessageTypeEnum.SC_FoodSync;
        }
    }

    [Serializable]
    public class FoodState
    {
        public long guid;
        public int tableId;
        public float radius;
    }
}
