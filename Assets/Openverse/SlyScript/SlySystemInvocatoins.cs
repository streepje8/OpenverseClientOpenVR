using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
namespace Sly
{
    public class SlySystemInvocations
    {
        private static Dictionary<string, SlySystemInvocation> invocationDictionary = null;

        public static Dictionary<string, SlySystemInvocation> getSystemInvocations()
        {
            if (invocationDictionary == null) {
                invocationDictionary = new Dictionary<string, SlySystemInvocation>();

                //Register system invocations
                Type[] allTypes = Assembly.GetAssembly(typeof(SlySystemInvocation)).GetTypes();

                foreach (var myType in allTypes)
                {
                    // Check if this type is subclass of your base class
                    bool isSubType = myType.IsSubclassOf(typeof(SlySystemInvocation));

                    // If it is sub-type, then print its name in Debug window.
                    if (isSubType)
                    {
                        SlySystemInvocation invok = (SlySystemInvocation)Activator.CreateInstance(myType);
                        invocationDictionary.Add(invok.name, invok);
                    }
                }
            }

            return invocationDictionary;
        }
    }

    [Serializable]
    public abstract class SlySystemInvocation
    {
        public abstract string name
        {
            get;
        }
        public abstract Dictionary<string, SlyObjectType> expectedParameters
        {
            get;
        }
        public abstract void Invoke(GameObject runner, SlyParameter[] parameters);
    }
}