using Openverse.Permissions;
using Openverse.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITester : MonoBehaviour
{
    private UIPanel panel;

    private void Start()
    {
        
    }

    public void DOThings()
    {
        UIManager.Instance.AlertBox("Something has requested me to show you this epic popup.");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
