//-------------------------------
//NetworkedObjectHandler
//This script handles all packets related to networked objects
//
//Author: streep
//Creation Date: 12-04-2022/12-08-2022
//--------------------------------
namespace Openverse.NetCode
{
    using Openverse.Core;
    using RiptideNetworking;
    using System;
    using UnityEngine;

    public class NetworkedObjectHandlers
    {
        [MessageHandler((ushort)ServerToClientId.spawnObject)]
        public static void SpawnObject(Message message)
        {
            NetworkedObject.Spawn(message);
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
    }
}