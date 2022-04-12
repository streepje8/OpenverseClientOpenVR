//-------------------------------
//OpenverseClientSettings
//Stores the openverse client settings
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.Core
{
    using Openverse.Events;
    using UnityEngine;
    public class OpenverseClientSettings : ScriptableObject
    {
        public bool isLoggedIn = false;
        public bool isGuestUser
        {
            get
            {
                return !isLoggedIn;
            }
        }
        public string username { get; private set; }
        public string startupJoinIP;
        public ushort port;
        public GameObject clientPrefab;
        public GameObject playerPrefab;
        public GameObject localPlayerPrefab;
        public GameEvent onVirtualWorldStartEvent;
    }
}