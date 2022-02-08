using RiptideNetworking.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OpenverseClient : MonoBehaviour
{
    public OpenverseClientSettings settings;

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
        //Connect to the server in settings.startupJoinIP and open it
        if(settings.isLoggedIn || settings.isGuestUser) { 
            OpenverseNetworkClient client = Instantiate(settings.clientPrefab).GetComponent<OpenverseNetworkClient>();
            client.settings = settings;
            client.Connect(settings.startupJoinIP, settings.port);
            DontDestroyOnLoad(client);
        }
    }
}
