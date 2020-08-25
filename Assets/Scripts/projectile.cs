using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MyBullet
{

    new void Start()
    {
        base.Start(); //instantiating trail if it's not null


    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();

    }

    /**
     * unlike in bullet, we want to detect collision with the player
     */
    void OnTriggerEnter2D(Collider2D other)
    {

        //if projectile hits player, should damage player 
        if (other.GetComponent<Collider2D>().tag == "Player")
        {
            if (explosion != null)
            {
                Instantiate(explosion, transform.position, transform.rotation);
            }
            other.GetComponent<Player>().damage(damage);
            Destroy(gameObject);

        }

    }

}
