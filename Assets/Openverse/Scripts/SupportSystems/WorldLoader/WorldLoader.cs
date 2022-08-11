using Openverse.Core;
using Openverse.Data;
using RiptideNetworking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Openverse.NetCode.NetworkingCommunications;

namespace Openverse.SupportSystems
{
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
            _ = EnterOpenverseWorld(sceneBundle, clientAssets, sceneAssets);
        }

        private async Task EnterOpenverseWorld(AssetBundle sceneBundle, AssetBundle clientAssets, AssetBundle sceneAssets)
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
    }
}