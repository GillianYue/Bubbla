using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class identifier : MonoBehaviour
{
    public string id;
    [Inject(InjectFrom.Anywhere)]
    public CustomEvents ce;

    void Start()
    {
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
