using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BattleResultItem : MonoBehaviour
{
    public Image bgImage;
    public Text rank;
    public Text playerName;
    public Text level;
    public const float showTime = 0.3f;
    private float m_showTime = 0;

    public const float hideTime = 0.3f;
    private float m_hideTime = 0;

    public float showPosX = 0;
    public float hidePosX = -400f;

    private bool isShow = false;
    private float m_delayTime = 0;

    Color netColor = new Color(0,0.8f,0.3f,0);
    Color mainColor = new Color(0.4f,0.2f,0.3f,0);

    void Start()
    {

    }

    void Update()
    {
        m_delayTime = Mathf.Max(m_delayTime - Time.deltaTime, 0);
        if (m_delayTime > 0)
            return;

        m_showTime = Mathf.Max(m_showTime - Time.deltaTime, 0);
        m_hideTime = Mathf.Max(m_hideTime - Time.deltaTime, 0);

        if (isShow && m_showTime >= 0)
        {
            float lerp = m_showTime / showTime;
            transform.localPosition = new Vector3(Mathf.Lerp(showPosX, hidePosX, lerp), transform.localPosition.y, 0);
        }

        if (!isShow && m_hideTime >= 0)
        {
            float lerp = m_hideTime / hideTime;
            transform.localPosition = new Vector3(Mathf.Lerp(hidePosX, showPosX, lerp), transform.localPosition.y, 0);
        }

        float _alpha = (transform.localPosition.x - hidePosX - 100) / (-hidePosX);
        _alpha = Mathf.Clamp(_alpha, 0, 1);
        UIHelper.SetImageAlpha(bgImage, _alpha);
        UIHelper.SetImageAlpha(rank, _alpha);
        UIHelper.SetImageAlpha(playerName, _alpha);
        UIHelper.SetImageAlpha(level, _alpha);
    }

    public void SetData(string s_rank, string s_name, string s_level,bool isMain)
    {
        rank.text = s_rank;
        playerName.text = s_name;
        level.text = s_level;
        if (isMain)
            bgImage.color = mainColor;
        else
            bgImage.color = netColor;
    }

    public void Show(float delay)
    {
        m_showTime = showTime;
        isShow = true;
        m_delayTime = delay;
    }

    public void Hide(float delay)
    {
        m_hideTime = hideTime;
        isShow = false;
        m_delayTime = delay;
    }

    public void HideNow()
    {
        transform.localPosition = new Vector3(-hidePosX, transform.localPosition.y, 0);
    }
}
