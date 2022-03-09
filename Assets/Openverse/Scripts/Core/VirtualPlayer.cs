using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VirtualPlayer : MonoBehaviour
{
    [SerializeField] public ushort id;
    [SerializeField] public string username;

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
    

    public void Move(Vector3 newPosition, Vector3 forward)
    {
        //transform.position = newPosition;

        //if (id != OpenverseNetworkClient.Instance.Client.Id) // Don't overwrite local player's forward direction to avoid noticeable rotational snapping
            //transform.forward = forward;
    }

    private void OnDestroy()
    {
        OpenversePlayer.list.Remove(id);
    }

}
