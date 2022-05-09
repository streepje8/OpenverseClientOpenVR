namespace Openverse.UI
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "NewUISettings", menuName = "Openverse/Settings/UI Settings Profile", order = 100)]    
    public class UISettings : ScriptableObject
    {
        public GameObject UIPanelPrefab;
        public GameObject UIButtonPrefab;
    }
}
