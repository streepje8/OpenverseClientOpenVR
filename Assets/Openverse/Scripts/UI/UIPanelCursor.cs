namespace Openverse.UI
{
    using Openverse.Core;
    using Openverse.Input;
    using UnityEngine;
    using UnityEngine.UI;
    [RequireComponent(typeof(RawImage))]
    public class UIPanelCursor : MonoBehaviour
    {
        [HideInInspector] public bool inDrag = false;
        [HideInInspector] public OpenverseDevice currentDragDevice;
        [HideInInspector]public UIPanel parent;

        private RawImage sprite;
        private OpenverseDevice leftController;
        private OpenverseDevice rightController;
        private GameObject currentDragObject;
        private Vector3 dragStartPosition = Vector3.zero;
        void Start()
        {
            OpenverseInput.AddDeviceConnectionHandler(onDeviceConnect);
            sprite = GetComponent<RawImage>();
            sprite.texture = UIManager.Instance.settings.PositionalMouseTexture;
        }

        public void onDeviceConnect(OpenverseDevice device)
        {
            if (device.type == OpenverseDeviceType.LeftController)
                leftController = device;
            if (device.type == OpenverseDeviceType.RightController)
                rightController = device;
        }

        void Update()
        {
            if(leftController?.Get<float>(UIManager.Instance.settings.PositionalDragButton) > 0.5f)
            {
                StartDrag(leftController);
            } else
            {
                if (rightController?.Get<float>(UIManager.Instance.settings.PositionalDragButton) > 0.5f)
                {
                    StartDrag(rightController);
                }
                else
                {
                    inDrag = false;
                }
            }
            if(inDrag)
            {
                sprite.enabled = true;
                Vector3 difference = currentDragObject.transform.position - dragStartPosition;
                difference = Quaternion.Inverse(currentDragObject.transform.rotation) * difference;
                difference.z = 0;
                difference.x *= parent.size.x;
                difference.y *= parent.size.y;
                transform.localPosition = difference;
            } else
            {
                sprite.enabled = false;
            }
        }

        void StartDrag(OpenverseDevice device)
        {
            //Debug.Log("NOICE");
            currentDragObject = (device.type == OpenverseDeviceType.LeftController) ? OpenverseClient.Instance.player.handLeft : OpenverseClient.Instance.player.handRight;
            currentDragDevice = device;
            if (!inDrag)
            {
                transform.localPosition = Vector3.zero;
                dragStartPosition = currentDragObject.transform.position;
            }
            inDrag = true;
        }
    }
}
