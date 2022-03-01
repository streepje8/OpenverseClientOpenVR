#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ScriptGenerator : EditorWindow
{
    string variableType;

    void OnGUI()
    {
        variableType = EditorGUILayout.TextField("Please Enter the variable type (case sensitive!!!)", variableType);

        if (GUILayout.Button("Generate"))
        {
            OnClickGenerate();
            GUIUtility.ExitGUI();
        }
    }

    void OnClickGenerate()
    {
        variableType = variableType.Trim();

        if (string.IsNullOrEmpty(variableType))
        {
            EditorUtility.DisplayDialog("Unable to generate", "Please specify a valid variable type.", "Close");
            return;
        }

        variableType = Regex.Replace(variableType, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);

        GenerateVariableScript(variableType);

        Close();
    }


    public static void GenerateVariableScript(string variableType)
    {
        string variableTypeCapitalized = variableType.Substring(0, 1).ToUpper() + variableType.Substring(1);
        CreateScript("/Openverse/Scripts/ScriptableObjectSystem/Variables/Variables/" + variableTypeCapitalized + "Variable.cs", GetVariableScript(variableType));
        CreateScript("/Openverse/Scripts/ScriptableObjectSystem/Variables/References/" + variableTypeCapitalized + "Reference.cs", GetVariableReferenceScript(variableType));
        CreateScript("/Openverse/Scripts/ScriptableObjectSystem/Variables/Editor/" + variableTypeCapitalized + "ReferenceDrawer.cs", GetVariableReferenceDrawerScript(variableType));
    }

    private static string GetVariableReferenceDrawerScript(string variableType)
    {
        string variableTypeCapitalized = variableType.Substring(0, 1).ToUpper() + variableType.Substring(1);
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("#if UNITY_EDITOR");
        builder.AppendLine("using UnityEditor;\nusing UnityEngine;\n\nnamespace Openverse.Variables\n{");
        builder.AppendLine("    [CustomPropertyDrawer(typeof(" + variableTypeCapitalized + "Reference))]");
        builder.AppendLine("    public class " + variableTypeCapitalized + "ReferenceDrawer : PropertyDrawer");
        //Long boy but its just a lot of code that doesnt change
        builder.AppendLine("{\n    private readonly string[] popupOptions = { \"Use Constant\", \"Use Variable\" };\n\n    private GUIStyle popupStyle;\n\n    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)\n    {\n        if (popupStyle == null)\n        {\n            popupStyle = new GUIStyle(GUI.skin.GetStyle(\"PaneOptions\"));\n            popupStyle.imagePosition = ImagePosition.ImageOnly;\n        }\n\n        label = EditorGUI.BeginProperty(position, label, property);\n        position = EditorGUI.PrefixLabel(position, label);\n\n        EditorGUI.BeginChangeCheck();\n\n        SerializedProperty useConstant = property.FindPropertyRelative(\"UseConstant\");\n        SerializedProperty constantValue = property.FindPropertyRelative(\"ConstantValue\");\n        SerializedProperty variable = property.FindPropertyRelative(\"Variable\");\n\n        Rect buttonRect = new Rect(position);\n        buttonRect.yMin += popupStyle.margin.top;\n        buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;\n        position.xMin = buttonRect.xMax;\n\n        int indent = EditorGUI.indentLevel;\n        EditorGUI.indentLevel = 0;\n\n        int result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, popupOptions, popupStyle);\n\n        useConstant.boolValue = result == 0;\n\n        EditorGUI.PropertyField(position,\n            useConstant.boolValue ? constantValue : variable,\n            GUIContent.none);\n\n        if (EditorGUI.EndChangeCheck())\n            property.serializedObject.ApplyModifiedProperties();\n\n        EditorGUI.indentLevel = indent;\n        EditorGUI.EndProperty();\n    }\n}\n}");
        builder.AppendLine("#endif");
        return builder.ToString();
    }

    private static string GetVariableReferenceScript(string variableType)
    {
        string variableTypeCapitalized = variableType.Substring(0, 1).ToUpper() + variableType.Substring(1);
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("using System;");
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine("namespace Openverse.Variables");
        builder.AppendLine("{");
        builder.AppendLine("    [Serializable]");
        builder.AppendLine("    public class " + variableTypeCapitalized + "Reference");
        builder.AppendLine("    {");
        builder.AppendLine("        public bool UseConstant = true;");
        builder.AppendLine("        public " + variableType + " ConstantValue;");
        builder.AppendLine("        public " + variableTypeCapitalized + "Variable Variable;");
        builder.AppendLine("        public " + variableTypeCapitalized + "Reference(){}");
        builder.AppendLine("        public " + variableTypeCapitalized + "Reference(" + variableType + " value)");
        builder.AppendLine("        {");
        builder.AppendLine("            UseConstant = true;");
        builder.AppendLine("            ConstantValue = value;");
        builder.AppendLine("        }");
        builder.AppendLine("        public " + variableType + " Value");
        builder.AppendLine("        {");
        builder.AppendLine("            get { return UseConstant ? ConstantValue : Variable.Value; }");
        builder.AppendLine("            set { if (UseConstant) { ConstantValue = value; } else { Variable.Value = value; } }    ");
        builder.AppendLine("        }");
        builder.AppendLine("        public static implicit operator " + variableType + "(" + variableTypeCapitalized + "Reference reference)");
        builder.AppendLine("        {");
        builder.AppendLine("            return reference.Value;");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static string GetVariableScript(string variableType)
    {
        string variableTypeCapitalized = variableType.Substring(0, 1).ToUpper() + variableType.Substring(1);
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("using UnityEngine;");
        builder.AppendLine("namespace Openverse.Variables");
        builder.AppendLine("{");
        builder.AppendLine("    [CreateAssetMenu(fileName = \"New " + variableTypeCapitalized + " Variable\", menuName = \"Openverse/Scriptable Object System/Variable/" + variableTypeCapitalized + " Variable\", order = 100)]");
        builder.AppendLine("    public class " + variableTypeCapitalized + "Variable : ScriptableObject");
        builder.AppendLine("    {\n#if UNITY_EDITOR\n        [Multiline]\n        public string DeveloperDescription = \"\";\n#endif");
        builder.AppendLine("        public " + variableType + " Value;");
        builder.AppendLine("        public void SetValue(" + variableType + " value)");
        builder.AppendLine("        {");
        builder.AppendLine("            Value = value;");
        builder.AppendLine("        }");
        builder.AppendLine("        public void SetValue(" + variableTypeCapitalized + "Variable value)");
        builder.AppendLine("        {");
        builder.AppendLine("            Value = value.Value;");
        builder.AppendLine("        }");
        builder.AppendLine("    }");
        builder.AppendLine("}");
        return builder.ToString();
    }

    private static void CreateScript(string filepath, string content)
    {
        string path = string.Concat(Application.dataPath, Path.DirectorySeparatorChar, filepath);

        try
        {
            using (FileStream stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("// ----- AUTO GENERATED CODE ----- //");
                    builder.AppendLine(content);
                    writer.Write(builder.ToString());
                    writer.Close();
                }
                stream.Close();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);

            // if we have an error, it is certainly that the file is screwed up. Delete to be save
            if (File.Exists(path) == true)
                File.Delete(path);
        }

        AssetDatabase.Refresh();
    }
}
#endif