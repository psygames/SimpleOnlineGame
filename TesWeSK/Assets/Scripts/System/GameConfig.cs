using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class GameConfig
{
    public const float moveSyncInterval = 0.05f;
    public const float netPlayerMoveSyncInterval = 0.2f;

    public const string localServ = "ws://127.0.0.1:8730/ShitMan";
    public const string webServ = "ws://115.28.245.188:8730/ShitMan";
    public const string serverAddress = localServ;
}
