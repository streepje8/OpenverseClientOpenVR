//-------------------------------
//NetworkingCommunications
//Stores the meanings of the packet indexes
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.NetCode
{
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
            addComponent,
            removeComponent,
            moveClientMoveable,
            RequestInput
        }

        public enum ClientToServerId : ushort
        {
            playerName = 1,
            vrPositions,
            playerReady,
            moveClientMoveable,
            supplyInput
        }
    }
}
