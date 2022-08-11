#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
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