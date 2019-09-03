using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L1 : LevelScript
{
    public PirateShip ps;
    public Player player;

    new void Start()
    {
        base.Start();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    /*
     * level 1 specific gameOver function; overrides base
     */
    public override void gameOver(GameObject GameOverC)
    {
        StartCoroutine(GameOver());
    }

    IEnumerator GameOver()
    {

        //fade in custom event
        player.respawn();

        gameControl.endGame(); //stop spawners and clears GOs on field

        bool[] vfxDone = new bool[1];
        string[] vfxPrms = makeParamString("0", "whatev");
        StartCoroutine(customEvents.vfx(vfxDone, vfxPrms));

        yield return new WaitUntil(() => vfxDone[0]); //will wait until vfx done
        yield return new WaitForSeconds(2);

        string[] prms2 = makeParamString("ps", "0"); //setting ship inactive
        StartCoroutine(customEvents.setGOActive(new bool[1], prms2));

        //clear scene, revert back to beginning bg

        bool[] vfxDone2 = new bool[1];
        string[] vfxPrms2 = makeParamString("1", "whatev");
        StartCoroutine(customEvents.vfx(vfxDone2, vfxPrms2));
        yield return new WaitUntil(() => vfxDone2[0]); //will wait until vfx done

        gameFlow.setPointerToSpecial(0); //SPECIAL 0
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
            case 3:
                StartCoroutine(bossFight());
                break;
            default:
                break;

        }

        yield return new WaitUntil(() => done[0]);
    }

    IEnumerator bossFight()
    {
        if (ps == null)
            ps = customEvents.findByIdentifier("ps").GetComponent<PirateShip>();

        Debug.Log("starting boss fight");
        bool[] bossFightEnd = new bool[1];

       gameControl.pSpawner.StartSpawn(bossFightEnd); //will end when boss fight ends

        //TODO enter BGM
        yield return new WaitForSeconds(3f);

        StartCoroutine(ps.bossFight(bossFightEnd));
        yield return new WaitUntil(() => bossFightEnd[0]);

        Debug.Log("boss fight done");

    }

    void pirateShipFireCannon(bool[] done)
    {
        if (ps == null)
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
