using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NetworkingCommunications;

public class OpenversePlayer : MonoBehaviour
{
    public static Dictionary<ushort, OpenversePlayer> list = new Dictionary<ushort, OpenversePlayer>();

    [SerializeField] private ushort id;
    [SerializeField] private string username;

    public void Move(Vector3 newPosition, Vector3 forward)
    {
        transform.position = newPosition;

        if (id != OpenverseNetworkClient.Instance.Client.Id) // Don't overwrite local player's forward direction to avoid noticeable rotational snapping
            transform.forward = forward;
    }

    private void OnDestroy()
    {
        list.Remove(id);
    }

    public static void Spawn(ushort id, string username, Vector3 position)
    {
        OpenversePlayer player;
        if (id == OpenverseNetworkClient.Instance.Client.Id)
        {
            player = Instantiate(OpenverseNetworkClient.Instance.settings.localPlayerPrefab, position, Quaternion.identity).GetComponent<OpenversePlayer>();
            player.name = $"[LOCAL]Player {id} ({username})";
        }
        else
        {
            player = Instantiate(OpenverseNetworkClient.Instance.settings.playerPrefab, position, Quaternion.identity).GetComponent<OpenversePlayer>();
            player.name = $"Player {id} ({username})";
        }
        
        player.id = id;
        player.username = username;
        list.Add(player.id, player);
    }

    #region Messages
    [MessageHandler((ushort)ServerToClientId.spawnPlayer)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }

    [MessageHandler((ushort)ServerToClientId.playerLocation)]
    private static void PlayerMovement(Message message)
    {
        ushort playerId = message.GetUShort();
        if (list.TryGetValue(playerId, out OpenversePlayer player))
            player.Move(message.GetVector3(), message.GetVector3());
    }
    #endregion
}
