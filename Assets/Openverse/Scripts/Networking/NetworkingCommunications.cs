//-------------------------------
//NetworkingCommunications
//Stores the meanings of the packet indexes
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.NetCode
{
    public enum ServerToClientId : ushort
    {
        spawnPlayer = 1,
        playerLocation,
        spawnObject,
        updateVariable,
        transformObject,
        addComponent,
        removeComponent,
        moveClientMoveable,
        requestInput,
        createStreamSource,
        modifyStreamSource,
        streamAudioData,
        deleteStreamSource
    }

    public enum ClientToServerId : ushort
    {
        playerName = 1,
        vrPositions,
        playerReady,
        moveClientMoveable,
        supplyInput,
        audioClientConnect,
        audioData,
        audioClientDisconnect
    }
}
