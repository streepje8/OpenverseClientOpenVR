//-------------------------------
//VirtualPlayerInput
//Manages input for various VR devices
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.Core
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.XR;
    using UnityEngine.XR.Interaction.Toolkit;

    public class VirtualPlayerInput : MonoBehaviour
    {
        private Dictionary<XRNode, InputDevice> devices = new Dictionary<XRNode, InputDevice>();
        private Dictionary<InputDevice, List<InputFeatureUsage>> features = new Dictionary<InputDevice, List<InputFeatureUsage>>();
        private Dictionary<string, bool> buttons = new Dictionary<string, bool>();
        private List<InputDevice> trackers = new List<InputDevice>();

        public XRController leftController;

        private List<XRNode> collectedDevices = new List<XRNode>()
        {
            XRNode.LeftHand,
            XRNode.RightHand,
        };

        private int lastCount = 0;

        void Update()
        {
            foreach (XRNode node in collectedDevices)
            {
                var foundDevices = new List<InputDevice>();
                InputDevices.GetDevicesAtXRNode(node, foundDevices);
                if (foundDevices.Count == 1)
                {
                    if (!devices.ContainsKey(node))
                    {
                        devices.Add(node, foundDevices[0]);
                        Debug.Log("Found a controller: " + foundDevices[0].name);
                    }
                }
                else if (foundDevices.Count > 1)
                {
                    //devices.Add(node, foundDevices[0]);
                    Debug.LogWarning("Found more than one device of " + node.ToString() + "!");
                }
                else if(foundDevices.Count < 1)
                {
                    if(devices.ContainsKey(node))
                    {
                        devices.Remove(node);
                    }
                }
            }
            if(devices.Count != lastCount)
            {
            features = new Dictionary<InputDevice, List<InputFeatureUsage>>();
                InputDevices.GetDevicesAtXRNode(XRNode.HardwareTracker, trackers);
                foreach (InputDevice device in devices.Values)
                {
                    List<InputFeatureUsage> usages = new List<InputFeatureUsage>();
                    device.TryGetFeatureUsages(usages);
                    Debug.Log(usages.Count);
                    features.Add(device, usages);
                }
            }

            //Get the input
            foreach (KeyValuePair<InputDevice, List<InputFeatureUsage>> keyValuePair in features)
            {
                foreach (InputFeatureUsage usage in keyValuePair.Value)
                {
                    if (usage.type == typeof(bool))
                    {
                        if (!buttons.ContainsKey(usage.name))
                        {
                            buttons.Add(usage.name, false);
                        }
                        keyValuePair.Key.TryGetFeatureValue(usage.As<bool>(), out bool buttonValue);
                        buttons[usage.name] = buttonValue;
                    }
                }
            }
            foreach (KeyValuePair<string, bool> inputset in buttons)
            {
                Debug.Log(inputset.Key + " is equal to " + inputset.Value);
            }
        }
    }
}