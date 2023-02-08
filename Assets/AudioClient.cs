using System;
using System.Collections.Generic;
using Openverse.Core;
using Openverse.NetCode;
using RiptideNetworking;
using UnityEngine;

public class AudioClient : Singleton<AudioClient>
{
    public GameObject sourcePrefab;

    private Dictionary<Guid, AudioSourceStream> sources = new Dictionary<Guid, AudioSourceStream>();

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
    private static void CreateStreamSource(Message message) => Instance.CreateSource(message);

    private void CreateSource(Message message)
    {
        AudioSourceStream source = Instantiate(sourcePrefab, Vector3.zero, Quaternion.identity).GetComponent<AudioSourceStream>();
        Guid id = Guid.Parse(message.GetString());
        source.transform.position = message.GetVector3();
        source.SetVolume(message.GetFloat());
        source.SetID(id);
        sources.Add(id,source);
    }

    [MessageHandler((ushort)ServerToClientId.deleteStreamSource)]
    private static void DeleteStreamSource(Message message)
    {
        Guid id = new Guid(message.GetBytes());
        Instance.DeleteSource(id);
    }

    private void DeleteSource(Guid id)
    {
        if (sources.TryGetValue(id, out AudioSourceStream val))
        {
            Destroy(val.gameObject);
            sources.Remove(id);
        }
        Debug.LogWarning("(AudioClient) Server tried to delete an audio source that doesn't exist!");
    }

    [MessageHandler((ushort)ServerToClientId.modifyStreamSource)]
    private static void ModifyStreamSource(Message message)
    {
        //Implement this
    }
    
    [MessageHandler((ushort)ServerToClientId.streamAudioData)]
    private static void StreamAudioData(Message message)
    {
        Guid id = new Guid(message.GetBytes());
        int streamingIndex = message.GetInt();
        byte[] dataChunk = message.GetBytes(true);
        Instance.GetSource(id)?.Buffer(streamingIndex,dataChunk);
    }

    private AudioSourceStream GetSource(Guid id)
    {
        if (sources.TryGetValue(id, out AudioSourceStream stream)) return stream;
        return null;
    }
}
