//-------------------------------
//Bootstrapper
//The first code that will be ran when the client launches
//
//Author: streep
//Creation Date: 12-08-2022
//--------------------------------
namespace Openverse.Core
{
    using UnityEngine;
    public class Bootstrapper : MonoBehaviour
    {
        void Start()
        {
            Debug.Log("(BOOTLOADER) Requesting home server info...");
            OpenverseServerInfoResponse homeServer =
                OpenverseClient.Instance.GetServerInfo(OpenverseClient.Instance.userSettings.HomeServerIP);
            Debug.Log("(BOOTLOADER) Connecting to server: " + homeServer.OpenverseServerName);
            Debug.Log("(BOOTLOADER) Handing control over to the client.");
            OpenverseClient.Instance.ConnectTo(homeServer);
        }
    }
}
