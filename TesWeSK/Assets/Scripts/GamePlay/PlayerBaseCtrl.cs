using UnityEngine;
using System.Collections;

public class PlayerBaseCtrl : MonoBehaviour
{
    public long guid;
    public string playerName;
    public float radius;
    public bool isMine;

    public int level { get { return (int)Mathf.Floor((radius-0.5f) / 0.1f); } }
}
