#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class OpenverseMenuItems : MonoBehaviour
{
    [MenuItem("Openverse/Create Scriptable Object Variable Type")]
    static void GenerateScript()
    {
        ScriptGenerator window = (ScriptGenerator)EditorWindow.GetWindow(typeof(ScriptGenerator));
        window.Show();
    }

}
#endif