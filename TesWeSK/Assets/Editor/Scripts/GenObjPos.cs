using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//自定义Tset脚本
[CustomEditor(typeof(SceneObjPos))]
public class GenObjPos : Editor
{
    List<string> filterNames = new List<string> {
        "Cabbage1", "Cabbage2", "Pumpkin1", "Pumpkin2", "Pumpkin3","Pumpkin4",
        "Watermelon", "Watermelon1", "Watermelon2", "Carrot1", "Carrot2",
        "Corn1", "Corn2", "Corn3" };
    public override void OnInspectorGUI()
    {
        SceneObjPos obj = (SceneObjPos)target;
        if (obj == null) return;
        for (int i = 0; i < filterNames.Count; i++)
        {
            EditorGUILayout.LabelField(filterNames[i]);
        }
        if (GUILayout.Button("生成文件"))
        {
            TableManager.ClearXmlData<TableFoodPos>();
            List<Transform> lst = new List<Transform>();
            GenFile(obj.transform, lst);
            List<TableFoodPos> foodPos = new List<TableFoodPos>();
            for (int i = 0; i < lst.Count; i++)
            {
                TableFoodPos fP = new TableFoodPos(i + 1, lst[i].position,lst[i].eulerAngles, "Prefabs/Scenes/farmDynamicPrefabs/" + lst[i].name);
                foodPos.Add(fP);
            }
            TableManager.AddItemToXml(foodPos.ToArray());
        }
    }

    void GenFile(Transform root, List<Transform> transList)
    {
        for (int i = 0; i < filterNames.Count; i++)
        {
            if (root.name.Equals(filterNames[i]))
            {
                transList.Add(root);
            }
        }

        for (int i = 0; i < root.childCount; i++)
        {
            GenFile(root.GetChild(i), transList);
        }
    }
}
