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
		GetComponent<Rigidbody> ().velocity += new Vector3 
			(GetComponent<Rigidbody> ().velocity.x * accl,  
				GetComponent<Rigidbody> ().velocity.y *accl, 0);


	}

	void OnTriggerEnter(Collider other){

		//if bullet hits enemy, it bursts and damages enemy
		if (other.GetComponent<Collider>().tag == "Enemy") {
			if (explosion != null) {
				Instantiate (explosion, transform.position, transform.rotation);
				other.GetComponent<Enemy> ().damage (damage, 
					gameObject.GetComponent<SpriteRenderer>().color);
				Destroy (gameObject);
			}
		}

	}
}
