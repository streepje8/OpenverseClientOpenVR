using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkingCommunications : MonoBehaviour
{
    public enum ServerToClientId : ushort
    {
        downloadWorld = 1,
        openWorld,
        spawnPlayer,
        playerLocation,
        spawnObject,
        updateObject,
        updateVariable,
        transformObject,
        addComponent
    }
    public enum ClientToServerId : ushort
    {
        playerName = 1,
        vrPositions,
        playerReady
    }
}
