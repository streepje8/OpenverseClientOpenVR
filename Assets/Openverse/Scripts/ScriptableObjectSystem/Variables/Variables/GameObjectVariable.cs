// ----- AUTO GENERATED CODE ----- //
using UnityEngine;
namespace Openverse.Variables
{
    [CreateAssetMenu(fileName = "New GameObject Variable", menuName = "Openverse/Scriptable Object System/Variable/GameObject Variable", order = 100)]
    public class GameObjectVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public GameObject Value;
        public void SetValue(GameObject value)
        {
            Value = value;
        }
        public void SetValue(GameObjectVariable value)
        {
            Value = value.Value;
        }
    }
}

