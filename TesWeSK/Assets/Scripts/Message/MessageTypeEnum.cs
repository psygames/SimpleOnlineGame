using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShitMan
{
    [Serializable]
    public enum MessageTypeEnum
    {
        CS_Login = 1001,
        SC_PlayerJoin = 1002,
        SC_PlayerJoinOther = 1003,
        CS_MoveSync = 2001,
        SC_PlayerEatFood = 2002,
        SC_MoveSyncOther = 2003,
        SC_FoodSync = 2004,
        SC_PlayerEatPlayer = 2005,
        SC_PlayerExitOther = 2006,
        CS_Action = 2007,
        SC_Action = 2008,
    }
}
