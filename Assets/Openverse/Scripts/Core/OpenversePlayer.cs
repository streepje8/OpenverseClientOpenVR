using RiptideNetworking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static NetworkingCommunications;

public class OpenversePlayer : MonoBehaviour
{
    public static Dictionary<ushort, VirtualPlayer> list = new Dictionary<ushort, VirtualPlayer>();

    public static void Spawn(ushort id, string username, Vector3 position)
    {
        VirtualPlayer player;
        if (!(id == OpenverseNetworkClient.Instance.Client.Id))
        {
            player = Instantiate(OpenverseNetworkClient.Instance.settings.playerPrefab, position, Quaternion.identity).GetComponent<VirtualPlayer>();
            player.name = $"VirtualPlayer {id} ({username})";
        } else
        {
            player = OpenverseClient.Instance.player.GetComponent<VirtualPlayer>();
        }
        player.id = id;
        player.username = username;
        list.Add(player.id, player);
    }

    public static void SendVRPositions(VirtualPlayer virtualPlayer)
    {
        if (OpenverseNetworkClient.Instance.Client.IsConnected)
        {
            Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.vrPositions);
            message.Add(virtualPlayer.head.transform.localPosition);
            message.Add(virtualPlayer.head.transform.localRotation);
            message.Add(virtualPlayer.handLeft.transform.localPosition);
            message.Add(virtualPlayer.handLeft.transform.localRotation);
            message.Add(virtualPlayer.handRight.transform.localPosition);
            message.Add(virtualPlayer.handRight.transform.localRotation);
            OpenverseNetworkClient.Instance.Client.Send(message);
        }
    }

    #region Messages
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
        OpenverseClient.Instance.spawnNetworkedObject(message);
    }

    private static byte[] currentFile = new byte[0];
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
            OpenverseClient.Instance.downloadStart();
            isDownloading = true;
        }
        string appdata = Application.persistentDataPath;
        currentServer = message.GetString();
        string foldername = appdata + "/Openverse/Cache/" + currentServer + "/"; //Make sure this is save
        if(!Directory.Exists(foldername))
        {
            Directory.CreateDirectory(foldername);
        }
        if(message.GetBool()) //IsNewFile
        {
            if (fileHasContents) { File.WriteAllBytes(foldername + currentFileName + ".asset", currentFile); files.Add(foldername + currentFileName + ".asset"); }
            currentFileName = message.GetString();
            Debug.Log("Saving new file to:" + foldername + currentFileName);
            currentFile = new byte[0];
            fileHasContents = false;
        } else
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
        OpenverseClient.Instance.openWorld(files);
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
