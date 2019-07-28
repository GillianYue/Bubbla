using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	
	public float accl;
	public GameObject trail;
	public GameObject explosion;
    public int damage = 1;

	void Start () {
		GameObject t = trail;
		t = Instantiate (trail, gameObject.transform.position,
			trail.transform.rotation) as GameObject;
		t.GetComponent<TrailFollowBall> ().setMyBullet (gameObject);
	}

	void Update () {
		//acceleration
		GetComponent<Rigidbody2D> ().velocity += new Vector2 
			(GetComponent<Rigidbody2D> ().velocity.x * accl,  
				GetComponent<Rigidbody2D> ().velocity.y *accl);


	}

	void OnTriggerEnter2D(Collider2D other){

		//if bullet hits enemy, it bursts and damages enemy
		if (other.GetComponent<Collider2D>().tag == "Enemy") {
			if (explosion != null) {
				Instantiate (explosion, transform.position, transform.rotation);
				other.GetComponent<Enemy> ().damage (damage, 
					gameObject.GetComponent<SpriteRenderer>().color);
				Destroy (gameObject);
			}
		}

	}
}
