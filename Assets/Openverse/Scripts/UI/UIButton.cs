namespace Openverse.UI
{
    using System;
    using TMPro;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class UIButton : UIElement
    {
        public enum ButtonStatus
        {
            Normal,
            Disabled,
            Selected,
            Pressed
        }
        [HideInInspector] public TextMeshProUGUI txt;
        [HideInInspector] public Image background;
        [HideInInspector] public Image icon;
        public ButtonStatus status
        {
            set
            {
                m_status = value;
                UpdateButton();
            }
            get
            {
                return m_status;
            }
        }
        private ButtonStatus m_status;
        public ColorBlock buttonColors;
        public Color normalTextColor;
        public Color disabledTextColor;
        public Color pressedTextColor;
        public Color selectedTextColor;

        public UnityEvent onClick;

        void Start()
        {
            txt = GetComponentInChildren<TextMeshProUGUI>();
            background = GetComponent<Image>();
            icon = GetComponentInChildren<Image>();
        }

        void UpdateButton()
        {
            switch (status)
            {
                case ButtonStatus.Normal:
                    background.color = buttonColors.normalColor;
                    txt.color = normalTextColor;
                    break;
                case ButtonStatus.Disabled:
                    background.color = buttonColors.disabledColor;
                    txt.color = disabledTextColor;
                    break;
                case ButtonStatus.Pressed:
                    background.color = buttonColors.pressedColor;
                    txt.color = pressedTextColor;
                    break;
                case ButtonStatus.Selected:
                    background.color = buttonColors.highlightedColor;
                    txt.color = selectedTextColor;
                    break;
            }
        }

        public void SetText(string text)
        {
            txt.text = text;
        }

        public void SetIcon(Sprite newIcon)
        {
            icon.sprite = newIcon;
        }

        public void Click()
        {
            onClick.Invoke();
        }

        public override void OnResize(Vector2 newSize)
        {
            base.OnResize(newSize);
            RectTransform backgroundTransform = (RectTransform)background.transform;
            backgroundTransform.sizeDelta = newSize;
            RectTransform iconTransform = (RectTransform)icon.transform;
            iconTransform.sizeDelta = new Vector2(newSize.y, newSize.y);
            iconTransform.position = new Vector3((newSize.x / 2) - 20, iconTransform.position.y, iconTransform.position.z);
            RectTransform textTransform = (RectTransform)txt.transform;
            textTransform.offsetMin = new Vector2(newSize.y + 10, textTransform.offsetMin.y);
        }
    }
}