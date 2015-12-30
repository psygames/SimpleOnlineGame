using System;
using UnityEngine;

class MathHelper
{
    public static Vector3 TransplantToVector3(ShitMan.Vector3 src)
    {
        return new Vector3(src.x, src.y, src.z);
    }

    public static ShitMan.Vector3 TransplantToMsgVector3(Vector3 src)
    {
        return new ShitMan.Vector3(src.x, src.y, src.z);
    }
}
