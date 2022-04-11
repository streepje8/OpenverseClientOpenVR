using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
