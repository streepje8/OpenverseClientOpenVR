using Sly;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Rotate : SlySystemInvocation
{
    public override string name => "Rotate";

    public override Dictionary<string, SlyObjectType> expectedParameters => new Dictionary<string, SlyObjectType>(){
     {"x",SlyObjectType.Typefloat},
     {"y",SlyObjectType.Typefloat},
     {"z",SlyObjectType.Typefloat}
    };

    public override void Invoke(GameObject runner, SlyParameter[] parameters)
    {
        int error = 0;
        float[] amounts = { 0f, 0f, 0f };
        if (parameters.Length == amounts.Length)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == SlyObjectType.Typefloat)
                {
                    amounts[i] = float.Parse(parameters[i].value);
                }
                else
                {
                    error = 2;
                }
            }
        }
        else
        {
            error = 1;
        }
        if (error == 0)
        {
            runner.transform.Rotate(new Vector3(amounts[0], amounts[1], amounts[2]) * Time.deltaTime);
        }
        else
        {
            switch (error)
            {
                case 1:
                    Debug.LogError("[Sly/ERROR] Translate expects 3 parameters (float, float, float).");
                    break;
                case 2:
                    Debug.LogError("[Sly/ERROR] Translate expects 3 parameters (float, float, float).");
                    break;
            }
        }
    }
}
