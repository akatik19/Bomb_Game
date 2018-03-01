using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class TimerDebug : MonoBehaviour
{
    [Tooltip("Update interval in seconds.")]
    public float Interval = 1f;
    private Timer _timer;
    // Use this for initialization
    void OnEnable()
    {
        _timer = GetComponent<Timer>();

        StartCoroutine(DebugTime());
    }

    private IEnumerator DebugTime()
    {
        while (enabled)
        {
            Debug.LogFormat("{0:00}:{1:00}", _timer.CurrentTime.Minutes, _timer.CurrentTime.Seconds);
            yield return new WaitForSeconds(Interval);
        }
    }
}
