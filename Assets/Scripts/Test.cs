using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(Disarmable))]
public class Test : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        var comp = GetComponent<Disarmable>();
        Debug.Log(string.Format("{0} = {1}", comp, comp.enabled), gameObject);
    }

    public void ToggleDisarmable()
    {
        var comp = GetComponent<Disarmable>();
        comp.Disarmed = !comp.Disarmed;
    }
}
