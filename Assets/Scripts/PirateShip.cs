using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PirateShip : BossBehavior
{
    private Player player;
    public GameObject projectile;
    public float cannonSpeed; //velocity of shot cannonballs
    public float enemyShootSpeed; //velocity of shot enemies
    public Text lifeText;
    private EnemyLoader enemyLoader;

    new void Start()
    {
        base.Start();
        life = 50;
        attack = 3; //cannonball damage
        player = gameControl.player.GetComponent<Player>();
        enemyLoader = gameFlow.loader.GetComponent<EnemyLoader>();

        projectile = Resources.Load("Projectile") as GameObject;
    }

    void Update()
    {
        lifeText.text = "boss life: " + life;
    }

    public IEnumerator bossFight(bool[] done)
    {
        int enemy = 0; int turns = 0; stage = 1;
        while(life > 0)
        {
            bool[] d = new bool[1]; //done checker for the local switch action
            switch (stage)
            {
                case 0:
                    yield return new WaitForSeconds(1);
                    d[0] = true;
                    break;
                case 1:
                    StartCoroutine(doForNumTimes(d, this, 4, 1.5f, Global.Do(() => fireCannonball(new bool[1]))));
                    break;
                case 2:
                    StartCoroutine(doForNumTimes(d, this, 8, 2f, Global.Do(() => fireEnemy(new bool[1], enemy))));
                    break;
            }
            yield return new WaitUntil(() => d[0]);
            yield return new WaitForSeconds(2);

            turns++; 
            if(stage == 1 && turns == 6)
            {
                yield return new WaitForSeconds(3);
                stage = 2;
                turns = 0;
            }else if (stage == 2 && turns == 4)
            {
                yield return new WaitForSeconds(3);
                stage = 1;
                turns = 0;
            }
        }
        done[0] = true;
    }

    /**
     * fires a cannonball at the player's direction
     * 
     */    
    public void fireCannonball(bool[] done)
    {
        //start pointin cannon
        Vector3 ship = Global.WorldToScreen(gameObject.transform.position);
        Vector3 direction = Camera.main.WorldToScreenPoint(player.transform.position) - ship;
        float tan = direction.x / direction.y;
        float angle = Mathf.Atan(tan);
        //*************THE ANGLE IS HERE*************

        float dgAngle = -1 * (angle * Mathf.Rad2Deg); //convert from radian to dgr

        GameObject cannonball = shootProjectileAt(projectile, gameObject.transform.position, direction, cannonSpeed, angle); //already instantiated
        cannonball.transform.parent = transform;

        done[0] = true;
    }

    public void fireEnemy(bool[] done, int eCode)
    {
        //start pointin cannon
        Vector3 ship = Global.WorldToScreen(gameObject.transform.position);
        Vector3 direction = Camera.main.WorldToScreenPoint(player.transform.position) - ship;
        float tan = direction.x / direction.y;
        float angle = Mathf.Atan(tan);
        //*************THE ANGLE IS HERE*************

        float dgAngle = -1 * (angle * Mathf.Rad2Deg); //convert from radian to dgr

        GameObject e = enemyLoader.getEnemyInstance(eCode);
        shootProjectileAt(e, gameObject.transform.position, direction, enemyShootSpeed, angle); //already instantiated
        e.transform.parent = transform;

        done[0] = true;
    }

}
