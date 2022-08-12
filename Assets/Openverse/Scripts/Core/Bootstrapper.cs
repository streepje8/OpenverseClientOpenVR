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
            OpenverseClient.Instance.ConnectTo(OpenverseClient.Instance.userSettings.HomeServerIP);
        }
    }
}
