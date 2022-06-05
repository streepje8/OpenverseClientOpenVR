//-------------------------------
//Permissons
//The main script that handles permissions on the client
//
//Author: streep
//Creation Date: 12-04-2022
//--------------------------------
namespace Openverse.Permissions
{
    using Newtonsoft.Json;
    using Openverse.Core;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;

    public enum PermissionState
    {
        ASK,
        ALLOW,
        DENY
    }

    public enum Permission
    {
        //XR Hardware Specific
        PositionalData, //Positions and rotations of controllers and headset
        PositionalPlus, //Extra trackers, ex: full body tracking
        ControllerInput, //Controller input, ex: trigger, grip, menu, etc.
        EyeTracking, //Eyetracking
        LipTracking, //Lip tracking
        TounghTracking, //Toungh tracking
        CheeckTracking, //Cheeck tracking
        JawTrackingFull, //Lip, toungh and cheeck tracking
        HeadsetCamera, //Camera feed from XR Device
        EnviromentData, //Used for AR, a sort of mesh of the enviroment around the user
        HeadsetInfo, //Information about the headset, ex: manufacturer, model, etc.
        HeadsetBattery, //Headset battery level
        MotorizedHaptics, //Non sexual motorized haptics (such as vibration motors)
        ElectronicHaptics, //Electronic based haptics
        SHaptics, //Sexual haptics
        HapticsFull, //Motorized and electronic haptics
        SHapticsFull, //HapticsFull + Sexual Haptics
        ThermalFeedback, //Thermal feedback

        //Non-XR Hardware Specific (if sensor is present)
        Microphone, //Microphone input
        Camera, //Camera feed from webcam
        GPS, //GPS location accurate to within 60km
        GPSPlus, //Accurate user location
        AirPressure, //Current air pressure
        HeartRate, //Current heart rate
        Humidity, //Current air humidity
        TemperatureData, //Current air temperature
        NFCData, //Read NFC tags
        SensorPlus, //All of the above until AirPressure
        PCInfo, //Information about the PC, ex: OS, CPU, GPU, RAM, etc.

        //Openverse Specific
        ServerRedirect, //Connect the user to a different server

        //User Data
        LocalizationInfo, //Current set language and timezone
        UserIDSimple, //Name, Age,
        UserIDPhone, //Phone number
        UserIDPlus, //Last Name, Adress
        UserIDGender, //User gender
        UserIDBody, //Such as user weight, arm length, skin color etc
        UserIDFull, //All of the above UserID
        BiometricIDFinger, //Fingerprint Data
        BiometricIDIris, //Iris
        BiometricIDBCI, //Brain to computer interface, ex EEG or related
        BiometricIDFull, //All above BiometricID

        //Device Permissions
        FileInfo, //File name, modified, changed, created data
        FileRead, //File contents
        FileWrite, //Write to files
        FileFull, //All of the above
        NetworkInfo, //Network name, IP, MAC, etc.
        NetworkStrength, //Network strength
        NetworkFull, //All of the above
        PrintFull, //Access printers on the network
        Bluetooth, //Allow use of bluetooth
        WebRequest, //Make http requests on the user's behalf
        WriteCalander, //Write activities to a user their calandar
        ReadCalandar, //Read activities from a user their calander
        CalandarFull, //Read and write activities in a user their calander
        WriteAlarm, //Create alarms in the user their clock
        ReadAlarms, //Read a list of a user their alarms
        DeleteAlarm, //Delete a user their alarm
        AlarmFull, //All the above alarm permissions

        //Dangerous permissions
        RunUnsafe, //Run unauthorized code on a user their device
    }

