using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * abstract base class for levelscripts, which contain custom functions for special events in each level
 */
public abstract class BossBehavior : MonoBehaviour
{
    [Inject(InjectFrom.Anywhere)]
    public Player player;
    [Inject(InjectFrom.Anywhere)]
    public BossVisual bossVisual;

    public int life = 1000, maxLife = 1000;
    public int attack;
    public int stage; //0

    public float sizeScale; //same as paintball, base scale to be multiplied with global
    public float colliderScale; //multiplied to the original generated polygon 2d collider shape to resize

    protected GameControl gameControl;
    protected GameFlow gameFlow;
    protected CustomEvents customEvents;

    public GameObject lifeBar; //the one with image attached
    public GameObject lifeContainer, //parent of lifeBar
    moveBody; //moveBody is GO of which the position will be updated (moved)
    protected RectTransform lifeRT;
    public bool lifeBarActive;

    public enum bossMode { IDLE, DAMAGE, DIR_ATTK, SHOOT_ATTK, PROJ_ATTK, SPEC_ATTK, ANTICIP, DEFEATED };
    public bossMode currMode = bossMode.IDLE;
    /// <summary>
    /// the second float in the tuple is erraticity of that attribute which ranges from 0 to 1
    /// to expose in the editor, is typed Vector2 instead of (float, float)
    /// 
    /// TODO: noise - there are different types of noise. Can pick random location and random "delta" target value periodically, i.e. every moment there's a secondary layer
    /// goal, such as +3 in 72 degree
    /// </summary>
    public Vector2 movementSpd, movementRange, hoverDuration, secondLayerNoise;
    /// <summary>
    /// hoverBounds defines the borders outside of which boss will not hover to
    /// The way it works: say we have upper (x:200, y:300) and lower (x:100, y: 50), it would mean that the boss's activity is limited within
    /// +300 to -50 vertically, and +200 and -100 horizontally
    /// </summary>
    public Vector2 hoverBoundsUpper, hoverBoundsLower;

    public Enemy.BuffMode debuff = Enemy.BuffMode.none; //uses the same system as a normal enemy
    IEnumerator currBuffProcess;

    [Inject(InjectFrom.Anywhere)]
    public PrefabHolder prefabHolder;

    public void Start()
    {

        setMode(bossMode.IDLE);
        customEvents = gameObject.GetComponent<CustomEvents>();
        GameObject gameController = GameObject.FindWithTag("GameController");
        gameControl = gameController.GetComponent<GameControl>();
        gameFlow = gameController.GetComponent<GameFlow>();

        moveBody = transform.parent.gameObject;

        if (sizeScale > 0) setSizeScale(sizeScale);
        if (colliderScale > 0) setColliderScale(colliderScale);

        lifeBar = GameObject.FindWithTag("BossLife");
        if (hoverBoundsUpper.Equals(Vector2.zero)) hoverBoundsUpper = new Vector2(Global.MainCanvasWidth / 2, Global.MainCanvasHeight / 2);
        if (hoverBoundsLower.Equals(Vector2.zero)) hoverBoundsUpper = new Vector2(Global.MainCanvasWidth / 2, Global.MainCanvasHeight / 2);

        StartCoroutine(Global.WaitUntilThenDo(setLifeRT, (lifeBar != null)));
           
    }

    public void Update()
    {
        setLifeBar();
    }

