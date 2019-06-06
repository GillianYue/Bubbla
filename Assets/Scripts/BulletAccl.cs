using UnityEngine;
using System.Collections;

public class BulletAccl : MonoBehaviour {
	
	public float accl;
	public GameObject trail;
	public GameObject explosion;

	void Start () {
		GameObject t = trail;
		t = Instantiate (trail, gameObject.transform.position,
			trail.transform.rotation) as GameObject;
		t.GetComponent<TrailFollowBall> ().setMyBullet (gameObject);

	}

	void Update () {
		//acceleration
		GetComponent<Rigidbody> ().velocity += new Vector3 
			(GetComponent<Rigidbody> ().velocity.x * accl, 0, 
				GetComponent<Rigidbody> ().velocity.z *accl);


	}

	void OnTriggerEnter(Collider other){

		//if bullet hits enemy, it bursts and damages enemy
		if (other.GetComponent<Collider>().tag == "Enemy") {
			if (explosion != null) {
				Instantiate (explosion, transform.position, transform.rotation);
				other.GetComponent<Enemy> ().damage (1, 
					gameObject.GetComponent<SpriteRenderer>().color);
				Destroy (gameObject);
			}
		}

	}
}
