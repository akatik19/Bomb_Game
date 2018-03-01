using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HardwareConnection : Connection<Message>
{

    public override event Receiver ReceiveMessage;

    public HardwareConnection OtherConnection;

    void Start()
    {
        if (OtherConnection == null)
            Debug.LogWarning("This connection is not connected to another connection.", this);
    }

    public override void Close()
    {
    }

    public override void Open()
    {
    }

    public override void Send(Message message)
    {
        OtherConnection.SendMessage("OnReceiveMessage", message);
    }

    private void OnReceiveMessage(Message message)
    {
        if (ReceiveMessage != null)
            ReceiveMessage(message);
    }
}
