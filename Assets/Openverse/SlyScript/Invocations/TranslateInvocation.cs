using Sly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translate : SlySystemInvocation
{
    public override string name => "Translate";

    public override Dictionary<string, SlyObjectType> expectedParameters => new Dictionary<string, SlyObjectType>(){
     {"x",SlyObjectType.Typefloat},
     {"y",SlyObjectType.Typefloat},
     {"z",SlyObjectType.Typefloat}
    };

    public override void Invoke(GameObject runner, SlyParameter[] parameters)
    {
        int error = 0;
        float[] cords = { 0f, 0f, 0f };
        if (parameters.Length == cords.Length)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].type == SlyObjectType.Typefloat)
                {
                    cords[i] = float.Parse(parameters[i].value);
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
            runner.transform.position += new Vector3(cords[0], cords[1], cords[2]);
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
