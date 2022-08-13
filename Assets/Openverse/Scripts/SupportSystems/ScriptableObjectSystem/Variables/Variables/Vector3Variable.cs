// ----- AUTO GENERATED CODE ----- //
using UnityEngine;
namespace Openverse.Variables
{
    [CreateAssetMenu(fileName = "New Vector3 Variable", menuName = "Openverse/Scriptable Object System/Variable/Vector3 Variable", order = 100)]
    public class Vector3Variable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public Vector3 Value;
        public void SetValue(Vector3 value)
        {
            Value = value;
        }
        public void SetValue(Vector3Variable value)
        {
            Value = value.Value;
        }
    }
}

