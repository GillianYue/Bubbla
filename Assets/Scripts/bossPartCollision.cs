using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bossPartCollision : MonoBehaviour
{
    public BossBehavior main;

    void Start()
    {
        if (main == null) main = transform.parent.GetComponent<BossStateManager>();
    }


    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Collider2D>().tag == "Player")
        {
            other.gameObject.GetComponent<Player>().damage(main.attack);
        }
    }
}
