using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingCommunications : MonoBehaviour
{
    public enum ServerToClientId : ushort
    {
        spawnPlayer = 1,
        playerLocation
    }
    public enum ClientToServerId : ushort
    {
        playerName = 1
    }
}
