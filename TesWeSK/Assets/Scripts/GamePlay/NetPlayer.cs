using UnityEngine;
using System.Collections;

public class NetPlayer : PlayerBaseCtrl
{
    public float frameInterval = 0;
    private float m_moveSyncLerp = 0;
    private Vector3 m_lastPos;
    private Quaternion m_lastRot;
    private Vector3 m_lastScale;

    private Vector3 m_tarPos;
    private Quaternion m_tarRot;
    private Vector3 m_tarScale;
    private Animator m_animator = null;

    private float m_lastSyncTime = 0;

    private bool m_isRuning = false;
    private bool m_isJump = false;

    void Start()
    {
        isMine = false;
        frameInterval = GameConfig.netPlayerMoveSyncInterval;
        m_animator = GetComponent<Animator>();
    }

    void Update()
    {
        UpdateMove();
        UpdateAnimator();
    }

    void UpdateMove()
    {
        m_moveSyncLerp += Time.deltaTime;
        float lerp = m_moveSyncLerp / frameInterval;
        lerp = Mathf.Clamp(lerp, 0, 1);
        transform.position = Vector3.Slerp(m_lastPos, m_tarPos, lerp);
        transform.rotation = Quaternion.Slerp(m_lastRot, m_tarRot, lerp);
        transform.localScale = Vector3.Lerp(m_lastScale, m_tarScale, lerp);

    }

    public void Sync(ShitMan.PlayerState state)
    {
        radius = state.radius;

        m_lastPos = transform.position;
        m_lastRot = transform.rotation;
        m_lastScale = transform.localScale;

        m_tarPos = MathHelper.TransplantToVector3(state.pos);
        m_tarRot = Quaternion.Euler(MathHelper.TransplantToVector3(state.dir));
        m_tarScale = Vector3.one * radius * 2;
        m_moveSyncLerp = 0;
    }

    public void SetAction(int action)
    {
        m_isJump = GetActionValue(action, 1);
    }

    private bool GetActionValue(int action, int i)
    {
        return (action & (1 << i)) > 0;
    }

    void UpdateAnimator()
    {
        // check
        Vector2 xzDisV = (m_tarPos - m_lastPos);
        xzDisV.y = 0;
        if (xzDisV.sqrMagnitude > 0.001f)
            m_isRuning = true;
        else
            m_isRuning = false;
        // set
        if (m_isRuning)
        {
            m_animator.SetFloat("Speed", 1);
        }
        else
        {
            m_animator.SetFloat("Speed", 0);
        }
        if (m_isJump)
        {
            m_animator.SetBool("Jump", true);
            m_isJump = false;
        }
        else
        {
            m_animator.SetBool("Jump", false);
        }
    }

    public void OnDestroy()
    {
        DestroyObject(gameObject);
    }
}
