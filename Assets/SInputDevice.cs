// To prevent the script from being marked as slow purely because it contains Debug.LogError calls
// ReSharper disable Unity.PerformanceCriticalCodeInvocation

namespace StreepInput
{
    using UnityEngine.InputSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.InputSystem.Layouts;

    /// <summary>
    /// The wrapper for the input device class
    /// </summary>    
    public class SInputDevice
    {
        
        /// <summary>
        /// The wrapped device
        /// </summary>
        public InputDevice Device { get; private set; }
        public InputDeviceDescription Description {  get => Device.description; }
        public bool IsConnected { get; private set; } = true;
        public bool Enabled { get; private set; } = true;
        public bool IsDestroyed { get; private set; } = false;
        
        /// <summary>
        /// Gets called when a device reconnects
        /// </summary>
        public Action onReconnect;
        /// <summary>
        /// Gets called when a device disconnects
        /// </summary>
        public Action onDisconnect;
        /// <summary>
        /// Gets called when a device is enabled
        /// </summary>
        public Action onEnable;
        /// <summary>
        /// Gets called when a device is disabled
        /// </summary>
        public Action onDisable;
        /// <summary>
        /// Gets called when a device is destroyed
        /// </summary>
        public Action onDestroy;

        /// <summary>
        /// Generic Input Listeners
        /// </summary>
        private List<InputListener> genericListeners = new List<InputListener>();
        
        /// <summary>
        /// Input Listener function template
        /// </summary>
        public delegate void InputListener(string name, object value);
        
        public SInputDevice(InputDevice device)
        {
            Device = device;
        }

        /// <summary>
        /// Add a generic input listener.
        /// </summary>
        /// <param name="il">The input listener you want to add</param>
        [Obsolete("Using generic listeners is slow, only use this for key rebinding. When you are on a keyboard, its highly recommended to read the 'anykey' input instead!")]
        public void AddGenericListener(InputListener il)
        {
            genericListeners.Add(il);
        }

        /// <summary>
        /// Remove a generic input listener
        /// </summary>
        /// <param name="il">The input listener you want to remove</param>
        public void RemoveGenericListener(InputListener il)
        {
            if (genericListeners.Contains(il)) genericListeners.Remove(il);
        }

        /// <summary>
        /// A cache to prevent having to query all controls every time an input is requested
        /// </summary>
        private Dictionary<string, InputControl> cache = new Dictionary<string, InputControl>();

        /// <summary>
        /// Gets an input from the input device
        /// </summary>
        /// <param name="query">The input you would like to read from this device</param>
        /// <typeparam name="T">The type of value you want to read from this device</typeparam>
        /// <returns>The value of the specified control for this input device</returns>
        public T Get<T>(string query)
        {
            InputControl control = null;
            if (cache.TryGetValue(query.ToLower(), out InputControl value))
            {
                control = value;
            }
            else
            {
                List<InputControl> results = new List<InputControl>();
                foreach (var childControl in Device.children)
                {
                    if (childControl.path.ToLower().Contains(query.ToLower()))
                    {
                        results.Add(childControl);
                    }
                }
                if (results.Count > 0)
                {
                    if (results.Count > 1)
                    {
                        //More accurate searching is required, for example 'space' and 'backspace' both get returned when you try to get space
                        List<InputControl> moreNarrowResults = new List<InputControl>();
                        foreach (var result in results)
                        {
                            if(result.path.Split('/').Last().Equals(query, StringComparison.OrdinalIgnoreCase)) moreNarrowResults.Add(result);
                        }
                        if (moreNarrowResults.Count > 1 || moreNarrowResults.Count < 1)
                        {
                            string warning =
                                "Input query returned multiple results, using the first result! Results: {\n";
                            results.ForEach(x => warning += "\"" + x.path + "\",\n");
                            warning = warning.Substring(0, warning.Length - 2) + "\n}";
                            Debug.LogWarning(warning);
                            cache.Add(query.ToLower(), results[0]);
                            control = results[0];
                        }
                        else
                        {
                            cache.Add(query.ToLower(), moreNarrowResults[0]);
                            control = moreNarrowResults[0];
                        }
                    }
                    else {
                        cache.Add(query.ToLower(),results[0]);
                        control = results[0];
                    }
                } else Debug.LogError("The input query \"" + query +"\" on device \"" + Device.name + "\" returned no results.");
            }
            if (control == null) return default;
            object inputValue = control.ReadValueAsObject();
            if (inputValue is T) return (T)inputValue;
            try
            {
                //Try to force cast
                return (T)Convert.ChangeType(inputValue, typeof(T));
            }
            catch (InvalidCastException e)
            {
                _ = e;
                Debug.LogWarning("The type input you requested was invalid. You requested " + typeof(T).Name + " but the input gave " + inputValue.GetType().Name + ".");
            }
            return default;
        }

        /// <summary>
        /// You should not call this function, its used by the StreepInputManager class to propagate input system changes to input devices
        /// </summary>
        /// <param name="change">The change that occured</param>
        public void OnDeviceChange(InputDeviceChange change)
        {
            switch (change)
            {
                case InputDeviceChange.Removed:
                    IsDestroyed = true;
                    onDestroy?.Invoke();
                    break;
                case InputDeviceChange.Disconnected:
                    IsConnected = false;
                    onDisconnect?.Invoke();
                    break;
                case InputDeviceChange.Reconnected:
                    IsConnected = true;
                    onReconnect?.Invoke();
                    break;
                case InputDeviceChange.Enabled:
                    Enabled = true;
                    onEnable?.Invoke();
                    break;
                case InputDeviceChange.Disabled:
                    Enabled = false;
                    onDisable?.Invoke();
                    break;
            }
        }

        /// <summary>
        /// You should not call this function, its called by the StreepInputManager.
        /// </summary>
        public void Update()
        {
            if (genericListeners.Count > 0)
            {
                foreach (var control in Device.allControls)
                {
                    if (!control.CheckStateIsAtDefault())
                    {
                        genericListeners.ForEach(x => x.Invoke(control.path,control.ReadValueAsObject()));
                    }
                }
            }
        }
    }
}