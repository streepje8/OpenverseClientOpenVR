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
        Debug.Log("Waiting for server to supply metaverse world...");
        //possibility to start a timer that suggests a disconnect after 60 seconds
    }

    private void FailedToConnect(object sender, EventArgs e)
    {
        //UIManager.Singleton.BackToMain();
    }

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(OpenversePlayer.list[e.Id].gameObject);
    }

    private void DidDisconnect(object sender, EventArgs e)
    {
        //UIManager.Singleton.BackToMain();

        foreach (OpenversePlayer player in OpenversePlayer.list.Values)
            Destroy(player.gameObject);
    }
}
