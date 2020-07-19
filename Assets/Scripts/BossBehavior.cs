using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * abstract base class for levelscripts, which contain custom functions for special events in each level
 */
public abstract class BossBehavior : MonoBehaviour
{

    public int life = 1000, maxLife = 1000;
    public int attack;
    public int stage; //0

    public float sizeScale; //same as paintball, base scale to be multiplied with global
    public float colliderScale; //multiplied to the original generated polygon 2d collider shape to resize

    protected GameControl gameControl;
    protected GameFlow gameFlow;
    protected CustomEvents customEvents;

    public GameObject lifeBar; //the one with image attached
    public GameObject lifeContainer; //parent of lifeBar
    protected RectTransform lifeRT;
    public bool lifeBarActive;

    public enum bossMode { IDLE, DAMAGE, DIR_ATTK, SHOOT_ATTK, PROJ_ATTK, SPEC_ATTK, ANTICIP, DEFEATED };
    public bossMode currMode = bossMode.IDLE;
    /// <summary>
    /// the second float in the tuple is erraticity of that attribute which ranges from 0 to 1
    /// to expose in the editor, is typed Vector2 instead of (float, float)
    /// </summary>
    public Vector2 movementSpd, movementRange, hoverDuration, secondLayerNoise; 

    public void Start()
    {
        customEvents = gameObject.GetComponent<CustomEvents>();
        GameObject gameController = GameObject.FindWithTag("GameController");
        gameControl = gameController.GetComponent<GameControl>();
        gameFlow = gameController.GetComponent<GameFlow>();

        if (sizeScale > 0) setSizeScale(sizeScale);
        if (colliderScale > 0) setColliderScale(colliderScale);

        lifeBar = GameObject.FindWithTag("BossLife");

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
            Debug.Log("inside");
            float spd = getCalcValue(movementSpd), range = getCalcValue(movementRange),
                hoverTime = getCalcValue(hoverDuration), secondNoise = getCalcValue(secondLayerNoise);
            float angle = UnityEngine.Random.Range(0.0f, Mathf.PI * 2); //random angle with no limit on direction
            Vector3 dir = new Vector3(Mathf.Cos(angle), Mathf.Sin(-angle), 0f);

            Ray ray = new Ray(transform.position, dir);
            Vector3 destination = ray.GetPoint(range); //goal is to get to destination with generated spd and noise, then stay there for hoverTime

            Debug.Log("gen values " + spd + " " + range + " " + hoverTime + " " + secondNoise + " " + dir + " " + destination);
            bool[] moveDone = { false };
            StartCoroutine(Global.moveTo(this.gameObject, (int)(destination.x), (int)(destination.y), spd, moveDone));
            yield return new WaitUntil(() => moveDone[0]);
            Debug.Log("waiting on hover");
            yield return new WaitForSeconds(hoverTime);
            Debug.Log("one route done, starting another");
        }
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
