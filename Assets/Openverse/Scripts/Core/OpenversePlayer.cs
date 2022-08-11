//-------------------------------
//OpenversePlayer
//The main handler of all the netcode 
//Might rename this to a name that fits the contents more
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.Core
{
    using Openverse.NetCode;
    using Openverse.Permissions;
    using RiptideNetworking;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using static Openverse.NetCode.NetworkingCommunications;

    public class OpenversePlayer : MonoBehaviour
    {
        public static Dictionary<ushort, VirtualPlayer> list = new Dictionary<ushort, VirtualPlayer>();

        public static void Spawn(ushort id, string username, Vector3 position)
        {
            VirtualPlayer player = null;
            if (!(id == OpenverseNetworkClient.Instance.riptideClient.Id))
            {
                player = Instantiate(OpenverseNetworkClient.Instance.settings.playerPrefab, position, Quaternion.identity).GetComponent<VirtualPlayer>();
                player.name = $"VirtualPlayer {id} ({username})";
            }
            else
            {
                player = OpenverseClient.Instance.player.GetComponent<VirtualPlayer>();
            }
            player.id = id;
            player.username = username;
            list.Add(player.id, player);
        }

        public static void SendVRPositions(VirtualPlayer virtualPlayer)
        {
            if (OpenverseNetworkClient.Instance.riptideClient.IsConnected && virtualPlayer.sendPositions)
            {
                Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.vrPositions);
                message.Add(virtualPlayer.head.transform.localPosition);
                message.Add(virtualPlayer.head.transform.localRotation);
                message.Add(virtualPlayer.handLeft.transform.localPosition);
                message.Add(virtualPlayer.handLeft.transform.localRotation);
                message.Add(virtualPlayer.handRight.transform.localPosition);
                message.Add(virtualPlayer.handRight.transform.localRotation);
                OpenverseNetworkClient.Instance.riptideClient.Send(message);
            }
        }

        #region Messages
        [MessageHandler((ushort)ServerToClientId.RequestInput)]
        private static void OnInputRequest(Message message)
        {
            OpenverseClient.Instance?.player?.input?.OnRequestInput(message);
        }

        [MessageHandler((ushort)ServerToClientId.moveClientMoveable)]
        private static void OnClientMoveableMoved(Message message)
        {
            ClientMoveable.ClientMoveables.TryGetValue(message.GetString(), out ClientMoveable moveable);
            if (moveable != null)
            {
                moveable.transform.position = message.GetVector3();
                moveable.transform.rotation = message.GetQuaternion();
                moveable.transform.localScale = message.GetVector3();
                moveable.lastPOS = moveable.transform.position;
                moveable.lastRot = moveable.transform.rotation;
                moveable.lastScale = moveable.transform.localScale;
            }
        }
        [MessageHandler((ushort)ServerToClientId.addComponent)]
        private static void AddComponentToObject(Message message)
        {
            NetworkedObject.AddComponent(message);
        }
        [MessageHandler((ushort)ServerToClientId.removeComponent)]
        private static void RemoveComponentFromObject(Message message)
        {
            NetworkedObject.RemoveComponent(message);
        }
        [MessageHandler((ushort)ServerToClientId.transformObject)]
        private static void UpdateObjectTransform(Message message)
        {
            Guid recieved = Guid.Parse(message.GetString());
            OpenverseNetworkClient.NetworkedObjects.TryGetValue(recieved, out NetworkedObject networkedObject);
            if (networkedObject != null)
            {
                networkedObject.transform.position = message.GetVector3();
                networkedObject.transform.rotation = message.GetQuaternion();
                networkedObject.transform.localScale = message.GetVector3();
            }
        }

        [MessageHandler((ushort)ServerToClientId.updateVariable)]
        private static void UpdateVariableOnObject(Message message)
        {
            Guid recieved = Guid.Parse(message.GetString());
            if (OpenverseNetworkClient.NetworkedObjects.ContainsKey(recieved))
            {
                NetworkedObject obj = OpenverseNetworkClient.NetworkedObjects[recieved];
                obj.UpdateVariable(message);
            }
            else
            {
                Debug.LogWarning("Tried to sync gameobject with GUID " + recieved.ToString() + " but it no longer exists!");
            }
        }

        [MessageHandler((ushort)ServerToClientId.spawnPlayer)]
        private static void SpawnPlayer(Message message)
        {
            Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
        }

        [MessageHandler((ushort)ServerToClientId.playerLocation)]
        private static void PlayerMovement(Message message)
        {
            ushort playerId = message.GetUShort();
            if (list.TryGetValue(playerId, out VirtualPlayer player))
                player.Move(message.GetVector3(), message.GetVector3());
        }

        [MessageHandler((ushort)ServerToClientId.spawnObject)]
        public static void SpawnObject(Message message)
        {
            NetworkedObject.Spawn(message);
        }

        private static byte[] currentFile = Array.Empty<byte>();
        private static string currentFileName = "";
        private static bool fileHasContents = false;
        private static string currentServer;
        private static bool isDownloading = false;

        private static List<string> files = new List<string>();

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
                if (fileHasContents) { 
                    File.WriteAllBytes(foldername + currentFileName + ".asset", currentFile); 
                    files.Add(foldername + currentFileName + ".asset");
                }
                currentFileName = message.GetString();
                Debug.Log("Saving new file to:" + foldername + currentFileName);
                currentFile = Array.Empty<byte>();
                fileHasContents = false;
            }
            else
            {
                byte[] filebytes = message.GetBytes(true);
                currentFile = MergeArrays(currentFile, filebytes);
                fileHasContents = true;
            }
        }

        [MessageHandler((ushort)ServerToClientId.openWorld)]
        private static void OpenWorld(Message message)
        {
            isDownloading = false;
            PermissionManager.Instance.LoadServerPermissions(currentServer);
            OpenverseClient.Instance.loader.LoadWorld(files);
        }


        #endregion

        private static byte[] MergeArrays(byte[] a, byte[] b)
        {
            byte[] array1 = a;
            byte[] array2 = b;
            byte[] newArray = new byte[array1.Length + array2.Length];
            Array.Copy(array1, newArray, array1.Length);
            Array.Copy(array2, 0, newArray, array1.Length, array2.Length);
            return newArray;
        }

        private static IEnumerable<IEnumerable<byte>> SplitArray(byte[] array, int maxElements)
        {
            for (var i = 0; i < (float)array.Length / maxElements; i++)
            {
                yield return array.Skip(i * maxElements).Take(maxElements);
            }
        }
    }
}