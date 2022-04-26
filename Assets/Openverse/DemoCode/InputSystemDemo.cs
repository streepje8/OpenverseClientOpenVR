using Openverse.Input;
using UnityEngine;

public class InputSystemDemo : MonoBehaviour
{
    public OpenverseDevice leftController;
    void Awake()
    {
        OpenverseInput.AddDeviceConnectionHandler(onDeviceConnect);
    }

    public void onDeviceConnect(OpenverseDevice device)
    {
        if (device.type == OpenverseDeviceType.LeftController)
            leftController = device;
    }

    void Update()
    {
        //Map names can be found at: https://docs.unity3d.com/Manual/xr_input.html#:~:text=Unity%20Input%20system.-,XR%20input%20mappings,-The%20following%20table
        if (leftController != null) //check if the controller is connected
        {
            Debug.Log("Trigger value: " + leftController.Get<float>("Trigger"));
            Debug.Log("Primary joystick values: " + leftController.Get<Vector2>("primary2DAxis"));

            if (leftController.Get<bool>("primaryButton"))
            {
                Debug.Log("Primary Button Pressed!");
            }
        }
    }
}
