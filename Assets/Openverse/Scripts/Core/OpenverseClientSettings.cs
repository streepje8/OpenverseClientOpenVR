using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenverseClientSettings : ScriptableObject
{
    public bool isLoggedIn = false;
    public bool isGuestUser { get
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
}
