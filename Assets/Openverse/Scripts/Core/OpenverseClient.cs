//-------------------------------
//OpenverseClient
//The main way of interfacing with the Openverse User Client
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
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
            get => userSettingsManager.GetSettings();
        }

        void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            //Start internal systems
            userSettingsManager.Load();
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
            catch (Exception e)
            {
                Debug.LogException(e);
                OpenverseServerInfoResponse reply = new OpenverseServerInfoResponse();
                reply.OpenverseServerName = "SERVER_CONNECTION_FAILED";
                return reply;
            }
        }
        
        public void ConnectTo(OpenverseServerInfoResponse homeResponse)
        {
            if (settings.isLoggedIn || settings.isGuestUser)
            {
                if (networkClient == null)
                {
                    networkClient = Instantiate(settings.clientPrefab).GetComponent<OpenverseNetworkClient>();
                    networkClient.settings = settings;
                    networkClient.riptideClient.Connected += (object sender, EventArgs e) => isConnected = true;
                    networkClient.riptideClient.Disconnected += (object sender, EventArgs e) => isConnected = false;
                    DontDestroyOnLoad(networkClient);
                }
                if (isConnected) networkClient.Disconnect();
                AsyncOperation operation = SceneManager.LoadSceneAsync(1);
                operation.completed += (AsyncOperation o) => networkClient.Connect(homeResponse);
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