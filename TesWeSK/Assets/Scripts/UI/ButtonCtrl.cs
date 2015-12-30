using UnityEngine;
using System.Collections;

public class ButtonCtrl : MonoBehaviour
{
    public static ButtonCtrl instance;
    public Transform mainBall;
    public UUIEventListener screenDragTrigger;
    private Camera m_mainCamera;
    private CameraCtrl m_cameraCtrl;

    void Start ()
    {
        instance = this;
        m_mainCamera = Camera.main;
        m_cameraCtrl = m_mainCamera.GetComponent<CameraCtrl>();
        screenDragTrigger.onDrag += OnScreenDrag;
    }
	
	void Update ()
    {

	}


    void OnScreenDrag(UUIEventListener listener)
    {
        if (mainBall == null)
            return;
        Vector2 delta = UIHelper.GetListenerDelta(listener);

        mainBall.Rotate(Vector3.up, delta.x);

        m_cameraCtrl.RotateX(delta.y);

        //m_mainCamera.transform.position = Vector3.Lerp(m_mainCamera.transform.position, transform.position + cameraPos, 0.5f);
        //m_mainCamera.transform.forward = mainBall.forward;

        //Vector3 eulr = Camera.main.transform.rotation.eulerAngles;
        //eulr.x -= delta.y;
        //Camera.main.transform.rotation = Quaternion.Euler(eulr);
    }
}
