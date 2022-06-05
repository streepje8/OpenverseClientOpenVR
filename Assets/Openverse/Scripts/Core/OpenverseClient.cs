//-------------------------------
//OpenverseClient
//The main way of interfacing with the Openverse User Client
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------

namespace Openverse.Core
{
    using Newtonsoft.Json;
    using Openverse.Data;
    using Openverse.Input;
    using Openverse.NetCode;
    using Openverse.Variables;
    using RiptideNetworking;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using static Openverse.NetCode.NetworkingCommunications;
    public class OpenverseClient : Singleton<OpenverseClient>
    {
        public VirtualPlayer player;
        public StringReference nextServer;
        public OpenverseClientSettings settings;
        public static Dictionary<Guid, NetworkedObject> NetworkedObjects = new Dictionary<Guid, NetworkedObject>();
        public UserSettings userSettings;

        private bool goToHome = true;
        [HideInInspector]public UnityEngine.Object[] allClientAssets;
        [HideInInspector]public AssetBundle clientAssets;
        [HideInInspector]public string currentServer = "";
        public bool isConnected
        {
            get;
            private set;
        }

        public void OnValidate()
        {
#if UNITY_EDITOR
            if (settings == null) //Try to find the existing settings
                settings = (OpenverseClientSettings)AssetDatabase.LoadAssetAtPath("Assets/Openverse/OpenvereSettings.asset", typeof(OpenverseClientSettings));
            if (settings == null) //Create a new settings object if none is found
            {
                settings = ScriptableObject.CreateInstance<OpenverseClientSettings>();
                AssetDatabase.CreateAsset(settings, "Assets/Openverse/OpenverseSettings.asset");
                AssetDatabase.SaveAssets();
                EditorUtility.FocusProjectWindow();
                Selection.activeObject = settings;
            }
#endif
        }

        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            //Load settings
            LoadSettings();
        }
        
        void LoadSettings()
        {
            string fileLoc = Application.persistentDataPath + "/Openverse/UserData/UserSettings.json";
            if (File.Exists(fileLoc))
            {
                string fileContents = File.ReadAllText(fileLoc);
                userSettings = JsonConvert.DeserializeObject<UserSettings>(fileContents);
            }
            else
            {
                SaveSettings();
            }            
        }

        void SaveSettings()
        {
            string fileLoc = Application.persistentDataPath + "/Openverse/UserData/UserSettings.json";
            string jsonString = JsonConvert.SerializeObject(userSettings, Formatting.Indented);
            if (!File.Exists(fileLoc))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Openverse/UserData");
                File.Create(fileLoc).Dispose();
            }
            File.WriteAllText(fileLoc, jsonString);
        }

        public void connectToNextServer()
        {
            if (settings.isLoggedIn || settings.isGuestUser)
            {
                OpenverseNetworkClient client = Instantiate(settings.clientPrefab).GetComponent<OpenverseNetworkClient>();
                client.settings = settings;
                client.Client.Connected += (object sender, EventArgs e) => { isConnected = true; };
                client.Client.Disconnected += (object sender, EventArgs e) => { isConnected = false; };
                client.Connect(goToHome ? userSettings.HomeServerIP : nextServer, settings.port);
                DontDestroyOnLoad(client);
            }
            goToHome = false;
        }

        public void openWorld(List<string> files)
        {
            OpenverseNetworkClient.Instance.DownloadEndEvent?.Raise();
            AssetBundle sceneBundle = null;
            AssetBundle sceneAssets = null;
            foreach (string file in files)
            {
                var loadedAssetBundle = AssetBundle.LoadFromFile(file);
                if (loadedAssetBundle == null)
                {
                    Debug.Log("Failed to load AssetBundle: " + Path.GetFileNameWithoutExtension(file));
                    return;
                }

                if (Path.GetFileNameWithoutExtension(file).Equals("clientassets", StringComparison.OrdinalIgnoreCase))
                {
                    clientAssets = loadedAssetBundle;
                }

                if (Path.GetFileNameWithoutExtension(file).Equals("clientscene", StringComparison.OrdinalIgnoreCase))
                {
                    sceneBundle = loadedAssetBundle;
                }
                if (Path.GetFileNameWithoutExtension(file).Equals("OpenverseBuilds", StringComparison.OrdinalIgnoreCase))
                {
                    sceneAssets = loadedAssetBundle;
                }
            }
            StartCoroutine(EnterOpenverseWorld(sceneBundle, clientAssets, sceneAssets));
        }

        public void spawnNetworkedObject(Message message)
        {
            NetworkedObject.Spawn(message);
        }

        public object LoadAsset(UnityEngine.Object foundAsset)
        {
            return clientAssets.LoadAsset(foundAsset.name);
        }

        public void downloadStart()
        {
            OpenverseNetworkClient.Instance.DownloadStartEvent?.Raise();
        }

        IEnumerator EnterOpenverseWorld(AssetBundle sceneBundle, AssetBundle clientAssets, AssetBundle sceneAssets)
        {
            AsyncOperation asyncLoad = clientAssets.LoadAllAssetsAsync();

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            asyncLoad = sceneAssets.LoadAllAssetsAsync();

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            string scenePath = sceneBundle.GetAllScenePaths()[0];
            asyncLoad = SceneManager.LoadSceneAsync(scenePath);
            asyncLoad.completed += (AsyncOperation o) =>
            {
                foreach (GameObject go in SceneManager.GetSceneByPath(scenePath).GetRootGameObjects())
                {
                    AllowedComponents.ScanAndRemoveInvalidScripts(go);
                }
            };
            while (!asyncLoad.isDone)
            {

                yield return null;
            }

            allClientAssets = clientAssets.LoadAllAssets();
            //clientAssets.Unload(false);
            sceneAssets.Unload(false);
            sceneBundle.Unload(false);
            settings.onVirtualWorldStartEvent?.Raise();
            OpenverseNetworkClient.Instance.Client.Send(Message.Create(MessageSendMode.reliable, ClientToServerId.playerReady));
        }

        public void AddObject(Guid id, NetworkedObject obj)
        {
            NetworkedObjects.Add(id, obj);
        }
        
        private float deviceCheckTimer = 0f;

        private void Update()
        {
            deviceCheckTimer += Time.deltaTime;
            if (deviceCheckTimer > userSettings.DeviceCheckTime)
            {
                OpenverseInput.UpdateDevices();
                deviceCheckTimer = 0f;
            }                
        }
    }
}