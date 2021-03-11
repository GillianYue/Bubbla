using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// interactable GOs in non-battle situations (sceneType == Mode.TRAVEL)
/// 
/// parent of ivsInteractable 
/// </summary>
public abstract class interactable : MonoBehaviour
{
    public int interactableDist; //default

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //determines if the other GO (usually player) is close enough to this particular ivs interactable
    public bool closeEnough(GameObject other)
    {
        //-1: always interactable; dist<interactableDist = close enough
        float dist = Global.findVectorDist(transform.position, other.transform.position);

        return (dist <= interactableDist);

    }

    public void setInteractableDist(int dist)
    {
        interactableDist = dist;
    }


    public abstract void interact();
}
