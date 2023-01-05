//-------------------------------
//OpenverseDevice
//Defines an Openverse Input Device and interfaces with the unity XR Device
//
//Author: streep
//Creation Date: 13-04-2022
//--------------------------------
namespace Openverse.Input
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.XR;

    public enum OpenverseDeviceType
    {
        LeftController,
        RightController,
        Headset,
        HardwareTracker
    }

    public class OpenverseDevice
    {

        public string name { get; private set; } = "Generic Openverse Device";
        public string manufacturer { get; private set; } = "Generic Device Company";
        public OpenverseDeviceType type { get; private set; } = OpenverseDeviceType.HardwareTracker;
        public HapticCapabilities hapticCapabilities { get; private set; } = new HapticCapabilities();

        private Dictionary<string, List<InputFeatureUsage>> featureUsages = new Dictionary<string, List<InputFeatureUsage>>();
        private InputDevice device;
        public OpenverseDevice(InputDevice device, OpenverseDeviceType type)
        {
            this.device = device;
            this.type = type;
            name = device.name;
            manufacturer = device.manufacturer;
            OpenverseInput.RegisterDevice(this);
            OpenverseInput.connectedDevices.Add(this);
            List<InputFeatureUsage> found = new List<InputFeatureUsage>();
            device.TryGetFeatureUsages(found);
            foreach (InputFeatureUsage usage in found)
            {
                if(!featureUsages.ContainsKey(usage.name.ToLower()))
                    featureUsages.Add(usage.name.ToLower(), new List<InputFeatureUsage>());
                featureUsages[usage.name.ToLower()].Add(usage);
            }
            device.TryGetHapticCapabilities(out HapticCapabilities hapticCapabilities);
            this.hapticCapabilities = hapticCapabilities;
        }

        public void Update()
        {
            if (!device.isValid)
            {
                Destroy();
            }
        }

        public void Destroy()
        {
            OpenverseInput.DeRegisterDevice(this);
        }

        internal InputDevice GetInputDevice()
        {
            return this.device;
        }

        //OUTPUT
        public bool SendHapticBuffer(uint channel, byte[] buffer)
        {
            if (hapticCapabilities.supportsBuffer)
            {
                device.SendHapticBuffer(channel, buffer);
                return true;
            }
            return false;
        }

        public bool SendHaptics(HapticOutput output)
        {
            if(hapticCapabilities.supportsImpulse)
            {
                device.SendHapticImpulse(output.channel,output.amplitude,output.duration);
                return true;
            }
            return false;
        }

        //INPUT        
        //GET FUNCTION
        public T Get<T>(string name)
        {
            switch (typeof(T).Name) //would rather do the type instead of its name but c# doesn't like that (yet)
            {
                case "Single":
                    return (T)(object)GetFloat(name.ToLower());
                case "Boolean":
                    return (T)(object)GetBool(name.ToLower());
                case "UInt32":
                    return (T)(object)GetUInt(name.ToLower());
                case "Vector2":
                    return (T)(object)GetVector2(name.ToLower());
                case "Vector3":
                    return (T)(object)GetVector3(name.ToLower());
                case "Quaternion":
                    return (T)(object)GetQuaternion(name.ToLower());
                case "InputTrackingState":
                    return (T)(object)GetState(name.ToLower());
                case "Hand":
                    return (T)(object)GetHand(name.ToLower());
                case "Bone":
                    return (T)(object)GetBone(name.ToLower());
                case "Eyes":
                    return (T)(object)GetEyes(name.ToLower());
            }
            throw new Exception("You are not allowed to request input of type: " + typeof(T).Name);
        }

        public object Get(Type inputType, string name)
        {
            switch (inputType.Name) //would rather do the type instead of its name but c# doesn't like that (yet)
            {
                case "Single":
                    return GetFloat(name.ToLower());
                case "Boolean":
                    return GetBool(name.ToLower());
                case "UInt32":
                    return GetUInt(name.ToLower());
                case "Vector2":
                    return GetVector2(name.ToLower());
                case "Vector3":
                    return GetVector3(name.ToLower());
                case "Quaternion":
                    return GetQuaternion(name.ToLower());
                case "InputTrackingState":
                    return GetState(name.ToLower());
                case "Hand":
                    return GetHand(name.ToLower());
                case "Bone":
                    return GetBone(name.ToLower());
                case "Eyes":
                    return GetEyes(name.ToLower());
            }
            throw new Exception("You are not allowed to request input of type: " + inputType.Name);
        }

        #region Get Specific Functions
        private bool GetBool(string name)
        {
            if (!featureUsages.ContainsKey(name))
            {
                throw new Exception("The input of name '" + name + "' of type bool could not be found!");
            }
            foreach (InputFeatureUsage usage in featureUsages[name])
            {
                if (usage.type == typeof(bool))
                {
                    device.TryGetFeatureValue(usage.As<bool>(), out bool result);
                    return result;
                }
            }
            throw new Exception("Usage '" + name + "' does not have a return value of bool.");
        }

        private float GetFloat(string name)
        {
            if (!featureUsages.ContainsKey(name))
            {
                throw new Exception("The input of name '" + name + "' of type float could not be found!");
            }
            foreach (InputFeatureUsage usage in featureUsages[name])
            {
                if (usage.type == typeof(float))
                {
                    device.TryGetFeatureValue(usage.As<float>(), out float result);
                    return result;
                }
            }
            throw new Exception("Usage '" + name + "' does not have a return value of float.");
        }

        private uint GetUInt(string name)
        {
            if (!featureUsages.ContainsKey(name))
            {
                throw new Exception("The input of name '" + name + "' of type UInt could not be found!");
            }
            foreach (InputFeatureUsage usage in featureUsages[name])
            {
                if (usage.type == typeof(uint))
                {
                    device.TryGetFeatureValue(usage.As<uint>(), out uint result);
                    return result;
                }
            }
            throw new Exception("Usage '" + name + "' does not have a return value of UInt.");
        }

        private Vector2 GetVector2(string name)
        {
            if (!featureUsages.ContainsKey(name))
            {
                throw new Exception("The input of name '" + name + "' of type Vector2 could not be found!");
            }
            foreach (InputFeatureUsage usage in featureUsages[name])
            {
                if (usage.type == typeof(Vector2))
                {
                    device.TryGetFeatureValue(usage.As<Vector2>(), out Vector2 result);
                    return result;
                }
            }
            throw new Exception("Usage '" + name + "' does not have a return value of Vector2.");
        }

        private Vector3 GetVector3(string name)
        {
            if (!featureUsages.ContainsKey(name))
            {
                throw new Exception("The input of name '" + name + "' of type Vector3 could not be found!");
            }
            foreach (InputFeatureUsage usage in featureUsages[name])
            {
                if (usage.type == typeof(Vector3))
                {
                    device.TryGetFeatureValue(usage.As<Vector3>(), out Vector3 result);
                    return result;
                }
            }
            throw new Exception("Usage '" + name + "' does not have a return value of Vector3.");
        }

        private Quaternion GetQuaternion(string name)
        {
            if (!featureUsages.ContainsKey(name))
            {
                throw new Exception("The input of name '" + name + "' of type Quaternion could not be found!");
            }
            foreach (InputFeatureUsage usage in featureUsages[name])
            {
                if (usage.type == typeof(Quaternion))
                {
                    device.TryGetFeatureValue(usage.As<Quaternion>(), out Quaternion result);
                    return result;
                }
            }
            throw new Exception("Usage '" + name + "' does not have a return value of Quaternion.");
        }

        private InputTrackingState GetState(string name)
        {
            if (!featureUsages.ContainsKey(name))
            {
                throw new Exception("The input of name '" + name + "' of type InputTrackingState could not be found!");
            }
            foreach (InputFeatureUsage usage in featureUsages[name])
            {
                if (usage.type == typeof(InputTrackingState))
                {
                    device.TryGetFeatureValue(usage.As<InputTrackingState>(), out InputTrackingState result);
                    return result;
                }
            }
            throw new Exception("Usage '" + name + "' does not have a return value of InputTrackingState.");
        }

        private Hand GetHand(string name)
        {
            if (!featureUsages.ContainsKey(name))
            {
                throw new Exception("The input of name '" + name + "' of type Hand could not be found!");
            }
            foreach (InputFeatureUsage usage in featureUsages[name])
            {
                if (usage.type == typeof(Hand))
                {
                    device.TryGetFeatureValue(usage.As<Hand>(), out Hand result);
                    return result;
                }
            }
            throw new Exception("Usage '" + name + "' does not have a return value of Hand.");
        }

        private Eyes GetEyes(string name)
        {
            if (!featureUsages.ContainsKey(name))
            {
                throw new Exception("The input of name '" + name + "' of type Eyes could not be found!");
            }
            foreach (InputFeatureUsage usage in featureUsages[name])
            {
                if (usage.type == typeof(Eyes))
                {
                    device.TryGetFeatureValue(usage.As<Eyes>(), out Eyes result);
                    return result;
                }
            }
            throw new Exception("Usage '" + name + "' does not have a return value of Eyes.");
        }

        private Bone GetBone(string name)
        {
            if (!featureUsages.ContainsKey(name))
            {
                throw new Exception("The input of name '" + name + "' of type Bone could not be found!");
            }
            foreach (InputFeatureUsage usage in featureUsages[name])
            {
                if (usage.type == typeof(Bone))
                {
                    device.TryGetFeatureValue(usage.As<Bone>(), out Bone result);
                    return result;
                }
            }
            throw new Exception("Usage '" + name + "' does not have a return value of Bone.");
        }
        #endregion
    }

    public class HapticOutput
    {
        public uint channel { get; private set; }
        public float amplitude { get; private set; }
        public float duration { get; private set; }

        public HapticOutput()
        {
            channel = 0;
            amplitude = 1;
            duration = 1;
        }
        public HapticOutput(float duration)
        {
            channel = 0;
            amplitude = 1;
            this.duration = duration;
        }

        public HapticOutput(float duration, float amplitude)
        {
            channel = 0;
            this.duration = duration;
            this.amplitude = amplitude;
        }

        public HapticOutput(uint channel, float duration, float amplitude)
        {
            this.channel = channel;
            this.amplitude = amplitude;
            this.duration = duration;
        }
    }
}