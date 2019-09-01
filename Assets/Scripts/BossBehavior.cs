using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/**
 * abstract base class for levelscripts, which contain custom functions for special events in each level
 */
public abstract class BossBehavior : MonoBehaviour
{

    public int life = 1000;
    public int attack;
    public int stage; //0

    public float sizeScale; //same as paintball, base scale to be multiplied with global
    public float colliderScale; //multiplied to the original generated polygon 2d collider shape to resize

    protected GameControl gameControl;
    protected GameFlow gameFlow;
    protected CustomEvents customEvents;

    public void Start()
    {
        customEvents = gameObject.GetComponent<CustomEvents>();
        GameObject gameController = GameObject.FindWithTag("GameController");
        gameControl = gameController.GetComponent<GameControl>();
        gameFlow = gameController.GetComponent<GameFlow>();

        if (sizeScale > 0) setSizeScale(sizeScale);
        if (colliderScale > 0) setColliderScale(colliderScale);

    }

    void Update()
    {
        
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
}
