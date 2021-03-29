using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/**
 * holds static references of terms and words
 */
public class Strings : MonoBehaviour
{
    private static Dictionary<string, string> terms = new Dictionary<string, string>()
    {
        { "Gillian", "the holy god, creator of this game" },
        { "iceLadySong", "Let It Go" },
        { "StarbucksWoman", "Medusa" }
    };

    public static string Get(string key)
    {
        string val;
        if(!terms.TryGetValue(key, out val)) Debug.LogError("terms does not contain "+key);

        return val;
    }
}

