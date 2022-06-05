using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Sly
{
    [Serializable]
    public class SlyResolver
    {
        private Dictionary<string, SlyVariable> vars = new Dictionary<string, SlyVariable>();
        private Dictionary<string, SlyFunction> functions = new Dictionary<string, SlyFunction>();
        private List<string> registeredClasses = new List<string>();
        public void Register(SlyClass sclass) {
            if(registeredClasses.Contains(sclass.name))
            {
                //Deregister class
                DeRegister(sclass);
            }
            string classname = sclass.name;
            foreach (SlyVariable var in sclass.variables)
            {
                vars.Add(classname + "." + var.name, var);
            }
            foreach (SlyFunction func in sclass.functions)
            {
                functions.Add(classname + "." + func.name, func);
            }
            registeredClasses.Add(sclass.name);
        }

        public void DeRegister(SlyClass sclass)
        {
            for(int i = 0; i < vars.Keys.Count;) {
                string key = new List<string>(vars.Keys)[i];
                if(key.Split('.')[0].Equals(sclass.name))
                {
                    vars.Remove(key);
                } else
                {
                    i++;
                }
            }
            for (int i = 0; i < functions.Keys.Count;)
            {
                string key = new List<string>(functions.Keys)[i];
                if (key.Split('.')[0].Equals(sclass.name))
                {
                    functions.Remove(key);
                }
                else
                {
                    i++;
                }
            }
            registeredClasses.Remove(sclass.name);
        }

        public SlyVariable resolveVar(string slyVariable)
        {
            slyVariable = Regex.Replace(slyVariable, @"[^\w., -]", "");
            if (vars.ContainsKey(slyVariable))
            {
                return vars[slyVariable];
            }
            return null;
        }

        public SlyFunction resolveFunction(string slyFunction)
        {
            slyFunction = Regex.Replace(slyFunction, @"[^\w., -]", "");
            if (functions.TryGetValue(slyFunction, out SlyFunction func))
            {
                return func;
            }
            return null;
        }

        public void printAllVars()
        {
            foreach(string key in vars.Keys) {
                Debug.Log(vars[key].name);
            }
        }

        public SlyVariable resolveVarFromListList(List<List<SlyVariable>> list, string v)
        {
            v = Regex.Replace(v, @"[^\w., -]", "");
            string varName = v.Split('.')[1];
            foreach(List<SlyVariable> varlist in list)
            {
                foreach(SlyVariable var in varlist)
                {
                    if(var.name.Equals(varName))
                    {
                        return var;
                    }
                }
            }
            return null;
        }

        public SlyVariable resolveVarFromParamaterArray(SlyParameter[] array, string v)
        {
            SlyVariable result = new SlyVariable();
            v = Regex.Replace(v, @"[^\w., -]", "");
            string varName = v.Split('.')[1];
            for(int i = 0; i < array.Length; i++)
            {
                if (array[i].name.Equals(varName))
                {
                    result.name = array[i].name;
                    result.value = array[i].value;
                    result.type = array[i].type;
                    return result;
                }
            }
            return null;
        }
    }
}