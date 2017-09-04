using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	int life = 1000;
	int attack;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (life <= 0) {
			Destroy (gameObject);
		}
	}

	public int getLife(){
		return life;
	}

	public void setValues(int LIFE, int ATTACK){
		life = LIFE;
		attack = ATTACK;
	}

	public void damage(int damage){
		life -= damage;
	}

	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Collider> ().tag == "Player") {
			other.gameObject.GetComponent<Player> ().damage (attack);
		}
	}
}
