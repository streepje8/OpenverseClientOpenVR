using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sly
{
    [Serializable]
    public class SlyFunction
    {
        public string className = "undefined";
        public string name = "undefined";
        public SlyObjectType returntype = SlyObjectType.TypeUndefined;

        public List<SlyVariable> locals = new List<SlyVariable>();
        public SlyVariable[] parameters;
        public List<SlyInvocation> invocations = new List<SlyInvocation>();


        public SlyFunction(string className)
        {
            this.className = className;
        }

        public void Run(SlyInstance instance, GameObject runner, SlyParameter[] parametervalues)
        {
            if(invocations != null) { 
                foreach(SlyInvocation sI in invocations)
                {
                    sI.Run(instance, parametervalues, locals, runner);
                }
            }
        }

        public string getPath()
        {
            return className + "." + name;
        }
    }
}
