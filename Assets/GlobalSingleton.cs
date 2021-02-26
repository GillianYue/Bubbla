using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// singleton script that will be instantiated on awake and will persist throughout scene transitions
/// 
/// can be used to store data that should not be destroyed during the entire game session
/// </summary>
public class GlobalSingleton : MonoBehaviour
{
    public static GlobalSingleton Instance;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}