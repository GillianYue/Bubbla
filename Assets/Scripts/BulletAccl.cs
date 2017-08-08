using UnityEngine;
using System.Collections;

public class BulletAccl : MonoBehaviour {
	
	public float accl;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GetComponent<Rigidbody> ().velocity += new Vector3 
			(GetComponent<Rigidbody> ().velocity.x * accl, 0, 
				GetComponent<Rigidbody> ().velocity.z *accl);
	}
}
