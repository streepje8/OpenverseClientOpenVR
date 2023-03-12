using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Openverse.Input;
using StreepInput;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public struct TestResult
{
    public bool headsetDetected;
    public int controllerDetectionCount;
    public bool triggerDetected;
    public bool headRotationDetected;
    public bool userHeadsetFeedback;
    public int deviceCount;
    public bool hapticsWork;
}

public class HeadsetTester : MonoBehaviour
{
    public OpenverseDevice headset;
    public OpenverseDevice leftController;
    public OpenverseDevice rightController;
    public SInputDevice keyboard;
    public List<OpenverseDevice> allDetectedDevices = new List<OpenverseDevice>();

    private bool testStarted = false;
    
    void Start()
    {
        OpenverseInput.AddDeviceConnectionHandler((device) =>
        {
            if (device.type == OpenverseDeviceType.Headset) headset = device;
            if (device.type == OpenverseDeviceType.LeftController) leftController = device;
            if (device.type == OpenverseDeviceType.RightController) rightController = device;
            allDetectedDevices.Add(device);
        });
        keyboard = StreepInputManager.Instance.GetSInputDevice<Keyboard>();
        Debug.Log("Press Space To Test!");
    }

    private void FixedUpdate()
    {
        OpenverseInput.UpdateDevices(); //Normally you should not do this in the fixed update
    }
    
    void Update()
    {
        if (keyboard.Get<bool>("space") && !testStarted)
        {
            testStarted = true;
            Test().ContinueWith(t => { Debug.Log("Test Complete."); if (!t.IsCompleted) {Debug.LogError("Test failed because of the following exception: "); Debug.LogException(t.Exception);} });
        }
    }

    private async Task Test()
    {
        TestResult result = new TestResult();
        result.headsetDetected = headset != null;
        result.controllerDetectionCount = ((leftController != null)?1:0) + ((rightController != null)?1:0);
        if (result.controllerDetectionCount > 1)
        {
            Debug.Log("Press the trigger on the left controller, if nothing happens press enter...");
            bool gotInput = false;
            while (!gotInput)
            {
                if (leftController.Get<float>("Trigger") > 0.2f)
                {
                    result.triggerDetected = true;
                    gotInput = true;
                }
                if (keyboard.Get<bool>("enter")) gotInput = true;
                await Task.Delay(100);
            }
            Debug.Log("Ok! If you are still holding enter, please release the key.");
            while (keyboard.Get<bool>("enter")) await Task.Delay(100);
            Debug.Log("Im sending a vibration to both controllers, if nothing happens press enter. Otherwise press Q");
            leftController.SendHaptics(new HapticOutput(1f, 0.8f));
            rightController.SendHaptics(new HapticOutput(1f, 0.8f));
            gotInput = false;
            while (!gotInput)
            {
                if (keyboard.Get<bool>("q"))
                {
                    result.hapticsWork = true;
                    gotInput = true;
                }
                if (keyboard.Get<bool>("enter")) gotInput = true;
                await Task.Delay(100);
            }
            Debug.Log("Ok! If you are still holding enter (or q), please release the key(s).");
            while (keyboard.Get<bool>("enter")) await Task.Delay(100);
        }
        else
        {
            Debug.Log("Controller Tests skipped. (No controllers were detected)");
        }

        if (result.headsetDetected)
        {
            Debug.Log("Please put on the headset and turn 180 degrees, if nothing happens press enter...");
            Vector3 initialHeadsetRotation = headset.Get<Quaternion>("centerEyeRotation") * Vector3.forward;
            bool gotInput = false;
            while (!gotInput)
            {
                if (Vector3.Dot(initialHeadsetRotation, headset.Get<Quaternion>("centerEyeRotation") * Vector3.forward) < 0.3f)
                {
                    result.headRotationDetected = true;
                    gotInput = true;
                }
                if (keyboard.Get<bool>("enter")) gotInput = true;
                await Task.Delay(100);
            }
            Debug.Log("Ok! If you are still holding enter, please release the key.");
            while (keyboard.Get<bool>("enter")) await Task.Delay(100);
        }
        else
        {
            Debug.Log("Headset Tests skipped. (No headset was detected)");
        }
        Debug.Log("Does the headset rotation and position work for you? [y/n]");
        bool gotInput2 = false;
        while (!gotInput2)
        {
            if (keyboard.Get<bool>("y"))
            {
                result.userHeadsetFeedback = true;
                gotInput2 = true;
            }
            if (keyboard.Get<bool>("enter") || keyboard.Get<bool>("n")) gotInput2 = true;
            await Task.Delay(100);
        }

        result.deviceCount = allDetectedDevices.Count;
        Debug.Log("Ok! Great! I got all the feedback i need, please send the following json to the person who requested this test ^_^");
        Debug.Log(JsonConvert.SerializeObject(result,Formatting.Indented));
    }
}
