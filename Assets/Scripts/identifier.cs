using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class identifier : MonoBehaviour
{
    public string id;
    CustomEvents ce;

    void Start()
    {
        ce = GameObject.FindWithTag("CustomEvent").GetComponent<CustomEvents>();
    }

    public void setID(string ID)
    {
        id = ID;
    }

    void OnDestroy()
    {
        ce.removeFromIdentified(this);
    }
}
