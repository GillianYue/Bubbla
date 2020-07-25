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
        ce = FindObjectOfType<CustomEvents>(); //can not use dependency injection b/c this script could be attached to
        //GO generated mid-way during runtime, which would create null-pointer errors if we're using D.I.
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
