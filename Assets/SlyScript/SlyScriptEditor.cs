using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
using UnityEditor;
#endif

namespace Sly
{
#if UNITY_EDITOR
    [CustomEditor(typeof(SlyScript))]
    public class SlyScriptEditor : Editor
    {

        public static void reloadAllEdited()
        {
            SlyManager.queueRecompileEdited = true;
        }

        public override void OnInspectorGUI()
        {
            SlyScript script = (SlyScript)target;
            if (GUILayout.Button("Recompile"))
            {
                string filepath = Path.GetFileName(AssetDatabase.GetAssetPath(script));
                LoadFile(filepath, script);
                EditorUtility.SetDirty(script);
                SlyManager.recompileAll();
            }
            GUILayout.Space(20);
            GUILayout.TextArea(script.sourceCode);
        }

        [OnOpenAssetAttribute(1)]
        public static bool step1(int instanceID, int line)
        {
            Object edited = EditorUtility.InstanceIDToObject(instanceID);
            if (edited.GetType() == typeof(SlyScript))
            {
                SlyScript script = (SlyScript)edited;
                string filepath = Path.GetFileName(AssetDatabase.GetAssetPath(script));
                SlyFileTracker.currentlyBeeingEdited.Add(script);
                SlyFileTracker.currentlyBeeingEditedPaths.Add(filepath);
                PrepareFile(filepath, script);
                openSlyFile(filepath);
                EditorUtility.SetDirty(script);
                return true;
            }
            return false;
        }

        public static FileSystemWatcher watcher = null;

        public static void openSlyFile(string filepath)
        {
            string vsPath = EditorPrefs.GetString("kScriptsDefaultApp");
            string slyFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\slyscript\\editCache\\" + filepath.Replace(".asset", "") + ".sly";
            System.Diagnostics.Process.Start("\"" + vsPath + "\"", "\"" + slyFilePath + "\"");
            if (watcher == null)
            {
                watcher = new FileSystemWatcher();
                watcher.Path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\slyscript\\editCache\\";
                watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                   | NotifyFilters.FileName | NotifyFilters.DirectoryName;
                watcher.Filter = "*.sly";
                // Add event handlers.
                watcher.Changed += new FileSystemEventHandler(OnFileChange);
                watcher.EnableRaisingEvents = true;
            }
        }

        private static void OnFileChange(object sender, FileSystemEventArgs e)
        {
            reloadAllEdited();
        }

        public static void PrepareFile(string filepath, SlyScript s)
        {
            string text = s.sourceCode;
            Directory.CreateDirectory(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\slyscript\\editCache\\");
            File.WriteAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\slyscript\\editCache\\" + filepath.Replace(".asset", "") + ".sly", text);
            Debug.Log("Sly has prepaired a file at: " + System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\slyscript\\editCache\\" + filepath.Replace(".asset", "") + ".sly");

        }

        public static void LoadFile(string filepath, SlyScript s)
        {
            string foundtext = File.ReadAllText(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\slyscript\\editCache\\" + filepath.Replace(".asset", "") + ".sly");
            if (foundtext != null)
            {
                s.sourceCode = foundtext;
                Debug.Log("Sly successfully reloaded: " + filepath.Replace(".asset", ""));
            }
            else
            {
                Debug.LogError("Sly failed to load file: " + filepath.Replace(".asset", ""));
            }
        }
    }
#endif
}
