namespace Openverse.UI
{
    using TMPro;
    using UnityEngine;

    public class UIText : UIElement
    {
        public string text 
        {
            get
            {
                return TMP.text;
            } 
            set
            {
                TMP.text = value;
            }
        }
        
        public TMP_FontAsset font
        {
            get
            {
                return TMP.font;
            }
            set
            {
                TMP.font = value;
            }
        }

        public Color color
        {
            get
            {
                return TMP.faceColor;
            }
            set
            {
                TMP.faceColor = value;
            }
        }

        private TMP_Text TMP;
        private Material mat;

        internal void SetText(string text)
        {
            TMP.text = text;
        }

        internal void Init()
        {
            TMP = GetComponent<TMP_Text>();
            TMP.enabled = false;
            mat = new Material(TMP.material);
            TMP.material = mat;
            TMP.enabled = true;
            TMP.faceColor = Color.white;
        }
    }
}
