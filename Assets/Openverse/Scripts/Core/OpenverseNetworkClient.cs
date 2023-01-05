//-------------------------------
//OpenverseNetworkClient
//The script that manages the riptide client
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
        
        [Serializable]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public struct FileHashes
        {
            public string ClientAssets;
            public string ClientScene;
            public string ServerScene;
            public string OpenverseBuilds;
        }

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
            if (serverInfo.OpenverseServerName.Equals("SERVER_CONNECTION_FAILED", StringComparison.OrdinalIgnoreCase))
            {
                ConnectionFailedEvent?.Raise();
                return;
            }
            Debug.Log("(NetworkClient) Downloading/Updating Content...");
            DownloadStartEvent?.Raise();
            string rootURL = OpenverseClient.GetWebAdress(serverInfo.OpenverseServerIP);
            string appdata = Application.persistentDataPath;
            OpenverseClient.Instance.CurrentServer = serverInfo.OpenverseServerName; //MAKE SURE TO ADD VERIFICATION FOR THIS IN THE FUTURE!!!!!!
            string foldername = appdata + "/Openverse/Cache/" + serverInfo.OpenverseServerName + "/";
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            string hasesfile = foldername + "hashes.json";
            using HttpClient client = new HttpClient();
            using HttpResponseMessage response = client.GetAsync(rootURL + "/hashes").Result; //Does this need to be async?
            using HttpContent content = response.Content;
            string json = content.ReadAsStringAsync().Result; //Same for this
            FileHashes reply = JsonConvert.DeserializeObject<FileHashes>(json);
            FileHashes localHashes; //skipcq
            if (File.Exists(hasesfile))
            {
                string jsonLocal = File.ReadAllText(hasesfile);
                localHashes = JsonConvert.DeserializeObject<FileHashes>(json);
                //Update the file
                File.Delete(hasesfile);
                File.WriteAllText(hasesfile,json);
            }
            else
            {
                File.WriteAllText(hasesfile,json);
                localHashes = reply;
            }

            Dictionary<Uri, string> fileGoal = new Dictionary<Uri, string>
            {
                { new Uri(rootURL + "/file/CLIENTASSETS"), foldername + "ClientAssets.asset" },
                { new Uri(rootURL + "/file/CLIENTSCENE"), foldername + "ClientScene.asset" },
                { new Uri(rootURL + "/file/OPENVERSEBUILDS"), foldername + "OpenverseBuilds.asset" }
            };
            if (reply.Equals(localHashes))
            {
                //No need to download new files unless they do not exist
                for (int i = fileGoal.Values.Count - 1; i >= 0; i--)
                {
                    if (File.Exists(fileGoal.Values.ToList()[i]))
                    {
                        fileGoal.Remove(fileGoal.Keys.ToList()[i]);
                    }
                }
            }
            else
            {
                for (int i = fileGoal.Values.Count - 1; i >= 0; i--)
                {
                    if (File.Exists(fileGoal.Values.ToList()[i]))
                    {
                        File.Delete(fileGoal.Values.ToList()[i]); //File is going to be redownloaded so discard our current.
                    }
                }
            }
            AssetLoader.DownloadFilesAsync(fileGoal).ContinueWith(t =>
                {
                    if(t.IsFaulted) Debug.LogException(t.Exception);
                    else
                    {
                        Debug.Log("Files downloaded successfully to: " + foldername);
                        ContinueConnection(serverInfo);
                    }
                    
                }
                ,TaskContinuationOptions.NotOnCanceled);
        }

        private void ContinueConnection(OpenverseServerInfoResponse serverInfo)
        {
            Debug.Log("(NetworkClient) Starting Openverse connection!");
            //if(!ReferenceEquals(ConnectionStartEvent, null)) ConnectionStartEvent.Raise(); //This somehow blocks function execution, idk why
            riptideClient.Connect($"{serverInfo.OpenverseServerIP}:{serverInfo.OpenverseServerPort}");
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
            string appdata = Application.persistentDataPath;
            string foldername = appdata + "/Openverse/Cache/" + OpenverseClient.Instance.CurrentServer + "/";
            OpenverseClient.Instance.Loader.LoadWorld(new List<string>
            {
                foldername + "ClientAssets.asset",
                foldername + "ClientScene.asset",
                foldername + "OpenverseBuilds.asset"
            });
            ConnectedEvent?.Raise();
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