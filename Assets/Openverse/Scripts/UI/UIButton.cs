namespace Openverse.UI
{
    using Openverse.Core;
    using Openverse.Events;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIButton : UIElement, ILazerInteractable
    {
        public enum ButtonStatus
        {
            Normal,
            Disabled,
            Selected,
            Pressed
        }
        [HideInInspector] public TextMeshProUGUI txt;
        [HideInInspector] public RawImage background;
        [HideInInspector] public Image icon;
        public ButtonStatus status
        {
            private set
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

        public delegate void onClickHandler();
        [HideInInspector]public event onClickHandler onClickEvent;
        public GameEvent onClick;

        private BoxCollider buttonBox;

        void Start()
        {
            Init();
        }

        private float disablecooldown = 0f;
        private bool canClickPositional = true;
        private void FixedUpdate()
        {
            if(UIManager.Instance.settings.CurrentUIMode == ControlMethod.Physical && buttonBox != null)
            {
                bool isTouching = buttonBox.bounds.Contains(OpenverseClient.Instance.player.handLeft.transform.position) || buttonBox.bounds.Contains(OpenverseClient.Instance.player.handRight.transform.position);
                if (isTouching)
                {
                    status = ButtonStatus.Selected;
                }
                else
                {
                    if (status == ButtonStatus.Selected)
                    {
                        Click();
                        status = ButtonStatus.Pressed;
                    } else
                    {
                        status = ButtonStatus.Normal;
                    }
                }
            }
            if(UIManager.Instance.settings.CurrentUIMode == ControlMethod.Lazer)
            {
                if(disablecooldown >= 0f)
                {
                    disablecooldown -= Time.deltaTime;
                } else
                {
                    status = ButtonStatus.Normal;
                }
            }
            if (UIManager.Instance.settings.CurrentUIMode == ControlMethod.Positional && UIManager.Instance.currentPanel != null && UIManager.Instance.currentPanel.myCursor != null)
            {
                bool isTouching = buttonBox.bounds.Contains(UIManager.Instance.currentPanel.myCursor.transform.position);
                if (isTouching && UIManager.Instance.currentPanel.myCursor.inDrag)
                {
                    status = ButtonStatus.Selected;
                    if (UIManager.Instance.currentPanel.myCursor.currentDragDevice.Get<float>(UIManager.Instance.settings.PositionalClickButton) > 0.5f)
                    {
                        if (canClickPositional)
                        {
                            Click();
                            canClickPositional = false;
                        }
                        status = ButtonStatus.Pressed;
                    } else
                    {
                        canClickPositional = true;
                    }
                }
                else
                {
                    status = ButtonStatus.Normal;
                }
            }
        }

        public void Init()
        {
            txt = GetComponentInChildren<TextMeshProUGUI>();
            background = GetComponent<RawImage>();
            icon = GetComponentInChildren<Image>();
            buttonBox = GetComponent<BoxCollider>();
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
            onClickEvent?.Invoke();
            onClick?.Raise();
        }

        public override Vector2 GetSize()
        {
            return transform.localScale;
        }

        public override void OnResize(Vector2 newSize)
        {
            transform.localScale = new Vector3(newSize.x, newSize.y, (newSize.x + newSize.y) / 2f);
        }

        public void OnLazerHover()
        {
            status = ButtonStatus.Selected;
            disablecooldown = 0.1f;
        }

        public void OnLazerClick()
        {
            Click();
            status = ButtonStatus.Pressed;
        }
    }
}