namespace Openverse.UI
{
    using UnityEngine;
    
    public enum ControlMethod
    {
        Physical, //Physically touch the button
        Lazer, //The raycast system we all know
        Positional //Sort of mouse that moves based on your controller position
    }

    [CreateAssetMenu(fileName = "NewUISettings", menuName = "Openverse/Settings/UI Settings Profile", order = 100)]    
    public class UISettings : ScriptableObject
    {
        public GameObject UIPanelPrefab;
        public GameObject UIButtonPrefab;
        public ControlMethod CurrentUIMode = ControlMethod.Lazer;
        public string LazerClickButton = "Trigger";
    }
}
