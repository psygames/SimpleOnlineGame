using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DynamicDeleteObj : MonoBehaviour
{
    List<string> filterNames = new List<string> {
        "Cabbage1", "Cabbage2", "Pumpkin1", "Pumpkin2", "Pumpkin3","Pumpkin4",
        "Watermelon", "Watermelon1", "Watermelon2", "Carrot1", "Carrot2",
        "Corn1", "Corn2", "Corn3" };
    void Start()
    {
        DeleteObj(transform);
    }

    void DeleteObj(Transform root)
    {
        for (int i = 0; i < filterNames.Count; i++)
        {
            if (root.name.Equals(filterNames[i]))
            {
                DestroyObject(root.gameObject);
            }
        }

        for (int i = 0; i < root.childCount; i++)
        {
            DeleteObj(root.GetChild(i));
        }
    }
}
