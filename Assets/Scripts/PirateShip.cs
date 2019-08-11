using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShip : BossBehavior
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        life = 50;
        attack = 3; //cannonball damage

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
