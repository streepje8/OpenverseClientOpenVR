using System;

namespace StreepInput
{
    using System.Linq;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;

    /// <summary>
    /// The script that manages and wraps all input devices
    /// </summary>
    public class StreepInputManager : MonoBehaviour
    {
        private static StreepInputManager instance;

        /// <summary>
        /// The main StreepInputManager instance
        /// </summary>
        public static StreepInputManager Instance
        {
            get
            {
                if (instance == null) instance = new GameObject("Input Manager").AddComponent<StreepInputManager>(); //Create a new input manager in case the user forgot it.
                return instance;
            }
            set
            {
                if (instance == null) {instance = value;}
                else { 
                    Debug.LogWarning("The StreepInputManager Singleton was overwritten, this may happen on scene change. Make sure you only have one StreepInputManager in your scene!");
                    instance = value;
                }
            }
        }

        //The database of the connected input devices
        private Dictionary<InputDevice, SInputDevice> devices = new Dictionary<InputDevice, SInputDevice>();

        private void Awake()
        {
            //Do some setup
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            //Find all currently connected devices
            foreach (var device in InputSystem.devices)
            {
                RegisterDevice(device);
            }
            
            //Add a listener for new connected devices
            InputSystem.onDeviceChange += (device, change) => GetSInputDevice(device).OnDeviceChange(change);
        }

        private void Update()
        {
            foreach (var SInputDevice in devices.Values)
            {
                SInputDevice.Update();
            }
        }

        /// <summary>
        /// Get a list of all connected devices
        /// </summary>
        /// <returns>A list of all connected devices</returns>
        public List<SInputDevice> ConnectedDevices()
        {
            return devices.Values.ToList();
        }
        
        /// <summary>
        /// Get the first input device of a certain type
        /// </summary>
        /// <typeparam name="T">The input device type</typeparam>
        /// <returns>The SInputDevice</returns>
        public SInputDevice GetSInputDevice<T>()
        {
            return devices.Values.First(x => x.Device is T);
        }

        /// <summary>
        /// Get all input devices of a certain type
        /// </summary>
        /// <typeparam name="T">The input device type</typeparam>
        /// <returns>A list of found SInputDevices</returns>
        public List<SInputDevice> GetSInputDevices<T>()
        {
            return devices.Values.Where(x => x.Device is T).ToList();
        }

        /// <summary>
        /// Converts a normal input device to an SInputDevice
        /// </summary>
        /// <param name="device">The device to convert</param>
        /// <returns>The converted device</returns>
        public SInputDevice GetSInputDevice(InputDevice device)
        {
            if (devices.TryGetValue(device, out SInputDevice sdevice))
            {
                return sdevice;
            }
            RegisterDevice(device);
            return devices[device];
        }

        /// <summary>
        /// Registers a device
        /// </summary>
        /// <param name="device">The device to register</param>
        private void RegisterDevice(InputDevice device)
        {
            if (!devices.ContainsKey(device))
            {
                devices.Add(device, new SInputDevice(device));
            }
            else Debug.Log("Tried to register a device that was already registered!");
        }
    }
}