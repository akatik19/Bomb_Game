using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModuleIdentifier))]
public class Disarmable : MonoBehaviour
{

    public bool Disarmed
    {
        set
        {
            enabled = !value;
            if (enabled)
            {
                Debug.Log(string.Format("Armed {0}.", gameObject), gameObject);
                SendMessageUpwards("Send", new Message { Module = GetComponent<ModuleIdentifier>().Name, Text = "armed" });
            }
            else
            {
                Debug.Log(string.Format("Disarmed {0}.", gameObject), gameObject);
                SendMessageUpwards("Send", new Message { Module = GetComponent<ModuleIdentifier>().Name, Text = "disarmed" });
            }
        }
        get { return !enabled; }
    }

}
