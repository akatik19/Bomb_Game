using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HardwareDisplay : MonoBehaviour
{

    public Button Button;

    public void Display(Sprite sprite)
    {
        Button.GetComponent<Image>().sprite = sprite;
    }
}
