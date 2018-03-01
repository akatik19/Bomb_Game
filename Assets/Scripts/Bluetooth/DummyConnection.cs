using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyConnection : Connection<Message>
{
    public override event Receiver ReceiveMessage;

    void Start()
    {
        Open();
    }

    void OnDestroy()
    {
        Close();
    }

    public override void Open()
    {
        Debug.Log("Open connection", this);
    }

    public override void Close()
    {
        Debug.Log("Close connection", this);
    }

    public override void Send(Message message)
    {
        Debug.Log(string.Format("Send message: {0}", message), this);
    }

    private void OnReceiveMessage(Message message)
    {
        Debug.Log(string.Format("Receivce message: {0}", message), this);
        if (ReceiveMessage != null)
            ReceiveMessage(message);
    }
}
