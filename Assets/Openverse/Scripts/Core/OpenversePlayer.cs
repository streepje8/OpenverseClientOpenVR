//-------------------------------
//OpenversePlayer
//Handles all netcode related to players
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.NetCode
{
    using Openverse.Core;
    using RiptideNetworking;
    using System.Collections.Generic;
    using UnityEngine;

    public class OpenversePlayer : MonoBehaviour
    {
        public static Dictionary<ushort, VirtualPlayer> list = new Dictionary<ushort, VirtualPlayer>();

        [MessageHandler((ushort)ServerToClientId.spawnPlayer)]
        private static void SpawnPlayer(Message message)
        {
            ushort id = message.GetUShort();
            string username = message.GetString();
            Vector3 position = message.GetVector3();
            VirtualPlayer player = null;
            if (!(id == OpenverseNetworkClient.Instance.riptideClient.Id))
            {
                player = Instantiate(OpenverseNetworkClient.Instance.settings.playerPrefab, position, Quaternion.identity).GetComponent<VirtualPlayer>();
                player.name = $"VirtualPlayer {id} ({username})";
            }
            else
            {
                player = OpenverseClient.Instance.player.GetComponent<VirtualPlayer>();
            }
            player.id = id;
            player.username = username;
            list.Add(player.id, player);
        }

        public static void SendVRPositions(VirtualPlayer virtualPlayer)
        {
            if (OpenverseNetworkClient.Instance.riptideClient.IsConnected && virtualPlayer.sendPositions)
            {
                Message message = Message.Create(MessageSendMode.unreliable, ClientToServerId.vrPositions);
                message.Add(virtualPlayer.head.transform.localPosition);
                message.Add(virtualPlayer.head.transform.localRotation);
                message.Add(virtualPlayer.handLeft.transform.localPosition);
                message.Add(virtualPlayer.handLeft.transform.localRotation);
                message.Add(virtualPlayer.handRight.transform.localPosition);
                message.Add(virtualPlayer.handRight.transform.localRotation);
                OpenverseNetworkClient.Instance.riptideClient.Send(message);
            }
        }

        [MessageHandler((ushort)ServerToClientId.playerLocation)]
        private static void PlayerMovement(Message message)
        {
            ushort playerId = message.GetUShort();
            if (list.TryGetValue(playerId, out VirtualPlayer player))
                player.Move(message.GetVector3(), message.GetVector3());
        }

    }
}