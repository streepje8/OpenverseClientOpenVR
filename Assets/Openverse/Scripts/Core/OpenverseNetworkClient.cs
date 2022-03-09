using Openverse.Events;
using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NetworkingCommunications;

public class OpenverseNetworkClient : Singleton<OpenverseNetworkClient>
{
    public Client Client { get; private set; }
    public OpenverseClientSettings settings;
    public GameEvent ConnectedEvent;
    public GameEvent ConnectionFailedEvent;
    public GameEvent OtherClientDisconnectedEvent;
    public GameEvent DisconnectedEvent;
    public GameEvent ConnectionStartEvent;
    public GameEvent DownloadStartEvent;
    public GameEvent DownloadEndEvent;

    private void Awake()
    {
        Instance = this;
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.ClientDisconnected += PlayerLeft;
        Client.Disconnected += DidDisconnect;
    }

    private void FixedUpdate()
    {
        Client.Tick();
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();

        Client.Connected -= DidConnect;
        Client.ConnectionFailed -= FailedToConnect;
        Client.ClientDisconnected -= PlayerLeft;
        Client.Disconnected -= DidDisconnect;
    }

    public void Connect(string ip, ushort port)
    {
        ConnectionStartEvent?.Raise();
        Debug.Log("ConnectionEvent");
        Client.Connect($"{ip}:{port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.playerName);
        if(settings.isLoggedIn) { 
            message.Add(settings.username);
        } else
        {
            if(settings.isGuestUser)
            {
                message.Add("Guest User");
            }
        }
        Client.Send(message);
        ConnectedEvent?.Raise();
        Debug.Log("Waiting for server to supply metaverse world...");
        //possibility to start a timer that suggests a disconnect after 60 seconds
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        ConnectionFailedEvent?.Raise();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        OtherClientDisconnectedEvent?.Raise();
        Destroy(OpenversePlayer.list[e.Id].gameObject);
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        DisconnectedEvent?.Raise();

        foreach (VirtualPlayer player in OpenversePlayer.list.Values)
            Destroy(player.gameObject);
    }
}
