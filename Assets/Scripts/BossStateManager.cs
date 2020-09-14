﻿using System.Collections;
using UnityEngine;

public class BossStateManager : BossBehavior
{

    //from parent: bossMode currMode
    //use setMode(bossMode m, bool[] done)

    public float dirAttkDistrib, shootAttkDistrib;
    public int idleHoverRounds; //rounds of hovering between attacks
    public float idleHoverRoundsNoise; //percentage
    IEnumerator stateManager; 

    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();
    }

    public void startActivities()
    {
        stateManager = stateManage();
        StartCoroutine(stateManager);
    }

    IEnumerator stateManage()
    {

        while (true)
        {
            Debug.Log("idle");
            bool[] idleDone = new bool[1];
            setModeAndStart(currMode, idleDone);
            for (int rounds = 0; rounds < Global.getValueWithNoise(idleHoverRounds, idleHoverRoundsNoise); rounds++)
            {
                yield return new WaitUntil(() => (!idleDone[0]));
                Debug.Log("idle round " + rounds);
                yield return new WaitUntil(() => (idleDone[0])); //if and only if this round's status changed from "not there" to "there"
            }

            Debug.Log("attack");
            bool[] attackDone = new bool[1];
            setModeAndStart(Global.percentChance(dirAttkDistrib) ? bossMode.DIR_ATTK : bossMode.SHOOT_ATTK, attackDone);
            yield return new WaitUntil(() => attackDone[0]);
            Debug.Log("attack done");

        }
    }



}
