using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	int life = 1000;
	int attack;
	public AudioStorage audioz;

	// Use this for initialization
	void Start () {
		audioz = GameObject.FindWithTag ("AudioStorage").GetComponent<AudioStorage>();
	}
	
	// Update is called once per frame
	void Update () {
		if (life <= 0) {
			audioz.enemyDefeatedSE ();
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
		audioz.enemyDamagedSE ();
		StartCoroutine (damageVFXenemy());
	}

	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Collider> ().tag == "Player") {
			other.gameObject.GetComponent<Player> ().damage (attack);
		}
	}

	IEnumerator damageVFXenemy(){
		for (int i = 0; i < 2; i++) {
			//flip visibility for three times
			GetComponent<SpriteRenderer> ().enabled = !GetComponent<SpriteRenderer> ().enabled;

			yield return new WaitForSeconds (0.1f);
		}
	}
}