    public IEnumerator idleHover()
    {
        Debug.Log("idle hovering; "+currMode);
        while(currMode.Equals(bossMode.IDLE)) //each while loop is one route 
        {
            float spd = getCalcValue(movementSpd), range = getCalcValue(movementRange),
                hoverTime = getCalcValue(hoverDuration), secondNoise = getCalcValue(secondLayerNoise);

            Vector3 destination, dir;
            do {
                float angle = UnityEngine.Random.Range(0.0f, Mathf.PI * 2); //random angle with no limit on direction
                dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(-angle), 0f);

                Ray ray = new Ray(transform.position, dir);
                destination = ray.GetPoint(range); //goal is to get to destination with generated spd and noise, then stay there for hoverTime
            } while ((destination.x > hoverBoundsUpper.x) || (destination.y > hoverBoundsUpper.y ) || (destination.x < -hoverBoundsLower.x) || (destination.y < -hoverBoundsLower.y)); 
            //keep randomize destination until satisfies constraints

            // Debug.Log("gen values " + spd + " " + range + " " + hoverTime + " " + secondNoise + " " + dir + " " + destination);
            bool[] moveDone = { false };
            StartCoroutine(Global.moveTo(moveBody, (int)(destination.x), (int)(destination.y), spd, moveDone));
            yield return new WaitUntil(() => moveDone[0]);

            yield return new WaitForSeconds(hoverTime);
        }
    }

    /// <summary>
    /// the boss might have multiple kinds of direct attack, and the attkIndex is for indicating which one is intended to be called
    /// 
    /// we assume that attkIndex would provide us the values of all hardcoded in variables in function
    /// </summary>
    /// <param name="attkIndex"></param>
    /// <returns></returns>
    public IEnumerator directAttack(int attkIndex, bool[] done)
    {
        bool hasAnticip = true; bool[] anticip_done = new bool[1];
        if (hasAnticip)
        {
            //TODO swap into sprite for anticipation
            setMode(bossMode.ANTICIP);
		Debug.Log("anticipation");

		yield return new WaitForSeconds(2);
            anticip_done[0] = true;
        }
        yield return new WaitUntil(() => anticip_done[0]); //anticipation done

        setMode(bossMode.DIR_ATTK);
        Vector2 attkSpot = player.transform.position, from = transform.position; //targets player position *at this moment in time* (means could miss when actually reaching there)
        float chargeSpd = 700, stopDistAway = 30; //stops some distance away from player's exact location to attack
        //TODO play charge animation/sprite
        Ray toPlayer = new Ray(from, attkSpot); bool[] charge_done = new bool[1];
        Vector2 dest = toPlayer.GetPoint(Global.findVectorDist(from, attkSpot) - stopDistAway);

        StartCoroutine(Global.moveTo(moveBody, attkSpot, chargeSpd, charge_done));
        yield return new WaitUntil(() => charge_done[0]);

        //TODO collider will need to change here
        //TODO play attack animation; wait till done
        yield return new WaitForSeconds(1.5f);

        //TODO return animation
        Vector2 returnTo = from; bool[] return_done = new bool[1];
        float returnSpd = chargeSpd/2;
        StartCoroutine(Global.moveTo(moveBody, from, returnSpd, return_done));
        yield return new WaitUntil(() => return_done[0]);

        done[0] = true;
    }

    public void setMode(bossMode m){
        currMode = m;
        bossVisual.updateModeSprite(m);
    }

    //might need to be overridden
    public void resetStats()
    {
        life = maxLife;
        stage = 0;
    }

    public void setLifeBar()
    {
        if (lifeBarActive) //if inactive, unnecessary to do the calculation + update
        {
            float percentage = (float)life / (float)maxLife;

            //if(!(percentage >= 0 && percentage <= 1))
            //{
            //    Debug.LogError("percentage life error: " + percentage);
            //}

            if (lifeRT != null)
                lifeRT.localScale = new Vector3(percentage, 1, 1);
        }
    }

    public void showLifeBar()
    {
        lifeBarActive = true;
        lifeContainer.SetActive(true);
    }

    public void hideLifeBar()
    {
        lifeBarActive = false;
        lifeContainer.SetActive(false);
    }

    //is called on start once lifeBar GO is found and ready, should be only called once
    public void setLifeRT()
    {
        lifeContainer = lifeBar.transform.parent.parent.gameObject; //parent is mask, container is grandparent
        lifeRT = lifeBar.GetComponent<RectTransform>();
        hideLifeBar(); //usually lifeBar for boss isn't needed on start
    }

    public void setSizeScale(float sScale)
    {
        sizeScale = sScale;

        if (Global.scaleRatio != 0)
        {
            transform.localScale = new Vector3(sizeScale * Global.scaleRatio,
                sizeScale * Global.scaleRatio, sizeScale * Global.scaleRatio);
        }
    }

    public void setColliderScale(float cScale)
    {
        Vector2[] colliderPoints = GetComponent<PolygonCollider2D>().points;
        Vector2[] scaledPoints = new Vector2[colliderPoints.Length];
        for (int p = 0; p < colliderPoints.Length; p++)
        {
            Vector2 np = GetComponent<PolygonCollider2D>().points[p] * colliderScale;
            scaledPoints[p] = np;
        }
        GetComponent<PolygonCollider2D>().SetPath(0, scaledPoints);
    }

    public void damage(int damage, Color col)
    {
        life -= damage;
        //audioz.enemyDamagedSE();
        StartCoroutine(damageVFXboss(col));
    }

    //can be overridden by children
    IEnumerator damageVFXboss(Color col)
    {
        for (int i = 0; i < 2; i++)
        {
            //              //flip visibility on and off
            GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;

            yield return new WaitForSeconds(0.1f);
        }
    }


    public void triggerBuff(Enemy.BuffMode b)
    {
        if (currBuffProcess != null) StopCoroutine(currBuffProcess);
        debuff = b; GameObject explosionPrefab;
        bool parentToOther = false;

        switch (b)
        {
            case Enemy.BuffMode.burn:
                explosionPrefab = prefabHolder.palletExplosionRed;
                parentToOther = true;

                currBuffProcess = burnDuration();
                StartCoroutine(currBuffProcess);
                break;
            case Enemy.BuffMode.freeze:
                explosionPrefab = prefabHolder.palletExplosionBlue;
                parentToOther = true;

                currBuffProcess = freezeDuration();
                StartCoroutine(currBuffProcess);
                break;
            default:
                Debug.Log("buff case not recognized");
                explosionPrefab = null;
                break;

        }

        GameObject effect = (explosionPrefab != null) ? Instantiate(explosionPrefab, transform.position, transform.rotation) : null;
        if (parentToOther) effect.transform.parent = transform;
    }

    IEnumerator burnDuration()
    {
        life -= 3; //TODO edit
                   //TODO sound effect
                   //TODO instantiate vfx here
        yield return new WaitForSeconds(2);
    }

    IEnumerator freezeDuration()
    {
        //sfx, vfx
        yield return new WaitForSeconds(3);
        debuff = Enemy.BuffMode.none; //cancels after duration
    }

    ////////////////////////

    /**
     * returns the instantiated projectile
     * 
     * make sure spawnPos is in world coordinates
     * the projectile doesn't necessarily need a Projectile script attached, but a Rigidbody2D is a must   
     * 
     * if inst is true, assumes GO proj is a mold, instantiates another copy of it;
     * if inst is false, will not instantiate but use proj directly
     */
    public static GameObject shootProjectileAt(bool inst, GameObject proj, Vector3 spawnPos, Vector3 dir, float spd, float angle)
    {
        GameObject p;
        if (inst)
        {
            p = Instantiate(proj, spawnPos, proj.transform.rotation) as GameObject;
        }
        else
        {
            p = proj;
        }

        if (proj.CompareTag("Enemy"))
        {
            p.GetComponent<EnemyMover>().setVelocity(new Vector2(((dir.y > 0) ? 10 : -10) * Mathf.Sin(angle) * spd,
                 ((dir.y > 0) ? 10 : -10) * Mathf.Cos(angle) * spd));
        }
        else { 
        p.GetComponent<Rigidbody2D>().velocity =
                  new Vector2(((dir.y > 0) ? 10 : -10) * Mathf.Sin(angle) * spd,
                 ((dir.y > 0) ? 10 : -10) * Mathf.Cos(angle) * spd);
    }

        return p;
    }

    /*
     * NEED TEST
     * range for startAngle and endAngle is 0-360 (unit circle), with endAngle always greater or equal to startAngle
     */
    public static ArrayList shootGroupProjectiles(GameObject prefab, Vector3 spawnPos, float startAngle, float endAngle, int num_pellets, float spd)
    {
        ArrayList pellets = new ArrayList();

        float incre = (endAngle - startAngle) / num_pellets;
        float currAngle = startAngle;
        for(int n=0; n<num_pellets; n++)
        {
            pellets.Add(
            shootProjectileAt(true, prefab, spawnPos, new Vector3((currAngle >= 90 && currAngle <= 270) ? -1 : 1, 0), spd, currAngle));
            
            currAngle += startAngle;
        }
        return pellets;
    }

    /*
     * NEED TEST
     * overload method that instead of taking starting and ending angles, only takes the starting angle and the increment angle
     */
    public static ArrayList shootGroupProjectiles(float startAngle, float incre, int num_pellets, GameObject prefab, Vector3 spawnPos, float spd)
    {
        float endAngle = startAngle + incre * num_pellets;
        return shootGroupProjectiles(prefab, spawnPos, startAngle, endAngle, num_pellets, spd);
    }


        public static IEnumerator doForNumTimes(bool[] done, MonoBehaviour g, int n, float interval, IEnumerator action)
    {
        for (int l = 0; l < n; l++)
        {
            g.StartCoroutine(action);
            yield return new WaitForSeconds(interval);
        }
        done[0] = true;
    }

    public static IEnumerator doForNumTimes(bool[] done, MonoBehaviour g, int n, float interval, Action action)
    {
        for (int l = 0; l < n; l++)
        {
            action();
            yield return new WaitForSeconds(interval);
        }
        done[0] = true;
    }



    // ------------------------------------------------------HELPER FUNCTIONS--------------------------------------------------------------------
    /// <summary>
    /// takes in tuple of (value, erraticity)
    /// </summary>
    /// <param name="instance"></param>
    /// <returns>returns a calculated value applied with erraticity</returns>
    private float getCalcValue((float, float) instance)
    {
        float val = instance.Item1, err = instance.Item2;
        float sign = UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f ? 1 : -1;
        float pickErr = UnityEngine.Random.Range(0.0f, err);
        val *= 1 + sign * pickErr;
        return val;
    }

    private float getCalcValue(Vector2 instance)
    {
        return getCalcValue((instance.x, instance.y));
    }

    // ------------------------------------------------------END HELPER FUNCTIONS--------------------------------------------------------------------
}
