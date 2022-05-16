namespace Openverse.UI
{
    using System.Collections.Generic;
    using UnityEngine;

    public class UIManager : Singleton<UIManager>
    {
        public UISettings settings;
        public LayerMask UILayer;
        public bool isUIOpen
        {
            get
            {
                return activePanels.Count > 0;
            }
            set
            {
                if (value == false)
                {
                    CloseAllUI();
                }
            }
        }
        

        private List<UIPanel> activePanels = new List<UIPanel>();

        private void Awake()
        {
            Instance = this;
        }

        private void FixedUpdate()
        {
            
        }

        public UIPanel CreateUIPanel()
        {
            UIPanel panel = Instantiate(settings.UIPanelPrefab).GetComponent<UIPanel>();
            panel.GetComponent<Canvas>().worldCamera = Camera.main;
            activePanels.Add(panel);
            panel.Open();
            SetGameObjectLayer(panel.gameObject, UILayer);
            return panel;
        }

        public UIButton CreateButton(UIPanel panel, Sprite icon = null, string text = null) => CreateButton(panel, Vector2.zero, icon, text);

        public UIButton CreateButton(UIPanel panel,Vector2 position,Sprite icon = null, string text = null)
        {
            UIButton button = Instantiate(settings.UIButtonPrefab).GetComponent<UIButton>();
            button.Init();
            button.SetPanel(panel);
            button.transform.position = position;
            if(icon != null) button.SetIcon(icon);
            if(text != null) button.SetText(text);
            SetGameObjectLayer(button.gameObject, UILayer);
            return button;
        }

        public void AlertBox(string message)
        {
            UIPanel alert = CreateUIPanel();
            //TODO fix this!
        }

        public void CloseAllUI()
        {
            foreach (UIPanel panel in activePanels)
            {
                panel.Close();
            }
            activePanels = new List<UIPanel>();
        }

        //Helper functions
        private static void SetGameObjectLayer(GameObject obj, LayerMask mask)
        {
            obj.layer = FirstSetLayer(mask);
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                obj.transform.GetChild(i).gameObject.layer = FirstSetLayer(mask);
            }
        }

        private static int FirstSetLayer(LayerMask mask)
        {
            int value = mask.value;
            if (value == 0) return 0;
            for (int l = 1; l < 32; l++)
                if ((value & (1 << l)) != 0) return l;
            return -1;
        }
    }
}