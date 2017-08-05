using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {

	public float speed;
	private Rigidbody rb;
	public Vector3 direction;

	void Start() {
		//Rigidbody
		rb = GetComponent<Rigidbody> ();
		rb.velocity = direction * speed;
	}

	void Update(){
		
	}
}
