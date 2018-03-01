using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BluetoothConnection : Connection<Message>, IBLECallback
{
    [Tooltip("Mac address of the device to connect to.")]
    // TODO list all discovered devices and let the user choose one
    // TODO remember the last chosen device
    public string MacAddress;
    // Use this for initialization
    void Awake()
    {
        //BCLService.CreateServiceClient();
        BCLService.CreateServiceServer();
    }

    void OnEnable()
    {
        Open();
    }

    void OnDisable()
    {
        Close();
    }

    #region Connection<Message>
    public override event Receiver ReceiveMessage;

    public override void Close()
    {
        BCLService.StopService();
        Debug.Log("Stop bluetooth service.");
    }

    public override void Open()
    {
        BCLService.StartService(MacAddress);
        Debug.Log("Start bluetooth service.");
    }

    public override void Send(Message message)
    {
        Debug.LogFormat("Send bluetooth message: {0}.", message);
        StringBuilder sb = new StringBuilder();
        sb.Append(message.Module);
        sb.Append(',');
        sb.Append(message.Text);
        sb.Append('\0');
        byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
        BCLService.Write(data, data.Length, false);
    }
    #endregion
    #region IBLECallback
    public void OnDidConnect()
    {
        Debug.Log("Bluetooth device connected.");
    }

    public void OnDidDisconnect()
    {
        Debug.Log("Bluetooth device disconnected.");
    }

    public void OnDidReceiveWriteRequests(string base64String)
    {
        byte[] data = System.Convert.FromBase64String(base64String);
        string message = Encoding.UTF8.GetString(data);
        message = message.TrimEnd('\0');
        string[] messageParts = message.Split(',');
        if (ReceiveMessage != null && messageParts.Length >= 2)
            ReceiveMessage(new Message { Module = messageParts[0], Text = messageParts[1] });
    }

    public void OnDidUpdateState()
    {
        Debug.Log("Bluetooth state updated.");
    }
    #endregion
}
