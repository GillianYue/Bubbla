using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Loader : MonoBehaviour
{

    [Inject(InjectFrom.Anywhere)]
    public LoadScene loadScene;

    void Awake()
    {

    }


    protected virtual void Start()
    {
        //print("adding " + name + " to all loaders");
        loadScene.allLoadersToCheck.Add(this);
    }


    void Update()
    {
        
    }

    /// <summary>
    /// should be overridden by inherited classes
    /// 
    /// an abstract modifier will require children to have an override version of this function
    /// </summary>
    /// <returns></returns>
    public abstract bool isLoadDone();
}
