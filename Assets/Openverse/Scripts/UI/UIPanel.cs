namespace Openverse.UI
{
    using Openverse.Core;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPanel : MonoBehaviour
    {
        public RawImage background;
        [HideInInspector]public UIPanelCursor myCursor;
        [HideInInspector]public List<UIElement> elements = new List<UIElement>();
        [HideInInspector]public bool panelCurrent = false;
        [HideInInspector]public Vector3 panelPosition = Vector3.zero;
        [HideInInspector]public Vector3 panelRotation = Vector3.zero;
        [HideInInspector]public bool lookAtPlayer = false;
        private void Start()
        {
            if (myCursor == null)
            {
                myCursor = GetComponentInChildren<UIPanelCursor>();
                myCursor.parent = this;
            }
        }

        private void Update()
        {
            if(panelCurrent && UIManager.Instance.settings.CurrentUIMode == ControlMethod.Positional)
            {
                myCursor.gameObject.SetActive(true);
            } else
            {
                myCursor.gameObject.SetActive(false);
            }
            transform.localPosition = Vector3.Lerp(transform.position, panelPosition, 10f * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(panelRotation), 10f * Time.deltaTime);
            if(lookAtPlayer)
            {
                Vector3 oldForward = transform.forward;
                transform.forward = -(OpenverseClient.Instance.player.head.transform.position - transform.position).normalized;
                panelRotation = transform.rotation.eulerAngles;
                transform.forward = oldForward; //to keep the smooth animation in
            }
        }

        public bool showBackground
        {
            set
            {
                background.gameObject.SetActive(value);
            }
            get
            {
                return background.gameObject.activeSelf;
            }
        }

        public float width
        {
            set
            {
                RectTransform rect = (RectTransform)transform;
                rect.sizeDelta = new Vector2(value, rect.sizeDelta.y);
                rect = (RectTransform)background.transform;
                rect.sizeDelta = new Vector2(value, rect.sizeDelta.y);
            }
        }

        public float height
        {
            set
            {
                RectTransform rect = (RectTransform)transform;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, value);
                rect = (RectTransform)background.transform;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, value);
            }
        }

        public Vector2 size
        {
            set
            {
                RectTransform rect = (RectTransform)transform;
                rect.sizeDelta = value;
                rect = (RectTransform)background.transform;
                rect.sizeDelta = value;
            }
            get
            {
                return ((RectTransform)transform).sizeDelta;
            }
        }

        internal void Close()
        {
            for (int i = elements.Count - 1; i >= 0; i--) {
                UIElement element = elements[i];
                element.Destroy();
            }
            UIManager.Instance.CloseUIPanel(this, false);
            Destroy(gameObject);
        }

        internal void Open()
        {

        }

        public void AddElement(UIElement element)
        {
            elements.Add(element);
            if(myCursor == null)
            {
                myCursor = GetComponentInChildren<UIPanelCursor>();
                myCursor.parent = this;
            }
            myCursor.transform.SetAsLastSibling();
        }

        public void RemoveElement(UIElement element)
        {
            if (elements.Contains(element))
                elements.Remove(element);
        }
    }
}
