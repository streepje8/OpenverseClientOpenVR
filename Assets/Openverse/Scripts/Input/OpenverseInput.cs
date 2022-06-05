//-------------------------------
//OpenverseInput
//The Openverse Input Manager
//
//Author: streep
//Creation Date: 13-04-2022
//--------------------------------
namespace Openverse.Input
{
    using System.Collections.Generic;
    using UnityEngine.XR;
    
    public static class OpenverseInput
    {
        public static List<OpenverseDevice> connectedDevices = new List<OpenverseDevice>();

        private static readonly Dictionary<InputDevice, OpenverseDevice> deviceDatabase = new Dictionary<InputDevice, OpenverseDevice>();

        public static void RegisterDevice(OpenverseDevice d)
        {
            deviceDatabase.Add(d.GetInputDevice(), d);
            connectedDevices.Add(d);
            foreach(OnDeviceConnected handler in genericHandlers)
            {
                handler.Invoke(d);
            }
            if(specificHandlers.TryGetValue(d.type, out List<OnDeviceConnected> list))
            {
                foreach(OnDeviceConnected handler in list) 
                {
                    handler.Invoke(d);
                }
            }
        }

        private static List<XRNode> collectedDevices = new List<XRNode>()
        {
            XRNode.LeftHand,
            XRNode.RightHand,
            XRNode.CenterEye,
            XRNode.HardwareTracker
        };

        public static void UpdateDevices()
        {
            //Detect New Devices
            foreach (XRNode node in collectedDevices)
            {
                var foundDevices = new List<InputDevice>();
                InputDevices.GetDevicesAtXRNode(node, foundDevices);
                for (int i = 0; i < foundDevices.Count; i++)
                {
                    if (!DeviceIsRegistered(foundDevices[i]))
                    {
                        switch (node)
                        {
                            case XRNode.LeftHand:
                                new OpenverseDevice(foundDevices[i], OpenverseDeviceType.LeftController);
                                break;
                            case XRNode.RightHand:
                                new OpenverseDevice(foundDevices[i], OpenverseDeviceType.RightController);
                                break;
                            case XRNode.CenterEye:
                                new OpenverseDevice(foundDevices[i], OpenverseDeviceType.Headset);
                                break;
                            case XRNode.HardwareTracker:
                                new OpenverseDevice(foundDevices[i], OpenverseDeviceType.HardwareTracker);
                                break;
                        }
                    }
                }
            }
        }

        public static void DeRegisterDevice(OpenverseDevice d)
        {
            if (deviceDatabase.ContainsValue(d))
            {
                List<InputDevice> toRemove = new List<InputDevice>(); ;
                foreach (KeyValuePair<InputDevice, OpenverseDevice> kv in deviceDatabase)
                {
                    if (kv.Value == d)
                    {
                        toRemove.Add(kv.Key);
                    }
                }
                foreach (InputDevice key in toRemove)
                {
                    deviceDatabase.Remove(key);
                }
            }
            if (connectedDevices.Contains(d))
            {
                connectedDevices.Remove(d);
            }
        }

        public delegate void OnDeviceConnected(OpenverseDevice device);
        private static List<OnDeviceConnected> genericHandlers = new List<OnDeviceConnected>();
        private static Dictionary<OpenverseDeviceType, List<OnDeviceConnected>> specificHandlers = new Dictionary<OpenverseDeviceType, List<OnDeviceConnected>>();
        
        public static void AddDeviceConnectionHandler(OnDeviceConnected handler)
        {
            genericHandlers.Add(handler);
        }

        public static void RemoveDeviceConnectionHandler(OnDeviceConnected handler)
        {
            if(genericHandlers.Contains(handler))
                genericHandlers.Remove(handler);
        }

        public static void AddDeviceConnectionHandler(OpenverseDeviceType type, OnDeviceConnected handler)
        {
            if (!specificHandlers.TryGetValue(type, out List<OnDeviceConnected> list))
            {
                List<OnDeviceConnected> connectedList = new List<OnDeviceConnected>();
                connectedList.Add(handler);
                specificHandlers.Add(type, connectedList);
            }
            else
            {
                list.Add(handler);
            }
        }

        public static void RemoveDeviceConnectionHandler(OpenverseDeviceType type, OnDeviceConnected handler)
        {
            if (!specificHandlers.TryGetValue(type, out List<OnDeviceConnected> list))
            {
                specificHandlers.Add(type, new List<OnDeviceConnected>());
            }
            else
            {
                if (list.Contains(handler))
                    list.Remove(handler);
            }
        }

        internal static bool DeviceIsRegistered(InputDevice inputDevice)
        {
            return deviceDatabase.ContainsKey(inputDevice);
        }
    }
}
