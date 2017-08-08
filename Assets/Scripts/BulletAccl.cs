using UnityEngine;
using System.Collections;

public class BulletAccl : MonoBehaviour {
	
	public float accl;
	public GameObject trail;

	void Start () {
		GameObject t = trail;
		t = Instantiate (trail, gameObject.transform.position,
			trail.transform.rotation) as GameObject;
		t.GetComponent<TrailFollowBall> ().setMyBall (gameObject);

	}

	void Update () {
		//acceleration
		GetComponent<Rigidbody> ().velocity += new Vector3 
			(GetComponent<Rigidbody> ().velocity.x * accl, 0, 
				GetComponent<Rigidbody> ().velocity.z *accl);


	}
}
