using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct OpenverseMicrophone
{
    public string deviceName;
    public bool valid;
    
    public OpenverseMicrophone(string deviceName = null, bool valid = true)
    {
        this.deviceName = deviceName;
        this.valid = valid;
    }
}

public class VirtualMicrophone : MonoBehaviour
{
    public OpenverseMicrophone ActiveMicrophone
    {
        get
        {
            if (activeMic.HasValue)
            {
                return activeMic.Value;
            }
            else
            {
                return new OpenverseMicrophone("NONE", false);
            }
        }
    }
    private List<OpenverseMicrophone> audioDevices = new List<OpenverseMicrophone>();
    private AudioClip recording;
    private OpenverseMicrophone? activeMic = null;
    private void Awake()
    {
        foreach (string mic in Microphone.devices)
        {
            audioDevices.Add(new OpenverseMicrophone(mic));
        }
    }

    public void SetActive(OpenverseMicrophone microphone)
    {
        if (!microphone.valid) throw new Exception("You tried to set an invalid microphone as an active microphone!");
        if(activeMic.HasValue) Microphone.End(activeMic?.deviceName);
        recording = Microphone.Start(microphone.deviceName, true, 2, AudioSettings.outputSampleRate);
    }

    public void SetInActive() => SetActive();
    public void SetActive() //To set nothing as active
    {
        if(activeMic.HasValue) Microphone.End(activeMic?.deviceName);
        activeMic = null;
    }

    public byte[] Sample()
    {
        if (activeMic == null) return Array.Empty<byte>();
        float[] microphoneData = Array.Empty<float>();
        recording.GetData(microphoneData,Microphone.GetPosition(activeMic?.deviceName));
        byte[] output = new byte[microphoneData.Length * 4];
        Buffer.BlockCopy(microphoneData, 0, output, 0, output.Length);
        return output;
    }
}
