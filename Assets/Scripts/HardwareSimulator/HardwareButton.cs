using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HardwareButton : MonoBehaviour
{

    public GameObject Button;

    public string Message;

    private void OnEnable()
    {
        if (Button)
            Button.GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        HardwareConnection con = transform.GetComponentInParent<HardwareConnection>();
        con.Send(new Message { Module = transform.GetComponentInParent<ModuleIdentifier>().Name, Text = Message });
    }

    private void OnDisable()
    {
        if (Button)
            Button.GetComponent<Button>().onClick.RemoveListener(OnClick);
    }
}
