//-------------------------------
//Worldloader
//This script is responsible for recieving and loading worlds
//
//Author: streep
//Creation Date: 12-08-2022
//--------------------------------
namespace Openverse.SupportSystems
{
    using Openverse.Core;
    using Openverse.Data;
    using Openverse.NetCode;
    using RiptideNetworking;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class WorldLoader
    {
        private AssetBundle clientAssets;
        public UnityEngine.Object[] allClientAssets;

        public void LoadWorld(List<string> files)
        {
            OpenverseNetworkClient.Instance.DownloadEndEvent?.Raise();
            AssetBundle sceneBundle = null;
            AssetBundle sceneAssets = null;
            foreach (string file in files)
            {
                Debug.Log("Loading file: " + file);
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
            _ = EnterOpenverseWorldAsync(sceneBundle, clientAssets, sceneAssets);
        }

        private async Task EnterOpenverseWorldAsync(AssetBundle sceneBundle, AssetBundle clientAssets, AssetBundle sceneAssets)
        {
            AsyncOperation asyncLoad = clientAssets.LoadAllAssetsAsync();

            while (!asyncLoad.isDone)
            {
                await Task.Delay(100);
            }

            asyncLoad = sceneAssets.LoadAllAssetsAsync();

            while (!asyncLoad.isDone)
            {
                await Task.Delay(100);
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

                await Task.Delay(100);
            }

            allClientAssets = clientAssets.LoadAllAssets();
            sceneAssets.Unload(false);
            sceneBundle.Unload(false);
            OpenverseClient.Instance.settings.onVirtualWorldStartEvent?.Raise();
            OpenverseNetworkClient.Instance.riptideClient.Send(Message.Create(MessageSendMode.reliable, ClientToServerId.playerReady));
        }

        public object LoadAsset(UnityEngine.Object foundAsset)
        {
            return clientAssets.LoadAsset(foundAsset.name);
        }

        /*
        private static byte[] currentFile = Array.Empty<byte>();
        private static string currentFileName = "";
        private static bool fileHasContents = false;
        public static string currentServer;
        public static bool isDownloading = false;
        public static List<string> files = new List<string>();
        
        [MessageHandler((ushort)ServerToClientId.downloadWorld)]
        private static void RecieveWorld(Message message)
        {
            //Add a timer that goes to 0 when this function is called, and when it hits 60 seconds then cancel the download because the server clearly has failed
            if (!isDownloading)
            {
                OpenverseNetworkClient.Instance.DownloadStartEvent?.Raise();
                isDownloading = true;
            }
            string appdata = Application.persistentDataPath;
            currentServer = message.GetString();
            OpenverseClient.Instance.currentServer = currentServer; //MAKE SURE TO ADD VERIFICATION FOR THIS IN THE FUTURE!!!!!!
            string foldername = appdata + "/Openverse/Cache/" + currentServer + "/"; //Make sure this is save
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }
            if (message.GetBool()) //IsNewFile
            {
                if (fileHasContents)
                {
                    File.WriteAllBytes(foldername + currentFileName + ".asset", currentFile);
                    string filename = foldername + currentFileName + ".asset";
                    if (!files.Contains(filename))
                    {
                        files.Add(filename);
                    }
                    else
                    {
                        Debug.LogWarning("Recieved duplicate file, file ignored: " + filename);
                    }
                }
                currentFileName = message.GetString();
                Debug.Log("Saving new file to:" + foldername + currentFileName);
                currentFile = Array.Empty<byte>();
                fileHasContents = false;
            }
            else
            {
                byte[] filebytes = message.GetBytes(true);
                currentFile = Util.MergeArrays(currentFile, filebytes);
                fileHasContents = true;
            }
        }

        [MessageHandler((ushort)ServerToClientId.openWorld)]
        private static void OpenWorld(Message message)
        {
            PermissionManager.Instance.LoadServerPermissions(currentServer);
            OpenverseClient.Instance.loader.LoadWorld(files);  
        }
        */
    }
}