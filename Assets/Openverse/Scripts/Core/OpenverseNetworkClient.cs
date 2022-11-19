//-------------------------------
//OpenverseNetworkClient
//The script that manages the riptide client
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------

using System.Net.Http;
using Openverse.SupportSystems;

namespace Openverse.Core
{
    using Openverse.Events;
    using Openverse.NetCode;
    using RiptideNetworking;
    using RiptideNetworking.Utils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using static Openverse.NetCode.NetworkingCommunications;

    public class OpenverseNetworkClient : Singleton<OpenverseNetworkClient>
    {
        public Client riptideClient { get; private set; }
        public OpenverseClientSettings settings;
        public GameEvent ConnectedEvent;
        public GameEvent ConnectionFailedEvent;
        public GameEvent OtherClientDisconnectedEvent;
        public GameEvent DisconnectedEvent;
        public GameEvent ConnectionStartEvent;
        public GameEvent ConnectionEndEvent;
        public GameEvent DownloadStartEvent;
        public GameEvent DownloadEndEvent;

        public static Dictionary<Guid, NetworkedObject> NetworkedObjects = new Dictionary<Guid, NetworkedObject>();

        private void Awake()
        {
            Instance = this;
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            riptideClient = new Client();
            riptideClient.Connected += DidConnect;
            riptideClient.ConnectionFailed += FailedToConnect;
            riptideClient.ClientDisconnected += PlayerLeft;
            riptideClient.Disconnected += DidDisconnect;
        }

        public void Connect(OpenverseServerInfoResponse serverInfo)
        {
            //Download the content
            Debug.Log("(NetworkClient) Downloading/Updating Content...");
            using HttpClient client = new HttpClient();
            string rootURL = OpenverseClient.GetWebAdress(serverInfo.OpenverseServerIP);
            string appdata = Application.persistentDataPath;
            OpenverseClient.Instance.currentServer = serverInfo.OpenverseServerName; //MAKE SURE TO ADD VERIFICATION FOR THIS IN THE FUTURE!!!!!!
            string foldername = appdata + "/Openverse/Cache/" + serverInfo.OpenverseServerName + "/";
            
            //TODO CONTINUE FROM HERE
            AssetLoader.DownloadFilesAsync(new Dictionary<Uri, string>(){
                { new Uri(rootURL + ""), foldername + ""}
            },client);
            //Good luck to future me on adding hash checking and file downloading here.
            
            //When its done run this code. Also make sure not to block the thread from this function cuz its will lock the game
            Debug.Log("(NetworkClient) Starting Openverse connection!");
            ConnectionStartEvent?.Raise();
            riptideClient.Connect($"{serverInfo.OpenverseServerIP}:{serverInfo.OpenverseServerPort}"); //Even more good luck on removing the downloading from the game networking!
        }

        public void Disconnect()
        {
            foreach (KeyValuePair<Guid, NetworkedObject> entry in NetworkedObjects)
            {
                Destroy(entry.Value.gameObject);
            }
            NetworkedObjects = new Dictionary<Guid, NetworkedObject>();
            ConnectionEndEvent?.Raise();
            riptideClient.Disconnect();
        }


        private void FixedUpdate()
        {
            riptideClient.Tick();
        }

        private void OnApplicationQuit()
        {
            riptideClient.Disconnect();
            riptideClient.Connected -= DidConnect;
            riptideClient.ConnectionFailed -= FailedToConnect;
            riptideClient.ClientDisconnected -= PlayerLeft;
            riptideClient.Disconnected -= DidDisconnect;
        }

        private void DidConnect(object sender, EventArgs e)
        {
            Message message = Message.Create(MessageSendMode.reliable, ClientToServerId.playerName);
            if (settings.isLoggedIn)
            {
                message.Add(settings.username);
            }
            else
            {
                if (settings.isGuestUser)
                {
                    message.Add("Guest User");
                }
            }
            riptideClient.Send(message);
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
        public void AddObject(Guid id, NetworkedObject obj)
        {
            NetworkedObjects.Add(id, obj);
        }
    }
}