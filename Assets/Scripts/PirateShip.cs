using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PirateShip : BossBehavior
{
    private Player player;
    public GameObject projectile;
    public float cannonSpeed;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        life = 50;
        attack = 3; //cannonball damage
        player = gameControl.player.GetComponent<Player>();

        projectile = Resources.Load("Projectile") as GameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
