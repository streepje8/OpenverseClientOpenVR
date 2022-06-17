//-------------------------------
//VirtualPlayerInput
//Bridge between OpenverseInput and the OpenverseNetcode
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.Core
{
    using Openverse.Input;
    using Openverse.Permissions;
    using RiptideNetworking;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using static Openverse.NetCode.NetworkingCommunications;

    [RequireComponent(typeof(VirtualPlayer))]
    public class VirtualPlayerInput : MonoBehaviour
    {
        public List<OpenverseDeviceType> trackedInputs = new List<OpenverseDeviceType>();
        
        private VirtualPlayer player;
        private OpenverseDevice leftController;
        private OpenverseDevice rightController;
        private OpenverseDevice headset;
        private List<OpenverseDevice> hardwareTrackers;

        private bool leftConnected = false;
        private bool rightConnected = false;
        private bool headsetConnected = false;
        private int trackerCount = 0;

        private void Awake()
        {
            player = GetComponent<VirtualPlayer>();
            if(trackedInputs.Contains(OpenverseDeviceType.LeftController))
                OpenverseInput.AddDeviceConnectionHandler(OpenverseDeviceType.LeftController, controllerConnectL);
            if (trackedInputs.Contains(OpenverseDeviceType.RightController))
                OpenverseInput.AddDeviceConnectionHandler(OpenverseDeviceType.RightController, controllerConnectR);
            if (trackedInputs.Contains(OpenverseDeviceType.Headset))
                OpenverseInput.AddDeviceConnectionHandler(OpenverseDeviceType.Headset, headsetConnect);
            if (trackedInputs.Contains(OpenverseDeviceType.HardwareTracker))
                OpenverseInput.AddDeviceConnectionHandler(OpenverseDeviceType.HardwareTracker, trackerConnect);
        }

        public void controllerConnectL(OpenverseDevice d)
        {
            leftController = d;
            leftConnected = true;
        }

        public void controllerConnectR(OpenverseDevice d)
        {
            rightController = d;
            rightConnected = true;
        }

        public void headsetConnect(OpenverseDevice d)
        {
            headset = d;
            headsetConnected = true;
        }

        public void trackerConnect(OpenverseDevice d)
        {
            hardwareTrackers.Add(d);
            trackerCount++;
        }

        private Dictionary<short, Type> idToType = new Dictionary<short, Type>() {
            {0, typeof(bool)},
            {1, typeof(int)},
            {2, typeof(float)},
            {3, typeof(Vector2)},
            {4, typeof(Quaternion)}
        };

        internal void OnRequestInput(Message message)
        {
            short type = message.GetShort();
            string namedevice = message.GetString();
            string device = namedevice.Split(':')[0];
            string name = namedevice.Split(':')[1];
            Type inputType = idToType[type];
            Message inputSupply = Message.Create(MessageSendMode.unreliable, ClientToServerId.supplyInput);
            inputSupply.Add(type);
            inputSupply.Add(namedevice);
            object value = null;
            if(device.Equals("Left", StringComparison.OrdinalIgnoreCase))
            {
                if (!(PermissionManager.Instance.GetPermission(Permission.ControllerInput) == PermissionState.ALLOW))
                    return;
                if (!leftConnected)
                    return;
                value = leftController.Get(inputType, name);
            }
            if (device.Equals("Right", StringComparison.OrdinalIgnoreCase))
            {
                if (!(PermissionManager.Instance.GetPermission(Permission.ControllerInput) == PermissionState.ALLOW))
                    return;
                if (!rightConnected)
                    return;
                value = rightController.Get(inputType, name);
            }
            if (device.Equals("Head", StringComparison.OrdinalIgnoreCase))
            {
                if (!(PermissionManager.Instance.GetPermission(Permission.HeadsetBattery) == PermissionState.ALLOW))
                    return;
                if (!headsetConnected)
                    return;
                value = headset.Get(inputType, name);
            }
            
            //Tracker support might be coming in the future
            /*
            if (device.Equals("Tracker", StringComparison.OrdinalIgnoreCase))
            {
                value = hardwareTrackers[0].Get(inputType, name);
            }
            */
            switch (type)
            {
                case 0:
                    inputSupply.Add((bool)value);
                    break;
                case 1:
                    inputSupply.Add((int)value);
                    break;
                case 2:
                    inputSupply.Add((float)value);
                    break;
                case 3:
                    inputSupply.Add((Vector2)value);
                    break;
                case 4:
                    inputSupply.Add((Quaternion)value);
                    break;
                default:
                    inputSupply.Add(false);
                    break;
            }
            OpenverseNetworkClient.Instance.Client.Send(inputSupply);
        }
    } 
}