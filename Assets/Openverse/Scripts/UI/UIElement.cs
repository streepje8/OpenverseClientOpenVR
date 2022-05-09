namespace Openverse.UI
{
    using UnityEngine;

    public class UIElement : MonoBehaviour
    {
        protected UIPanel parentPanel;
        public float width
        {
            set
            {
                RectTransform rect = (RectTransform)transform;
                OnResize(new Vector2(value, rect.sizeDelta.y) / transform.localScale);
            }
        }

        public float height
        {
            set
            {
                RectTransform rect = (RectTransform)transform;
                OnResize(new Vector2(rect.sizeDelta.x, value) / transform.localScale);
            }
        }

        public Vector2 size
        {
            set
            {
                OnResize(value / transform.localScale);
            }
        }

        public virtual void OnResize(Vector2 newSize) {
            RectTransform rect = (RectTransform)transform;
            rect.sizeDelta = newSize;
        }

        public void SetPanel(UIPanel panel)
        {
            parentPanel?.RemoveElement(this);
            parentPanel = panel;
            if (parentPanel == null)
            {
                Destroy(gameObject);
                return;
            }
            transform.parent = panel.transform;
            parentPanel?.AddElement(this);
        }

        public void Destroy() => SetPanel(null);
    }
}
