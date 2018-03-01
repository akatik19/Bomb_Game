using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardwareSimonSays : MonoBehaviour
{
    [Tooltip("Amount of time in seconds to wait after the sequence is finished.")]
    public float TimeAfterSequence = 2;

    public string DisarmedMessage = "disarmed";

    public string ResetMessage = "reset";

    public GameObject RedHardwareButton;
    public GameObject GreenHardwareButton;
    public GameObject BlueHardwareButton;
    public GameObject YellowHardwareButton;

    private List<string> _sequence;

    void Update()
    {
        if (!_showingSequence)
        {
            _showingSequence = true;
            StartCoroutine(ShowSequence());
        }
    }

    bool _showingSequence = false;

    IEnumerator ShowSequence()
    {
        for (int i = 0; i < _sequence.Count; i++)
        {
            var color = _sequence[i];
            switch (color)
            {
                case "Red":
                    yield return RedHardwareButton.GetComponent<HardwareLight>().Blink();
                    break;
                case "Blue":
                    yield return BlueHardwareButton.GetComponent<HardwareLight>().Blink();
                    break;
                case "Green":
                    yield return GreenHardwareButton.GetComponent<HardwareLight>().Blink();
                    break;
                case "Yellow":
                    yield return YellowHardwareButton.GetComponent<HardwareLight>().Blink();
                    break;
                default:
                    Debug.LogWarningFormat(this, "Illegal message '{0}' in sequence.", color);
                    break;
            }
        }

        yield return new WaitForSeconds(TimeAfterSequence);
        _showingSequence = false;
        yield return null;
    }

    void OnEnable()
    {
        _sequence = new List<string>();
        var connection = GetComponentInParent<HardwareConnection>();
        if (connection)
            connection.ReceiveMessage += OnReceiveMessage;
    }

    void OnDisable()
    {
        var connection = GetComponentInParent<HardwareConnection>();
        if (connection)
            connection.ReceiveMessage -= OnReceiveMessage;
    }

    public void OnReceiveMessage(Message message)
    {
        if (message.Module != GetComponent<ModuleIdentifier>().Name)
            return;

        if (message.Text == DisarmedMessage || message.Text == ResetMessage)
            _sequence.Clear();
        else if (message.Text == "Red" || message.Text == "Green" || message.Text == "Blue" || message.Text == "Yellow")
            _sequence.Add(message.Text);
    }
}
