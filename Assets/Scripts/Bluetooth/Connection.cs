using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Connection<T> : MonoBehaviour
{

    public delegate void Receiver(T message);

    public abstract event Receiver ReceiveMessage;

    public abstract void Open();

    public abstract void Close();

    public abstract void Send(T message);
}
