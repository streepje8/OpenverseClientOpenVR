//-------------------------------
//OpenverseClient
//The main way of interfacing with the Openverse User Client
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.Core
{
    using Openverse.Input;
    using Openverse.SupportSystems;
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class OpenverseClient : Singleton<OpenverseClient>
    {
        public VirtualPlayer player;
        public OpenverseNetworkClient networkClient { get; private set; }
        public WorldLoader loader { get; private set; } = new WorldLoader();
        public string currentServer { get; internal set; }

        public bool isConnected
        {
            get;
            private set;
        }

        public OpenverseClientSettings settings;
        public SettingsManager<UserSettings> userSettingsManager { get; private set; } = new SettingsManager<UserSettings>("UserSettings");
        
        public UserSettings userSettings
        {
            get
            {
                return userSettingsManager.GetSettings();
            }
        }

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //Start internal systems
            userSettingsManager.Load();
        }

        public void ConnectTo(string IP)
        {
            if (settings.isLoggedIn || settings.isGuestUser)
            {
                if (networkClient == null)
                {
                    networkClient = Instantiate(settings.clientPrefab).GetComponent<OpenverseNetworkClient>();
                    networkClient.settings = settings;
                    networkClient.riptideClient.Connected += (object sender, EventArgs e) => { isConnected = true; };
                    networkClient.riptideClient.Disconnected += (object sender, EventArgs e) => { isConnected = false; };
                    DontDestroyOnLoad(networkClient);
                }

                if (isConnected) networkClient.Disconnect();
                AsyncOperation operation = SceneManager.LoadSceneAsync(1);
                operation.completed += (AsyncOperation o) => { networkClient.Connect(IP, settings.port); };
            } else
            {
                Debug.LogError("User not logged in!");
            }
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

        #region EDITOR_CODE
        #if UNITY_EDITOR
                public void OnValidate()
                {

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

                }
        #endif
        #endregion
    }
}