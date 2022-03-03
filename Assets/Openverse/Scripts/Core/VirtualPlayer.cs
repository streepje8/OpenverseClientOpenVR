using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualPlayer : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnWorldStart()
    {
        GameObject spawn = GameObject.Find("WorldSpawn");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
        }
    }
}
