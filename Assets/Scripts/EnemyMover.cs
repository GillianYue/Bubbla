﻿using UnityEngine;
using System.Collections;

public class EnemyMover : MonoBehaviour {

	public float speed;
	private Rigidbody rb;
	public Vector3 direction;
	public int enemyType;
	//type 0 inanimate, 1 move-stop, 2 screen span, 3 track
	public int scnRange; //half OffMeshLink world Screen width
	public Camera cam;


	void Start() {
		//Rigidbody
		rb = GetComponent<Rigidbody> ();
		rb.velocity = direction * speed;

		switch(enemyType){
		case 1: //crab
			StartCoroutine (movePause (8, 2f));
			break;
		case 2: 
		//	StartCoroutine (screenSpan ());
			break;
		default:
			//just sticks with the original velocity
			break;
		}
	}

	void Update(){
		
	}

	IEnumerator movePause (int turns, float speed){

		for (int t = 0; t < turns; t++) {
			rb.velocity += new Vector3(speed * Mathf.Pow(-1, t%2), 0, 0);
			GetComponent<SpriteRenderer> ().flipX = (t%2 == 0 ? false : true);
			//even numbers: positive velocity
			if (t % 2 == 0) {
				while (rb.transform.position.x < 
					(Screen.width * Global.PixelToWorldFactor.x/2)*0.8) {
					yield return new WaitForSeconds (.5f);
					//not changing the velocity (stuck in this loop) until reaching the target
				}
			} else { //else, odd, negative velocity
				while (rb.transform.position.x > 
					-1*(Screen.width * Global.PixelToWorldFactor.x/2)*0.8) {
					yield return new WaitForSeconds (.5f);
				}
			}

			//reset speed upon reaching the target
			rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
		}
	}

	/*IEnumerator screenSpan(){
		//enemy travels through screen

	}
	*/
}