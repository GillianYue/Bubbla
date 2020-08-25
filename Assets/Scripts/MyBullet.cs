using UnityEngine;
using System.Collections;

public class MyBullet : MonoBehaviour {
	
	public float accl;
	public GameObject trail;
	public GameObject explosion;
    public int damage = 1;
    public float bulletSpeed;

    public void Start () {
        if (trail != null)
        {
            GameObject t = trail;
            t = Instantiate(trail, gameObject.transform.position,
                trail.transform.rotation) as GameObject;
            t.GetComponent<TrailFollowBall>().setMyBullet(gameObject);
        }
	}

	public void Update () {

	}

    public void setVelocity(Vector3 direction, float angle)
    {
        GetComponent<Rigidbody2D>().
            velocity = new Vector2(((direction.y > 0) ? 10 : -10) * Mathf.Sin(angle) * bulletSpeed,
                ((direction.y > 0) ? 10 : -10) * Mathf.Cos(angle) * bulletSpeed);

        transform.Rotate(new Vector3(0, 0,
                ((direction.y > 0) ? -1 : 1) * Mathf.Rad2Deg * angle));
    }

    public void FixedUpdate()
    {
        //acceleration
        GetComponent<Rigidbody2D>().velocity += new Vector2
            (GetComponent<Rigidbody2D>().velocity.x * accl,
                GetComponent<Rigidbody2D>().velocity.y * accl);
    }

    void OnTriggerEnter2D(Collider2D other){
        string t = other.GetComponent<Collider2D>().tag;
        //if bullet hits enemy, it bursts and damages enemy
        if (t == "Enemy") {
			if (explosion != null) {
				Instantiate (explosion, transform.position, transform.rotation);
				other.GetComponent<Enemy> ().damage (damage, 
					gameObject.GetComponent<SpriteRenderer>().color);
				Destroy (gameObject);
			}
		}else if(t == "Boss")
        {
            other.transform.parent.GetComponent<BossBehavior>().damage(damage,
    gameObject.GetComponent<SpriteRenderer>().color);
            Destroy(gameObject);
        }

	}
}
