using UnityEngine;
using System.Collections;

public class AddMeshCollider : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
        Component[] list = gameObject.GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].GetComponent<MeshCollider>() == null)
            {
                list[i].gameObject.AddComponent<MeshCollider>();
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
