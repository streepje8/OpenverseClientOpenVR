using System;
using Openverse.Core;
using Openverse.NetCode;
using RiptideNetworking;
using UnityEngine;

public class AudioClient : Singleton<AudioClient>
{
    private void Awake()
    {
        Instance = this;
    }

    public void Connect(object sender, EventArgs e)
    {
        Debug.Log("(AudioClient) Connecting audio client...");
        Message connectMessage = Message.Create(MessageSendMode.reliable,ClientToServerId.audioClientConnect);
        OpenverseNetworkClient.Instance.riptideClient.Send(connectMessage);
    }

    [MessageHandler((ushort)ServerToClientId.createStreamSource)]
    private static void CreateStreamSource(Message message)
    {
        //Implement this
    }
    
    [MessageHandler((ushort)ServerToClientId.deleteStreamSource)]
    private static void DeleteStreamSource(Message message)
    {
        //Implement this
    }
    
    [MessageHandler((ushort)ServerToClientId.modifyStreamSource)]
    private static void ModifyStreamSource(Message message)
    {
        //Implement this
    }
    
    [MessageHandler((ushort)ServerToClientId.streamAudioData)]
    private static void StreamAudioData(Message message)
    {
        
    }
}
