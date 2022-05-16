namespace Openverse.UI
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIPanel : MonoBehaviour
    {
        public RawImage background;
        [HideInInspector]public List<UIElement> elements = new List<UIElement>();

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
        }

        internal void Close()
        {
            for (int i = elements.Count - 1; i >= 0; i--) {
                UIElement element = elements[i];
                element.Destroy();
            }
            Destroy(gameObject);
        }

        internal void Open()
        {

        }

        public void AddElement(UIElement element)
        {
            elements.Add(element);
        }

        public void RemoveElement(UIElement element)
        {
            if (elements.Contains(element))
                elements.Remove(element);
        }
    }
}
