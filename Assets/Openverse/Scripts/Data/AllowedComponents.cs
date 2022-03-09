using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class AllowedComponents : MonoBehaviour
{
    public static HashSet<Type> allowedTypes = new HashSet<Type>()
    {
        typeof(Transform),
        typeof(MeshRenderer),
        typeof(MeshFilter),
        typeof(MeshCollider),
        typeof(BoxCollider),
        typeof(SphereCollider),
        typeof(CapsuleCollider),
        typeof(TerrainCollider),
        typeof(Light),
        typeof(PostProcessVolume),
        typeof(PostProcessLayer),
        typeof(PostProcessProfile),
        typeof(PostProcessEffectSettings),
        typeof(ParticleSystem),
        typeof(Rigidbody)
    };

    public static List<Type> allowedTypesList = new List<Type>()
    {
        typeof(Transform),
        typeof(MeshRenderer),
        typeof(MeshFilter),
        typeof(MeshCollider),
        typeof(BoxCollider),
        typeof(SphereCollider),
        typeof(CapsuleCollider),
        typeof(TerrainCollider),
        typeof(Light),
        typeof(PostProcessVolume),
        typeof(PostProcessLayer),
        typeof(PostProcessProfile),
        typeof(PostProcessEffectSettings),
        typeof(ParticleSystem),
        typeof(Rigidbody)
    };

    public static void ScanAndRemoveInvalidScripts(GameObject go)
    {
        for(int i = 0; i < go.transform.childCount; i++)
        {
            ScanAndRemoveInvalidScripts(go.transform.GetChild(i).gameObject);
        }
        Component[] components = go.GetComponents<Component>();
        for(int i = 0; i < components.Length; i++)
        {
            if(components[i] != null)
            {
                if(!allowedTypes.Contains(components[i].GetType()))
                {
                    Destroy(components[i]);
                    Debug.LogWarning("Removed component of type " + components[i].GetType() + ". This component type is not allowed!");
                }
            }
        }
    }
}
