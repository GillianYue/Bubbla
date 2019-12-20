using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interactable : MonoBehaviour
{
    public int interactableDist; //default

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
}
