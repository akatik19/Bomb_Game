using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HardwareSymbolGame : MonoBehaviour
{

    public string DisarmedMessage = "disarmed";

    public string ResetMessage = "reset";

    public string ImagesMessage = "Images";
    public string ColumnMessage = "Column";

    public GameObject[] HardwareDisplays;

    private int _expectedColumn;

    void OnEnable()
    {
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

        if (message.Text.Contains(ImagesMessage))
        {
            string[] messageParts = message.Text.Split(':');
            if (messageParts.Length >= 2)
            {
                int[] imageIds = messageParts[1].Split(',').Select(idString => int.Parse(idString)).ToArray();
                int count = Mathf.Min(imageIds.Length, HardwareDisplays.Length);
                for (int i = 0; i < count; i++)
                {
                    var gameController = GameObject.FindGameObjectWithTag("GameController");
                    Sprite sprite = gameController.GetComponentInChildren<SymbolGame>().GetSymbol(imageIds[i], _expectedColumn);
                    Debug.LogFormat("Set display {0} to {1}", i, sprite);

                    HardwareDisplays[i].GetComponent<HardwareDisplay>().Display(sprite);
                }
            }
        }
        else if (message.Text.Contains(ColumnMessage))
        {
            string[] messageParts = message.Text.Split(':');
            if (messageParts.Length >= 2)
            {
                if (!int.TryParse(messageParts[1], out _expectedColumn))
                    Debug.LogFormat("Couldn't parse '{0}' to int.", messageParts[1]);
            }
        }
    }
}
