using UnityEngine;
using System.Collections;

public class FoodCtrl : MonoBehaviour
{
    public long guid;
    public float radius;

    void Start()
    {

    }

    void Update()
    {

    }

    public void Sync(ShitMan.FoodState state)
    {
        TableFoodPos tFood = TableManager.instance.GetPropertiesById<TableFoodPos>(state.tableId);
        Vector3 pos = tFood.pos;
        Vector3 dir = tFood.dir;
        Vector3 scale = Vector3.one * state.radius * 2;
        radius = state.radius;
        transform.position = pos;
        transform.rotation = Quaternion.Euler(dir);
        transform.localScale = scale;
    }

    public void OnDestroy()
    {
        DestroyObject(gameObject);
    }
}
