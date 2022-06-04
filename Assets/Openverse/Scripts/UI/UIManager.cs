using System.Collections.Generic;

namespace Openverse.UI
{
    using Openverse.Core;
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
        
        public UIPanel currentPanel
        {
            get
            {
                if(activePanels.Count > 0)
                    return activePanels.Top();
                return null;
            }
        }

        private SemiStack<UIPanel> activePanels = new SemiStack<UIPanel>();

        private void Awake()
        {
            Instance = this;
        }

        public UIPanel CreateUIPanel()
        {
            UIPanel panel = Instantiate(settings.prefabs.UIPanel).GetComponent<UIPanel>();
            panel.GetComponent<Canvas>().worldCamera = Camera.main;
            if(currentPanel != null)
                currentPanel.panelCurrent = false;
            activePanels.Push(panel);
            currentPanel.panelCurrent = true;
            panel.Open();
            SetGameObjectLayer(panel.gameObject, UILayer);
            return panel;
        }

        public UIButton CreateButton(UIPanel panel, Sprite icon = null, string text = null) => CreateButton(panel, Vector2.zero, icon, text);

        public UIButton CreateButton(UIPanel panel,Vector2 position,Sprite icon = null, string text = null)
        {
            UIButton button = Instantiate(settings.prefabs.UIButton).GetComponent<UIButton>();
            button.Init();
            button.SetPanel(panel);
            button.transform.position = position;
            if(icon != null) button.SetIcon(icon);
            if(text != null) button.SetText(text);
            SetGameObjectLayer(button.gameObject, UILayer);
            return button;
        }

        public UIText CreateText(UIPanel panel, string text = "New Text") => CreateText(panel, Vector2.zero, text);

        public UIText CreateText(UIPanel panel, Vector2 position, string text = "New Text")
        {
            UIText textObj = Instantiate(settings.prefabs.UIText).GetComponent<UIText>();
            textObj.Init();
            textObj.SetPanel(panel);
            textObj.SetText(text);
            textObj.transform.position = position;
            SetGameObjectLayer(textObj.gameObject, UILayer);
            return textObj;
        }

        public void AlertBox(string message)
        {
            UIPanel alert = CreateUIPanel();
            alert.background.texture = settings.backgrounds.defaultBG;
            UIText text = CreateText(alert, Vector2.up * 0.125f, message);
            UIButton button = CreateButton(alert, -Vector2.up * 0.25f, settings.icons.xMark, "Close");
            alert.width = 1f;
            alert.height = 1f;
            text.size *= 0.1f;
            text.width *= 0.95f; //make text less wide
            button.size *= 0.1f;
            
            alert.showBackground = true;
            alert.panelPosition = OpenverseClient.Instance.player.transform.position + OpenverseClient.Instance.player.transform.forward * 2f + (OpenverseClient.Instance.player.head.transform.localPosition.y / 2f * Vector3.up);
            alert.lookAtPlayer = true;
            text.color = new Color(0.03189604f, 0.03189604f, 0.03189604f, 1);
            button.onClickEvent += () => { alert.Close(); };
        }

        public void CloseCurrentPanel()
        {
            CloseUIPanel(activePanels.Pop());
            currentPanel.panelCurrent = true;
        }

        public void CloseUIPanel(UIPanel p, bool callCloseMethod = true)
        {
            if (activePanels.Contains(p))
            {
                activePanels.Remove(p);
            }
            if (callCloseMethod) p.Close();
        }

        public void CloseAllUI()
        {
            foreach (UIPanel panel in activePanels)
            {
                panel.Close();
            }
            activePanels = new SemiStack<UIPanel>();
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

public class SemiStack<T> : LinkedList<T>
{
    public T Pop()
    {
        T first = Top();
        RemoveFirst();
        return first;
    }

    public void Push(T obj)
    {
        AddFirst(obj);
    }
    
    public T Top()
    {
        return First.Value;
    }
}