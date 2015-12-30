using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LoginCtrl : MonoBehaviour
{
    public static LoginCtrl instance = null;
    public InputField inputField;
    public Button button;

    public string inputName { get { return inputField.text; } }

    void Awake()
    {
    }

    void Start()
    {
        instance = this;
    }

    void Update()
    {

    }

    public void Hide()
    {
        UIHelper.HideTransform(transform);
    }

    public void Show()
    {
        UIHelper.ShowTransform(transform);
    }

    public void OnJoinClick()
    {
        SessionManager.instance.ConnectToServer();
    }
}

