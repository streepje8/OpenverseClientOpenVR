using Openverse.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITester : MonoBehaviour
{
    private UIPanel panel;

    void Start()
    {
        panel = UIManager.Instance.CreateUIPanel();
        UIButton button = UIManager.Instance.CreateButton(panel,null, "Epic Test Button");
        button.onClickEvent += () => { Debug.Log("Noice"); };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
