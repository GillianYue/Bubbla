using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//"enemy bullet"
public class projectile : MyBullet
{

    new void Start()
    {
        base.Start(); //instantiating trail if it's not null


    }

    new void Update()
    {
        base.Update();
    }

    /**
     * unlike in bullet, we want to detect collision with the player; override
     */
    void OnTriggerEnter2D(Collider2D other)
    {

        //if projectile hits player, should damage player 
        if (other.GetComponent<Collider2D>().tag == "Player")
        {

              //  Instantiate(prefabHolder.palletExplosion, transform.position, transform.rotation);

            other.GetComponent<Player>().damage(damage);
            Destroy(gameObject);

        }

    }

}
