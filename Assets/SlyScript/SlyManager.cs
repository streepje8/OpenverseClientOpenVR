using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sly
{
#if UNITY_EDITOR
    [ExecuteAlways]
    [InitializeOnLoad]
    public class SlyManager : EditorWindow
    {

        public static bool queueRecompileEdited = true;
        private static EditorWindow window = null;
        public static SlyResolver resolver = new SlyResolver();

        public static void recompileAll()
        {
            recompileAllExceptSelf(null);
        }

        [InitializeOnLoadMethod]
        static void start()
        {
            if (window == null)
            {
                window = GetWindowWithRect(typeof(SlyManager), new Rect(0, 0, -10, -10));
            }
        }

        void Update()
        {
            Rect r = position;
            r.x = 10000;
            r.y = 10000;
            position = r;
            if (queueRecompileEdited)
            {
                for (int i = 0; i < SlyFileTracker.currentlyBeeingEdited.Count; i++)
                {
                    SlyScript script = SlyFileTracker.currentlyBeeingEdited[i];
                    string filepath = SlyFileTracker.currentlyBeeingEditedPaths[i];
                    SlyScriptEditor.LoadFile(filepath, script);
                    EditorUtility.SetDirty(script);
                }
                SlyManager.recompileAll();
                queueRecompileEdited = false;
            }
        }

        public static void recompileAllExceptSelf(SlyScript self)
        {
            List<Scene> scenes = new List<Scene>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                scenes.Add(SceneManager.GetSceneByBuildIndex(i));
            }
            foreach (Scene scene in scenes)
            {
                GameObject[] rootObjectsInScene = scene.GetRootGameObjects();
                for (int i = 0; i < rootObjectsInScene.Length; i++)
                {
                    SlyScriptComponent[] allComponents = rootObjectsInScene[i].GetComponentsInChildren<SlyScriptComponent>(true);
                    for (int j = 0; j < allComponents.Length; j++)
                    {
                        SlyScriptComponent ssc = allComponents[j];
                        if (ssc.Script != null)
                        {
                            if (ssc.Script != self)
                            {
                                ssc.Script.Compile();
                            }
                            if (ssc.instance == null)
                            {
                                ssc.instance = new SlyInstance(ssc.Script.compiledClass);
                            }
                            ssc.instance.recompile(ssc.Script.compiledClass);
                        }
                    }
                }
            }

        }
    }
#else
    [ExecuteAlways]
    public class SlyManager : MonoBehaviour
    {

        public static bool queueRecompileEdited = true;
        public static SlyResolver resolver = new SlyResolver();

        public static void recompileAll()
        {
            recompileAllExceptSelf(null);
        }

        void Update()
        {
            if (queueRecompileEdited)
            {
                SlyManager.recompileAll();
                queueRecompileEdited = false;
            }
        }

        public static void recompileAllExceptSelf(SlyScript self)
        {
            List<Scene> scenes = new List<Scene>();
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                scenes.Add(SceneManager.GetSceneByBuildIndex(i));
            }
            foreach (Scene scene in scenes)
            {
                GameObject[] rootObjectsInScene = scene.GetRootGameObjects();
                for (int i = 0; i < rootObjectsInScene.Length; i++)
                {
                    SlyScriptComponent[] allComponents = rootObjectsInScene[i].GetComponentsInChildren<SlyScriptComponent>(true);
                    for (int j = 0; j < allComponents.Length; j++)
                    {
                        SlyScriptComponent ssc = allComponents[j];
                        if (ssc.Script != null)
                        {
                            if (ssc.Script != self)
                            {
                                ssc.Script.Compile();
                            }
                            if (ssc.instance == null)
                            {
                                ssc.instance = new SlyInstance(ssc.Script.compiledClass);
                            }
                            ssc.instance.recompile(ssc.Script.compiledClass);
                        }
                    }
                }
            }

        }
    }
#endif
}