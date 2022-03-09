using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VirtualPlayerInput : MonoBehaviour
{
    private Dictionary<XRNode, InputDevice> devices = new Dictionary<XRNode,InputDevice>();
    private Dictionary<InputDevice, List<InputFeatureUsage>> features = new Dictionary<InputDevice, List<InputFeatureUsage>>();
    private Dictionary<string, bool> buttons = new Dictionary<string, bool>();
    private List<InputDevice> trackers = new List<InputDevice>();

    private List<XRNode> collectedDevices = new List<XRNode>()
    {
        XRNode.LeftHand,
        XRNode.RightHand,
    };

    void Start()
    {
        foreach (XRNode node in collectedDevices)
        {
            var foundDevices = new List<InputDevice>();
            InputDevices.GetDevicesAtXRNode(node, foundDevices);
            if (foundDevices.Count == 1)
            {
                devices.Add(node,foundDevices[0]);
                Debug.Log("Found a controller: " + foundDevices[0].name);
            }
            else if (foundDevices.Count > 1)
            {
                devices.Add(node, foundDevices[0]);
                Debug.LogWarning("Found more than one device of " + node.ToString() + "!");
            }
        }
        InputDevices.GetDevicesAtXRNode(XRNode.HardwareTracker, trackers);
        foreach(InputDevice device in devices.Values)
        {
            List<InputFeatureUsage> usages = new List<InputFeatureUsage>();
            device.TryGetFeatureUsages(usages);
            features.Add(device, usages);
        }
    }

    void Update()
    {
        foreach(KeyValuePair<InputDevice,List<InputFeatureUsage>> keyValuePair in features)
        {
            foreach(InputFeatureUsage usage in keyValuePair.Value)
            {
                if (usage.type == typeof(bool))
                {
                    if (buttons.ContainsKey(usage.name))
                    {
                        buttons.Remove(usage.name);
                    }
                    bool buttonValue = false;
                    keyValuePair.Key.TryGetFeatureValue(usage.As<bool>(),out buttonValue);
                    buttons.Add(usage.name, buttonValue);
                }
            }
        }
        foreach(string key in buttons.Keys)
        {
            Debug.Log(key + " is equal to " + buttons[key]);
        }
    }
}
