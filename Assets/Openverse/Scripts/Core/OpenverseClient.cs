//-------------------------------
//OpenverseClient
//The main way of interfacing with the Openverse User Client
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Newtonsoft.Json;

namespace Openverse.Core
{
    using Openverse.Input;
    using Openverse.SupportSystems;
    using System;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    [Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public struct OpenverseServerInfoResponse
    {
        public string OpenverseServerName;
        public ushort OpenverseServerPort;
        public string OpenverseServerIP; //This field is client filled!
        public string ProtocolVersion;
        public string IconURL;
        public string Description;
    }
    
    public class OpenverseClient : Singleton<OpenverseClient>
    {
        public VirtualPlayer player;
        public OpenverseNetworkClient NetworkClient { get; private set; }
        public WorldLoader Loader { get; private set; } = new WorldLoader();
        public string CurrentServer { get; internal set; }

        public bool IsConnected
        {
            get;
            private set;
        }

        public OpenverseClientSettings settings;
        public SettingsManager<UserSettings> UserSettingsManager { get; private set; } = new SettingsManager<UserSettings>("UserSettings");
        
        public UserSettings userSettings
        {
            get => UserSettingsManager.GetSettings();
        }

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //Start internal systems
            UserSettingsManager.Load();
        }

        public OpenverseServerInfoResponse GetServerInfo(string IP)
        {
            return GetInfoAsync(IP);
        }

        private OpenverseServerInfoResponse GetInfoAsync(string IP)
        {
            try
            {
                HttpClient client = new HttpClient();
                using HttpResponseMessage
                    response = client.GetAsync(GetWebAdress(IP)).Result; //Idk why im using async here tbh
                using HttpContent content = response.Content;
                var json = content.ReadAsStringAsync().Result; //Same for this
                OpenverseServerInfoResponse reply = JsonConvert.DeserializeObject<OpenverseServerInfoResponse>(json);
                reply.OpenverseServerIP = IP;
                return reply;
            }
            catch (Exception e) //skipcq
            {
                Debug.LogException(e);
                OpenverseServerInfoResponse reply = new OpenverseServerInfoResponse()
                {
                    OpenverseServerName = "SERVER_CONNECTION_FAILED"
                };
                return reply;
            }
        }
        
        public void ConnectTo(OpenverseServerInfoResponse homeResponse)
        {
            if (settings.isLoggedIn || settings.isGuestUser)
            {
                if (NetworkClient == null)
                {
                    NetworkClient = Instantiate(settings.clientPrefab).GetComponent<OpenverseNetworkClient>();
                    NetworkClient.settings = settings;
                    NetworkClient.riptideClient.Connected += (object sender, EventArgs e) => IsConnected = true;
                    NetworkClient.riptideClient.Connected += AudioClient.Instance.Connect;
                    NetworkClient.riptideClient.Disconnected += (object sender, EventArgs e) => IsConnected = false;
                    DontDestroyOnLoad(NetworkClient);
                }
                if (IsConnected) NetworkClient.Disconnect();
                AsyncOperation operation = SceneManager.LoadSceneAsync(1);
                operation.completed += (AsyncOperation o) => NetworkClient.Connect(homeResponse);
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

        public static string GetWebAdress(string IP)
        {
            return "http://" + IP.Replace("127.0.0.1", "localhost" + ":8080");
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