using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L1 : LevelScript
{


    //override
    public override void customEvent(int index)
    {
        switch (index)
        {
            case 1:
                StartCoroutine(tutorialEnemy());
                break;

            default:
                break;

        }

    }

    private IEnumerator tutorialEnemy()
    {
        GameObject e = eSpawner.genMonster(0, eSpawner.spawnValues.y, eSpawner.spawnValues.z, 0);
        e.GetComponent<EnemyMover>().enabled = false;

        bool[] done = new bool[1];
        done[0] = false;
        StartCoroutine(EnemyMover.moveTo(e, 0, 400, 50f, done));
        yield return new WaitUntil(() => done[0]);

        yield return new WaitForSeconds(2);

        done[0] = false;
        StartCoroutine(EnemyMover.moveToInSecs(e, 0, 200, 2, done));
        yield return new WaitUntil(() => done[0]);

        yield return new WaitForSeconds(2);

        moveOn(); //all set and ready to continue
    }

    void moveOn()
    {
        gameFlow.incrementPointer();
        //move on to the next thing
        //no need to call processCurrentLine in gFlow because GameControl is constantly 
        //watching over the pointer
    }

}
