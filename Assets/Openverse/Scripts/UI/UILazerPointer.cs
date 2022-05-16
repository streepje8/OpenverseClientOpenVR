namespace Openverse.UI
{
    using Openverse.Input;
    using UnityEngine;

    public class UILazerPointer : MonoBehaviour
    {
        public OpenverseDeviceType SelectionDeviceType;
        public Vector3 rotationOffset = Vector3.zero;
        public LayerMask UILayer;

        private OpenverseDevice SelectionDevice;
        private LineRenderer linerenderer;
        private bool clickCooldown = false;
        void Start()
        {
            OpenverseInput.AddDeviceConnectionHandler(onDeviceConnect);
            linerenderer = GetComponent<LineRenderer>();
            linerenderer.enabled = false;
        }

        public void onDeviceConnect(OpenverseDevice device)
        {
            if (device.type == SelectionDeviceType)
                SelectionDevice = device;
        }

        void FixedUpdate()
        {
            if (UIManager.Instance.settings.CurrentUIMode == ControlMethod.Lazer && SelectionDevice != null) //If enabled
            {
                clickCooldown = !(!clickCooldown || !(SelectionDevice.Get<float>(UIManager.Instance.settings.LazerClickButton) > 0.5f));
                if (UIManager.Instance.isUIOpen && !clickCooldown)
                {
                    linerenderer.enabled = true;
                    Vector3 pointTwo = transform.position + (transform.rotation * Quaternion.Euler(rotationOffset) * Vector3.forward * 1000f);
                    if (Physics.Raycast(transform.position, pointTwo.normalized, out RaycastHit hit, 1000f, UILayer))
                    {
                        LazerInteractable element = hit.collider.gameObject.GetComponent<LazerInteractable>();
                        if (element != null)
                        {
                            element.OnLazerHover();
                            if (SelectionDevice.Get<float>(UIManager.Instance.settings.LazerClickButton) > 0.5f)
                            {
                                element.OnLazerClick();
                            }
                        }
                        pointTwo = hit.transform.position;
                    }
                    linerenderer.SetPositions(new Vector3[] { transform.position, pointTwo });
                }
                else
                {
                    linerenderer.enabled = false;
                }
            }
        }
    }
}