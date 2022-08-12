// ----- AUTO GENERATED CODE ----- //
using UnityEngine;
namespace Openverse.Variables
{
    [CreateAssetMenu(fileName = "New Int Variable", menuName = "Openverse/Scriptable Object System/Variable/Int Variable", order = 100)]
    public class IntVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public int Value;
        public void SetValue(int value)
        {
            Value = value;
        }
        public void SetValue(IntVariable value)
        {
            Value = value.Value;
        }
    }
}

