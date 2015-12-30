using ShitMan;
using UnityEngine;

public class ASystem : MonoBehaviour
{
    void Awake()
    {
        // initial
        MessageEventManager.Create();
        TableManager.Create();
        Debug.Log(MessageHandler.instance.ToString());
        Debug.Log(SessionManager.instance.ToString());
        Debug.Log(EntryManager.instance.ToString());
    }
    void Start()
    {

    }

    void Update()
    {
        MessageHandler.instance.UpdateActive();
    }

    void OnApplicationQuit()
    {
        SessionManager.instance.CloseSocket();
    }
}
