using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HardwareLight : MonoBehaviour
{

    public GameObject Light;

    /// <summary>
    /// Duration time of the blinking in seconds
    /// </summary>
    [Tooltip("Duration time of the blinking in seconds")]
    public float BlinkTime = 1;

    public IEnumerator Blink()
    {
        var colors = Light.GetComponent<Button>().colors;
        var oldColor = colors.normalColor;
        colors.normalColor = Light.GetComponent<Button>().colors.highlightedColor;
        Light.GetComponent<Button>().colors = colors;
        yield return new WaitForSeconds(BlinkTime * 0.5f);
        colors.normalColor = oldColor;
        Light.GetComponent<Button>().colors = colors;
        yield return new WaitForSeconds(BlinkTime * 0.5f);
    }
}
