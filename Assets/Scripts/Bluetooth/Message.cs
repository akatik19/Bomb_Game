using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Message
{
    public string Module;
    public string Text;

    public override string ToString()
    {
        return string.Format("Message {{Module={0}, Text={1}}}", Module, Text);
    }
};
