using UnityEngine;
using System.Collections;
public class MoveCtrl : MonoBehaviour
{
    public Transform scroTrans;

    public const float gravityA = 9.8f; //重力加速度
    public const float maxSpeed = 6;
    public const float speedAcc = 108;
    public const float speedDcc = 100;

    public bool isGrounded;
    public bool lastIsGrounded;

    public Vector3 m_moveSpeed = Vector3.zero;
    public float m_speedAcc = 8;
    public float m_speedDcc = 3;


    private Vector2 m_InputDir = Vector2.zero;
    private Camera m_mainCamera = null;
    private CharacterController m_characterCtrl = null;
    private Animator m_animator = null;

    private bool m_isRuning = false;
    private bool m_isJump = false;

    void Start()
    {
        m_characterCtrl = GetComponent<CharacterController>();
        m_animator = GetComponent<Animator>();
        m_mainCamera = Camera.main;
        lastSyncTime = 0;
        m_speedAcc = speedAcc;

        ButtonCtrl.instance.mainBall = transform;
        m_mainCamera.GetComponent<CameraCtrl>().mainBall = transform;
    }

    void Update()
    {
        CalGravity();
        CheckInput();
        CalSpeedXZ();
        UpdateMove();
        UpdateAction();
        UpdateFrameSync();
    }

    void UpdateAction()
    {
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
            SessionManager.instance.SendAction(1 << 1);
        }
        else
        {
            m_animator.SetBool("Jump", false);
        }
    }

    void UpdateMove()
    {
        m_characterCtrl.Move(m_moveSpeed * Time.deltaTime);
        if (scroTrans != null)
            scroTrans.rotation = Quaternion.Euler(m_moveSpeed.z * 100 * Time.deltaTime, 0, -m_moveSpeed.x * 100 * Time.deltaTime) * scroTrans.rotation;
    }

    private float lastSyncTime = 0;
    void UpdateFrameSync()
    {
        lastSyncTime += Time.deltaTime;
        if (lastSyncTime >= GameConfig.moveSyncInterval)
        {
            lastSyncTime -= GameConfig.moveSyncInterval;
            Sync();
        }
    }

    void Sync()
    {
        ShitMan.CS_MoveSync msg = new ShitMan.CS_MoveSync();
        msg.pos = MathHelper.TransplantToMsgVector3(transform.position);
        msg.dir = MathHelper.TransplantToMsgVector3(transform.rotation.eulerAngles);
        SessionManager.instance.Send(msg);
    }

    private void CalGravity()
    {
        lastIsGrounded = isGrounded;
        isGrounded = m_characterCtrl.isGrounded;
        if (isGrounded)
        {
            m_moveSpeed.y = -0.1f;
        }
        else
        {
            m_moveSpeed += Vector3.down * gravityA * Time.deltaTime;
        }

    }

    private void CalSpeedXZ()
    {
        if (isGrounded)
        {
            m_speedDcc = speedDcc;
            m_speedAcc = speedAcc;
        }
        else
        {
            m_speedDcc = 0.5f * speedDcc;
            m_speedAcc = 0.5f * speedAcc;
        }

        Vector3 moveDir = Vector3.zero;
        if (m_InputDir != Vector2.zero)
        {
            m_InputDir = m_InputDir.normalized;
            moveDir = transform.forward * m_InputDir.y;
            moveDir += transform.right * m_InputDir.x;
            moveDir.Normalize();
        }

        Vector2 curXZMoveDir = new Vector2(moveDir.x, moveDir.z);
        Vector2 curXZSpeed = new Vector2(m_moveSpeed.x, m_moveSpeed.z);
        curXZSpeed += curXZMoveDir * m_speedAcc * Time.deltaTime;
        if (m_speedDcc * Time.deltaTime >= curXZSpeed.magnitude)
            curXZSpeed = Vector2.zero;
        else
            curXZSpeed += (-curXZSpeed.normalized) * m_speedDcc * Time.deltaTime;
        curXZSpeed = Vector2.ClampMagnitude(curXZSpeed, maxSpeed);

        if (curXZMoveDir == Vector2.zero && curXZSpeed.sqrMagnitude <= 0.01f)
        {
            curXZSpeed = Vector2.zero;
        }

        m_moveSpeed.x = curXZSpeed.x;
        m_moveSpeed.z = curXZSpeed.y;
    }

    private void CheckInput()
    {
        Vector2 dir = Vector2.zero;
        if (Input.GetKey("w"))
        {
            dir.y += 1;
        }
        if (Input.GetKey("s"))
        {
            dir.y -= 1;
        }
        if (Input.GetKey("a"))
        {
            dir.x -= 1;
        }
        if (Input.GetKey("d"))
        {
            dir.x += 1;
        }
        m_InputDir = dir;

        if (m_InputDir != Vector2.zero)
        {
            m_isRuning = true;
        }
        else
        {
            m_isRuning = false;
        }

        if (Input.GetKey("space") && isGrounded)
        {
            m_moveSpeed += Vector3.up * 5;
            m_isJump = true;
        }
    }
}
