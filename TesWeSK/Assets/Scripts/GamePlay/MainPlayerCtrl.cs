using UnityEngine;
using System.Collections;

public class MainPlayerCtrl : PlayerBaseCtrl
{
    void Start()
    {
        isMine = true;
    }

    void Update()
    {

    }

    public void Sync()
    {

    }

    public void OnEatFood(long guid, float radius)
    {
        this.radius = Mathf.Pow(this.radius * this.radius * this.radius + radius * radius * radius, 1/3f);
        transform.localScale = Vector3.one * this.radius * 2;
    }

    public void OnEatPlayer(long guid, float radius)
    {
        this.radius = Mathf.Pow(this.radius * this.radius * this.radius + radius * radius * radius, 1 / 3f);
        transform.localScale = Vector3.one * this.radius * 2;
    }

    public void OnDestroy()
    {
        DestroyObject(gameObject);
    }
}
