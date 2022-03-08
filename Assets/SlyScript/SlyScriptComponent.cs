using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sly
{
    [Serializable]
    public class SlyScriptComponent : MonoBehaviour
    {
        public SlyScript Script = null;
        public SlyInstance instance = null;
        public SlyInstance runtimeInstance = null;
        public bool hasCompiled = false;
        private SlyFunction startfunc = null;
        private SlyFunction update = null;

        private void Awake()
        {
            Script.Compile();
            runtimeInstance = new SlyInstance(instance);
            foreach (SlyFunction func in runtimeInstance.functions)
            {
                if (SlyScript.RemoveSpecialCharacters(func.name) == "Update")
                {
                    update = func;
                }
                if (SlyScript.RemoveSpecialCharacters(func.name) == "Start")
                {
                    startfunc = func;
                }
            }
        }

        private void Start()
        {
            startfunc?.Run(runtimeInstance, gameObject, new SlyParameter[0]);
        }

        private void Update()
        {
            update?.Run(runtimeInstance, gameObject, new SlyParameter[0]);
        }

        public void hotReload()
        {
            Script.Compile();
            instance = new SlyInstance(Script.compiledClass);
            runtimeInstance = new SlyInstance(instance);
            foreach (SlyFunction func in runtimeInstance.functions)
            {
                if (SlyScript.RemoveSpecialCharacters(func.name) == "Update")
                {
                    update = func;
                }
                if (SlyScript.RemoveSpecialCharacters(func.name) == "Start")
                {
                    startfunc = func;
                }
            }
            Debug.Log("Hot Reloaded SlyScript");
        }
    }
}
