using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sly
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SlyScriptComponent))]
    public class SlyScriptComponentEditor : Editor
    {
        SerializedProperty m_myScript;

        void OnEnable()
        {
            // Fetch the objects from the MyScript script to display in the inspector
            m_myScript = serializedObject.FindProperty("Script");
        }

        public override void OnInspectorGUI()
        {
            SlyScriptComponent script = (SlyScriptComponent)target;
            if (script.Script == null)
            {
                EditorGUILayout.PropertyField(m_myScript);
                EditorGUILayout.LabelField("Please add a sly script here!");
            }
            else
            {
                EditorGUILayout.PropertyField(m_myScript);
                if (!script.hasCompiled)
                {
                    script.Script.Compile();
                    script.hasCompiled = true;
                }
                if (script.Script.compiledClass.variables == null)
                {
                    script.Script.Compile();
                }
                if (script.instance == null)
                {
                    script.instance = new SlyInstance(script.Script.compiledClass);
                }
                if (script.instance.type.name.Equals("Undefined", StringComparison.OrdinalIgnoreCase))
                {
                    script.instance = null;
                }
                if (Application.isPlaying)
                {
                    if (GUILayout.Button("Hot Reload Script"))
                    {
                        script.hotReload();
                    }
                    drawInspectorFor(script.runtimeInstance);
                }
                else
                {
                    drawInspectorFor(script.instance);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        public void drawInspectorFor(SlyInstance instance)
        {
            if (instance != null)
            {
                if (instance.variables != null)
                {
                    List<SlyVariable> vars = instance.getVariables();
                    for (int i = 0; i < vars.Count; i++)
                    {
                        switch (vars[i].type)
                        {
                            case SlyObjectType.TypeString:
                                instance.setVariable(i, EditorGUILayout.TextField(vars[i].name, vars[i].value));
                                break;
                            case SlyObjectType.Typeint:
                                int ivalue = 0;
                                if (instance.variables[i].value.Length > 0)
                                {
                                    ivalue = int.Parse(vars[i].value);
                                }
                                instance.setVariable(i, EditorGUILayout.IntField(vars[i].name, ivalue).ToString());
                                break;
                            case SlyObjectType.Typefloat:
                                float fvalue = 0;
                                if (instance.variables[i].value.Length > 0)
                                {
                                    fvalue = float.Parse(vars[i].value);
                                }
                                instance.setVariable(i, EditorGUILayout.FloatField(vars[i].name, fvalue).ToString());
                                break;
                            case SlyObjectType.TypeSlyObject:
                                EditorGUILayout.LabelField(vars[i].name);
                                EditorGUILayout.LabelField("Sly objects cant be edited in the inspector (yet), please make it private!");
                                break;
                            case SlyObjectType.TypeUndefined:
                                EditorGUILayout.LabelField(vars[i].name);
                                EditorGUILayout.LabelField("Undefined variable found in script! Please fix and recompile!!!");
                                break;
                        }
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Please fix the compile errors on this script first!!");
                }
            }
            else
            {
                EditorGUILayout.LabelField("Compiling....");
            }
        }
    }
#endif
}