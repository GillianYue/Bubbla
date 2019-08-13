﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L1 : LevelScript
{
    public PirateShip ps;

    new void Start()
    {
        base.Start();
    }

    public override IEnumerator levelScriptEvent(int code, bool[] done)
    {
        switch (code)
        {
            case 0:
                StartCoroutine(tutorial(done));
                break;
            case 1:
                StartCoroutine(bulletGaugeTutorial(done));
                break;
            case 2:
                pirateShipFireCannon(done);
                break;
            default:
                break;

        }

        yield return new WaitUntil(() => done[0]);
    }

    void pirateShipFireCannon(bool[] done)
    {
        if(ps == null)
        ps = customEvents.findByIdentifier("ps").GetComponent<PirateShip>();
        ps.fireCannonball(done);
    }

    /**
     * enemy id is tutorial
     * pb id is ball
     * 
     * while(enemy exists){
     *  if(pb absorbed) refill;
     * }
     * 
     */
    IEnumerator tutorial(bool[] done)
    {
        GameObject enemy = customEvents.identified.Find(i => i.id.Equals("tutorial")).gameObject;

        while (enemy != null)
        {
            if(customEvents.findByIdentifier("ball") == null)
            {

                int r = Random.Range(100, 250);
                int g = Random.Range(100, 250);
                int b = Random.Range(100, 250);
                string[] prms = makeParamString(r+","+g+","+b, "-220,660,0", "ball", "3", "0");

                customEvents.genPaintball(new bool[1], prms); //genP is a void

                string[] prms2 = makeParamString("ball", "-220", "250", "3");
                customEvents.moveToInSecs(new bool[1], prms2);
            }

            yield return new WaitForSeconds(0.5f);
        }

        GameObject bal;
        if ( (bal = customEvents.findByIdentifier("ball")) != null)
        {
            Destroy(bal);
        }
        done[0] = true;
    }

    IEnumerator bulletGaugeTutorial(bool[] done)
    {
        while (Player.bulletGauge.Count == 0) //wait for player tapping on pb
        {
            yield return new WaitForSeconds(0.2f);
        }
        gameControl.gadgets[1].SetActive(true); //show bullet gauge
        done[0] = true;
    }



}
