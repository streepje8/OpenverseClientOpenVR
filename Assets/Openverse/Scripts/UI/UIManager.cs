namespace Openverse.UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class UIManager : Singleton<UIManager>
    {
        public UISettings settings;

        private List<UIPanel> activePanels = new List<UIPanel>();

        private void Awake()
        {
            Instance = this;
        }

        public UIPanel CreateUIPanel()
        {
            UIPanel panel = Instantiate(settings.UIPanelPrefab).GetComponent<UIPanel>();
            panel.GetComponent<Canvas>().worldCamera = Camera.main;
            activePanels.Add(panel);
            panel.Open();
            return panel;
        }

        public UIButton CreateButton(UIPanel panel, Sprite icon = null, string text = null) => CreateButton(panel, Vector2.zero, icon, text);

        public UIButton CreateButton(UIPanel panel,Vector2 position,Sprite icon = null, string text = null)
        {
            UIButton button = Instantiate(settings.UIButtonPrefab).GetComponent<UIButton>();
            button.SetPanel(panel);
            button.transform.position = position;
            if(icon != null) button.SetIcon(icon);
            if(icon != null) button.SetText(text);
            return button;
        }

        public void AlertBox(string message)
        {
            UIPanel alert = CreateUIPanel();

        }

        public void CloseAllUI()
        {
            foreach (UIPanel panel in activePanels)
            {
                panel.Close();
            }
        }
    }
}