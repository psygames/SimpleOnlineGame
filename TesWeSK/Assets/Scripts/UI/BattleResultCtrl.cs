using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleResultCtrl : MonoBehaviour
{
    public UUIEventListener showBattleResultTrigger;
    public Transform itemsRoot;

    private List<BattleResultItem> m_itemList;
    private bool m_isShowingItems = false;

    void Start()
    {
        m_itemList = new List<BattleResultItem>();
        for (int i = 0; i < 11; i++)
        {
            GameObject go = (GameObject)Object.Instantiate(Resources.Load("Prefabs/UI/Items/BattleResultItem"));
            go.transform.SetParent(itemsRoot);
            go.transform.localPosition = Vector3.up * i * -40;
            go.transform.localScale = Vector3.one;
            BattleResultItem _ctrl = go.GetComponent<BattleResultItem>();
            _ctrl.HideNow();
            m_itemList.Add(_ctrl);
        }
        m_isShowingItems = false;

        showBattleResultTrigger.onClick += OnShowBattleResult;
    }

    void Update()
    {

    }

    private int m_lastPlayersCount = 0;
    void OnShowBattleResult(UUIEventListener listener)
    {
        m_isShowingItems = !m_isShowingItems;

        if (m_isShowingItems)
        {
            List<PlayerBaseCtrl> list = EntryManager.instance.GetAllPlayerSorted();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].isMine && i >= m_itemList.Count)
                {
                    list[m_itemList.Count - 1] = list[i];
                }
            }
            m_lastPlayersCount = Mathf.Min(m_itemList.Count, list.Count);
            for (int i = 0; i < m_lastPlayersCount; i++)
            {
                m_itemList[i].SetData((i + 1).ToString(), list[i].playerName, list[i].level.ToString(), list[i].isMine);
                m_itemList[i].Show(0.15f * i);
            }
        }
        else
        {
            for (int i = 0; i < m_lastPlayersCount; i++)
            {
                m_itemList[m_lastPlayersCount - i - 1].Hide(0.15f * i);
            }
        }
    }
}
