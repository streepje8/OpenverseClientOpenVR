using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Sly
{
    [Serializable]
    public class SlyInvocation
    {
        public bool isSystemInvocation = false;

        //IF isSystemInvocation == false
        public string executionFunction;

        //IF isSystemInvocation == true
        public string name;
        public SlySystemInvocation invocation;
        public SlyParameter[] Myparameters;

        public SlyInvocation(SlyFunction func, string invocation)
        {
            executionFunction = func.getPath();
            isSystemInvocation = false;
            name = invocation.Substring(0, invocation.IndexOf('('));
            string temp = invocation.Substring(invocation.IndexOf('(') + 1);
            temp = temp.Replace(")", "");
            temp = temp.Replace("\"", "-QUOTE-");
            temp = Regex.Replace(temp, @"[^\w., -]", ""); //Sanitize the string
            temp = temp.Replace("-QUOTE-", "\"");
            string[] foundParameters = { };
            if (temp.Length > 0)
            {
                foundParameters = temp.Split(',');
            }
            name = Regex.Replace(name, @"[^\w\.@-]", ""); //Sanitize the string
            if (func.parameters.Length == foundParameters.Length)
            {
                SlyParameter[] converted = SlyScript.ToParameterArray(foundParameters);
                List<string> expectedParameterNames = new List<string>();
                List<SlyObjectType> expectedParameterTypes = new List<SlyObjectType>();
                foreach (SlyVariable parameter in func.parameters)
                {
                    expectedParameterNames.Add(parameter.name);
                    expectedParameterTypes.Add(parameter.type);
                }
                int i = 0;
                bool success = true;
                foreach (SlyObjectType type in expectedParameterTypes)
                {
                    if ((type != converted[i].type) && (converted[i].type != SlyObjectType.Typereference))
                    {
                        success = false;
                        Debug.LogError("[SLY/ERROR] Type mismatch for paramater " + expectedParameterNames[i] + "! Expected: " + type.ToString().Replace("Type", "") + " but got " + converted[i].type.ToString().Replace("Type", ""));
                    } else
                    {
                        converted[i].name = expectedParameterNames[i];
                    }
                    i++;
                }
                if (success)
                {
                    Myparameters = converted;
                }
            }
        }
        public SlyInvocation(string invocationS)
        {
            //isSystemInvocation = true;
            name = invocationS.Substring(0, invocationS.IndexOf('('));
            string temp = invocationS.Substring(invocationS.IndexOf('(') + 1);
            temp = temp.Replace(")", "");
            temp = temp.Replace("\"", "-QUOTE-");
            temp = Regex.Replace(temp, @"[^\w., -]", ""); //Sanitize the string
            temp = temp.Replace("-QUOTE-", "\"");
            string[] foundParameters = { };
            if (temp.Length > 0)
            {
                foundParameters = temp.Split(',');
            }
            name = Regex.Replace(name, @"[^\w\.@-]", ""); //Sanitize the string
            if(SlySystemInvocations.getSystemInvocations().ContainsKey(name))
            {
                isSystemInvocation = true;
                invocation = SlySystemInvocations.getSystemInvocations()[name];
                if(invocation.expectedParameters.Keys.Count == foundParameters.Length)
                {
                    SlyParameter[] converted = SlyScript.ToParameterArray(foundParameters);
                    List<string> expectedParameterNames = new List<string>(invocation.expectedParameters.Keys);
                    int i = 0;
                    bool success = true;
                    foreach(SlyObjectType type in invocation.expectedParameters.Values)
                    {
                        if((type != converted[i].type) && (converted[i].type != SlyObjectType.Typereference))
                        {
                            success = false;
                            Debug.LogError("[SLY/ERROR] Type mismatch for paramater " + expectedParameterNames[i] + "! Expected: " + type.ToString().Replace("Type", "") + " but got " + converted[i].type.ToString().Replace("Type", ""));
                        }
                        i++;
                    }
                    if(success)
                    {
                        Myparameters = converted;
                    }
                } else
                {
                    Debug.LogError("[SLY/ERROR] " + name + " expects " + invocation.expectedParameters.Keys.Count + " parameters!");
                }
            } else
            {
                Debug.LogError("[SLY/ERROR] " + name + " is not a valid system function and not found in any compiled class");
            }
        }

        public void Run(SlyInstance instance, SlyParameter[] parameters, List<SlyVariable> locals, GameObject runner)
        {
            SlyParameter[] finalparameters = new SlyParameter[Myparameters.Length];
            bool success = true;
            for(int i = 0; i < Myparameters.Length; i++)
            {
                if(Myparameters[i].type != SlyObjectType.Typereference)
                {
                    finalparameters[i] = Myparameters[i];
                } else
                {
                    SlyVariable var = SlyManager.resolver.resolveVarFromParamaterArray(parameters, instance.type.name + "." + Myparameters[i].value);
                    if(var == null) { 
                        var = SlyManager.resolver.resolveVarFromListList(new List<List<SlyVariable>>() { instance.variables, locals }, instance.type.name + "." + Myparameters[i].value);
                    }
                    if (var != null) {
                        if(isSystemInvocation) { 
                            finalparameters[i] = new SlyParameter(var.type, var.name, var.value);
                        } else
                        {
                            finalparameters[i] = new SlyParameter(var.type, Myparameters[i].name, var.value);
                        }
                    } else
                    {
                        Debug.LogError("[Sly/ERROR] Undefined variable: " + Myparameters[i].value);
                        success = false;
                    }
                }
            }
            if(success) { 
                if(isSystemInvocation)
                {
                    invocation.Invoke(runner, finalparameters);
                } else
                {
                    foreach(SlyFunction sf in instance.functions)
                    {
                        if(sf.name.Equals(name))
                        {
                            sf.Run(instance, runner, finalparameters);
                        }
                    }
                }
            }
        }
    }
}