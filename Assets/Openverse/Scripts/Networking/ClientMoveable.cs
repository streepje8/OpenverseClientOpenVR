using RiptideNetworking;
using System;
using System.Collections.Generic;
using UnityEngine;
using static NetworkingCommunications;

public class ClientMoveable : MonoBehaviour
{
    public MessageSendMode mode = MessageSendMode.unreliable;
    public string myID = Guid.NewGuid().ToString();
    public bool autoSync = false;

    public static Dictionary<string, ClientMoveable> ClientMoveables = new Dictionary<string, ClientMoveable>();
    
    [HideInInspector]public Vector3 lastPOS;
    [HideInInspector]public Quaternion lastRot;
    [HideInInspector]public Vector3 lastScale;

    private void Awake()
    {
        ClientMoveables.Add(myID, this);
        lastPOS = transform.position;
        lastRot = transform.rotation;
        lastScale = transform.localScale;
    }

    private void FixedUpdate()
    {
        if(autoSync)
        {
            if(transform.position != lastPOS || transform.rotation != lastRot || transform.localScale != lastScale)
            {
                lastPOS = transform.position;
                lastRot = transform.rotation;
                lastScale = transform.localScale;
                Sync();
            }
        }
    }

    public void Move(Vector3 newPos)
    {
        transform.position = newPos;
        lastPOS = newPos;
        Sync();
    }

    public void Scale(Vector3 newScale)
    {
        transform.localScale = newScale;
        lastScale = newScale;
        Sync();
    }

    public void Rotate(Quaternion newRot)
    {
        transform.rotation = newRot;
        lastRot = newRot;
        Sync();
    }

    public void Sync() {
        Message updateServer = Message.Create(MessageSendMode.unreliable,ClientToServerId.moveClientMoveable);
        updateServer.Add(myID);
        updateServer.Add(lastPOS);
        updateServer.Add(lastRot);
        updateServer.Add(lastScale);
        OpenverseNetworkClient.Instance.Client.Send(updateServer);
    }
}
