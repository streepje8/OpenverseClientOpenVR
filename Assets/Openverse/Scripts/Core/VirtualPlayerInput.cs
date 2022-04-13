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
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(VirtualPlayer))]
    public class VirtualPlayerInput : MonoBehaviour
    {
        public List<OpenverseDeviceType> trackedInputs = new List<OpenverseDeviceType>();
        
        private VirtualPlayer player;
        private OpenverseDevice leftController;
        private OpenverseDevice rightController;
        private OpenverseDevice headset;
        private List<OpenverseDevice> hardwareTrackers;

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
        }

        public void controllerConnectR(OpenverseDevice d)
        {
            rightController = d;
        }

        public void headsetConnect(OpenverseDevice d)
        {
            headset = d;
        }

        public void trackerConnect(OpenverseDevice d)
        {
            hardwareTrackers.Add(d);
        }

        private void Update()
        {
            Debug.Log(leftController.Get<float>("Trigger"));
        }
    } 
}