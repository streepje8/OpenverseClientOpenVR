namespace Openverse.UI
{
    using UnityEngine;

    [System.Serializable]
    public struct UIPrefabs
    {
        public GameObject UIPanel;
        public GameObject UIButton;
        public GameObject UIText;
    }
    
    [System.Serializable]
    public struct UIIcons
    {
        public Sprite xMark;
    }

    [System.Serializable]
    public struct UIBackgrounds
    {
        public Texture2D defaultBG;
    }

    public enum ControlMethod
    {
        Physical, //Physically touch the button
        Lazer, //The raycast system we all know
        Positional //Sort of mouse that moves based on your controller position
    }

    [CreateAssetMenu(fileName = "NewUISettings", menuName = "Openverse/Settings/UI Settings Profile", order = 100)]    
    public class UISettings : ScriptableObject
    {
        public UIPrefabs prefabs;
        public UIIcons icons;
        public UIBackgrounds backgrounds;
        public ControlMethod CurrentUIMode = ControlMethod.Lazer;
        public string LazerClickButton = "Trigger";
        public string PositionalClickButton = "Trigger";
        public string PositionalDragButton = "Grip";
        public Texture2D PositionalMouseTexture = null;
    }
}
