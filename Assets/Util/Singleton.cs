using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            return _instance;
        }
        set
        {
            if (_instance == null)
            {
                _instance = value;
            }
            else
            {
                _instance = value;
                Debug.LogWarning("You can only have one instance of a singleton, ive overwritten the previous singleton instance!");
            }
        }
    }
}