    public class Permissions : MonoBehaviour
    {
        public static Dictionary<Permission, PermissionState> userDefaults { get; private set; } = new Dictionary<Permission, PermissionState>()
    {
        //XR Hardware Specific
        { Permission.PositionalData, PermissionState.ALLOW },
        { Permission.PositionalPlus, PermissionState.ASK },
        { Permission.ControllerInput, PermissionState.ALLOW },
        { Permission.EyeTracking, PermissionState.ASK },
        { Permission.LipTracking, PermissionState.ASK },
        { Permission.TounghTracking, PermissionState.ASK },
        { Permission.CheeckTracking, PermissionState.ASK },
        { Permission.JawTrackingFull, PermissionState.ASK },
        { Permission.HeadsetCamera, PermissionState.ASK },
        { Permission.EnviromentData, PermissionState.ASK },
        { Permission.HeadsetInfo, PermissionState.ASK },
        { Permission.HeadsetBattery, PermissionState.ASK },
        { Permission.MotorizedHaptics, PermissionState.ALLOW },
        { Permission.ElectronicHaptics, PermissionState.ASK },
        { Permission.SHaptics, PermissionState.ASK },
        { Permission.HapticsFull, PermissionState.ASK },
        { Permission.SHapticsFull, PermissionState.ASK },
        { Permission.ThermalFeedback, PermissionState.ASK },

        //Non-XR Hardware Specific (if sensor is present)
        { Permission.Microphone, PermissionState.ASK },
        { Permission.Camera, PermissionState.ASK },
        { Permission.GPS, PermissionState.ASK },
        { Permission.GPSPlus, PermissionState.ASK },
        { Permission.AirPressure, PermissionState.ASK },
        { Permission.HeartRate, PermissionState.ASK },
        { Permission.Humidity, PermissionState.ASK },
        { Permission.TemperatureData, PermissionState.ASK },
        { Permission.NFCData, PermissionState.ASK },
        { Permission.SensorPlus, PermissionState.ASK },
        { Permission.PCInfo, PermissionState.ASK },

        //Openverse Specific
        { Permission.ServerRedirect, PermissionState.ALLOW },

        //User Data
        { Permission.LocalizationInfo, PermissionState.ASK },
        { Permission.UserIDSimple, PermissionState.ASK },
        { Permission.UserIDPhone, PermissionState.ASK },
        { Permission.UserIDPlus, PermissionState.ASK },
        { Permission.UserIDGender, PermissionState.ASK },
        { Permission.UserIDBody, PermissionState.ASK },
        { Permission.UserIDFull, PermissionState.ASK },
        { Permission.BiometricIDFinger, PermissionState.ASK },
        { Permission.BiometricIDIris, PermissionState.ASK },
        { Permission.BiometricIDBCI, PermissionState.ASK },
        { Permission.BiometricIDFull, PermissionState.ASK },

        //Device Permissions
        { Permission.FileInfo, PermissionState.ASK },
        { Permission.FileRead, PermissionState.ASK },
        { Permission.FileWrite, PermissionState.ASK },
        { Permission.FileFull, PermissionState.ASK },
        { Permission.NetworkInfo, PermissionState.ASK },
        { Permission.NetworkStrength, PermissionState.ASK },
        { Permission.NetworkFull, PermissionState.ASK },
        { Permission.PrintFull, PermissionState.ASK },
        { Permission.Bluetooth, PermissionState.ASK },
        { Permission.WebRequest, PermissionState.ASK },
        { Permission.WriteCalander, PermissionState.ASK },
        { Permission.ReadCalandar, PermissionState.ASK },
        { Permission.CalandarFull, PermissionState.ASK },
        { Permission.WriteAlarm, PermissionState.ASK },
        { Permission.ReadAlarms, PermissionState.ASK },
        { Permission.DeleteAlarm, PermissionState.ASK },
        { Permission.AlarmFull, PermissionState.ASK },

        //Dangerous permissions
        { Permission.RunUnsafe, PermissionState.DENY },
    };

        public static Dictionary<Permission, PermissionState> serverPermissions { get; private set; } = new Dictionary<Permission, PermissionState>();

        public static void LoadPermissions()
        {
            string fileLoc = Application.persistentDataPath + "/Openverse/UserData/DefaultPermissions.json";
            if (File.Exists(fileLoc))
            {
                string fileContents = File.ReadAllText(fileLoc);
                userDefaults = JsonConvert.DeserializeObject<Dictionary<Permission, PermissionState>>(fileContents);
            } else
            {
                //Save the default permission settings
                SavePermissions();
            }
        }

        public static void SavePermissions()
        {
            string fileLoc = Application.persistentDataPath + "/Openverse/UserData/DefaultPermissions.json";
            string jsonString = JsonConvert.SerializeObject(userDefaults, Formatting.Indented);
            if (!File.Exists(fileLoc))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Openverse/UserData");
                File.Create(fileLoc).Dispose();
            }
            File.WriteAllText(fileLoc, jsonString);
        }

        public static PermissionState GetDefaultPermission(Permission permission)
        {
            //Not needed but for savety
            if (userDefaults.TryGetValue(permission, out PermissionState state))
            {
                return state;
            }
            return PermissionState.ASK;
        }

        protected static bool SetDefaultPermission(Permission permission, PermissionState state)
        {
            if (userDefaults.ContainsKey(permission))
            {
                userDefaults[permission] = state;
                return true;
            }
            return false;
        }

        public static void LoadServerPermissions(string name)
        {
            string fileLoc = Application.persistentDataPath + "/Openverse/ServerData/" + name + "/Permissions.json";
            if (File.Exists(fileLoc))
            {
                string fileContents = File.ReadAllText(fileLoc);
                serverPermissions = JsonConvert.DeserializeObject<Dictionary<Permission, PermissionState>>(fileContents);
            }
            else
            {
                //Save the default permission settings
                serverPermissions = new Dictionary<Permission, PermissionState>();
                SaveServerPermissions(name);
            }
        }

        public static void SaveServerPermissions(string name)
        {
            string fileLoc = Application.persistentDataPath + "/Openverse/ServerData/" + name + "/Permissions.json";
            string jsonString = JsonConvert.SerializeObject(serverPermissions, Formatting.Indented);
            if (!File.Exists(fileLoc))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Openverse/ServerData/" + name);
                File.Create(fileLoc).Dispose();
            }
            File.WriteAllText(fileLoc, jsonString);
        }

        public static PermissionState GetServerPermission(Permission permission)
        {
            if (serverPermissions.TryGetValue(permission, out PermissionState state))
            {
                return state;
            }
            return GetDefaultPermission(permission);
        }

        protected static bool SetServerPermission(Permission permission, PermissionState state)
        {
            serverPermissions[permission] = state;
            SaveServerPermissions(OpenverseClient.Instance.currentServer);
            return true;
        }

        protected static bool SetServerPermission(string name, Permission permission, PermissionState state)
        {
            LoadServerPermissions(name);
            serverPermissions[permission] = state;
            SaveServerPermissions(name);
            LoadServerPermissions(OpenverseClient.Instance.currentServer);
            return true;
        }
    }
}