using UnityEngine;
using System.Collections;

public class CameraCtrl : MonoBehaviour
{
    public Transform mainBall;
    public Vector3 cameraPos;

    void Start()
    {
        cameraPos = new Vector3(0, 3, -6);
    }

    void Update()
    {
        if (mainBall != null)
        {
            Vector3 offset = mainBall.right * cameraPos.x + mainBall.up * cameraPos.y + mainBall.forward * cameraPos.z;
            transform.position = mainBall.position + offset;
            Vector3 eulr = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(eulr.x, mainBall.rotation.eulerAngles.y, eulr.z);


            /*------------摄像机碰到遮挡物-----------------*/
            Vector3 curCamPos = transform.position;
            RaycastHit hitInfo;
            Vector3 origin = mainBall.position + Vector3.up * 1.8f;
            float distPlayToCamera = Vector3.Distance(origin, curCamPos);
            Vector3 direction = (curCamPos - origin).normalized;
            int layerMask = 1 << gameObject.layer;
            bool hit = Physics.Raycast(
                origin,
                direction,
                out hitInfo, distPlayToCamera, layerMask);
            if (hit)
            {
                curCamPos = hitInfo.point + (hitInfo.point - origin).normalized * 0.1f;
            }

            transform.position = curCamPos;
        }
    }
    private float m_curUIVerticalRotateAngle = 0;

    public void RotateX(float x)
    {
        x = 0.1f * x;
        float angleX = transform.rotation.eulerAngles.x;
        angleX = FomateAngle(angleX);
        m_curUIVerticalRotateAngle += -x;
        m_curUIVerticalRotateAngle = Mathf.Clamp(m_curUIVerticalRotateAngle, -45, 45);

        Vector3 eulr = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(m_curUIVerticalRotateAngle, eulr.y, eulr.z);
    }

    private float FomateAngle(float angle)
    {
        if (angle > 180)
            angle = angle % 360 - 360;
        if (angle < -180)
            angle = angle % 360 + 360;
        return angle;
    }
}
