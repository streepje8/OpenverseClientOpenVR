using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PermissionState
{
    ASK,
    ALLOW,
    DENY
}

public class PermissionManager : Singleton<PermissionManager>
{
    private void Awake()
    {
        Instance = this;
    }
}
