using System;
using System.Collections.Generic;
using System.Linq;
using Openverse.Audio;
using UnityEngine;
using UnityEngine.Networking.Types;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceStream : MonoBehaviour
{
    public float audioBufferSize = 0.5f;
    public int bufferSize = 1000;
    public int currentlyBuffered = 0;
    public int channels = 2;
    public int sampleRate = 44100;
    public int position = 0;
    public float frequency = 440;

    public int CurrentStreamingIndex { get; private set; } = 0;
    private AudioSource source;
    private Guid id;
    
    private Dictionary<int, byte[]> dataBuffer = new Dictionary<int, byte[]>();
    private float timeSpendWaiting = 0f;
    private List<byte[]> processingBuffer = new List<byte[]>();
    private float[] sampleBuffer = Array.Empty<float>();
    
    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.loop = false;
        source.playOnAwake = false;
        AudioClip clip = AudioClip.Create("Streamed Clip", sampleRate * channels * 3600, channels, sampleRate, true, OnAudioRead, OnAudioSetPosition);
        source.clip = clip;
    }

    public void SetVolume(float value)
    {
        source.volume = value;
    }

    public void SetID(Guid id)
    {
        this.id = id;
    }

    public void Buffer(int streamingIndex, byte[] dataChunk)
    {
        dataBuffer.Add(streamingIndex,dataChunk);
    }
    private void Update()
    {
        if (dataBuffer.TryGetValue(CurrentStreamingIndex, out byte[] data))
        {
            timeSpendWaiting = 0f;
            processingBuffer.Add(data);
            if (processingBuffer.Count > bufferSize)
            {
                byte[] dat = Array.Empty<byte>();
                for (int i = 0; i < processingBuffer.Count; i++)
                {
                    byte[] curdata = AudioUtility.DeCompress(processingBuffer[i]);
                    dat = dat.Concat(curdata).ToArray();
                    
                }
                float[] samples = new float[dat.Length / 4];
                System.Buffer.BlockCopy(dat, 0, samples, 0, dat.Length);
                sampleBuffer = sampleBuffer.Concat(samples).ToArray();
                processingBuffer = new List<byte[]>();
                if(!source.isPlaying) source.Play();
            }
            CurrentStreamingIndex++;
        }
        else
        {
            timeSpendWaiting += Time.deltaTime;
        }
        if (timeSpendWaiting > 0.1f)
        {
            Debug.Log("A streaming index timed out! Skipping it!");
            CurrentStreamingIndex++;
            timeSpendWaiting = 0f;
        }
    }

    void OnAudioRead(float[] data)
    {
        int count = 0;
        while (count < data.Length)
        {
            data[count] = sampleBuffer[position]; //Mathf.Sin(2 * Mathf.PI * frequency * position / sampleRate)
            position++;
            count++;
        }
    }

    void OnAudioSetPosition(int newPosition)
    {
        position = newPosition;
    }
}
