using UnityEngine;
using System.Collections;

public class AudioStorage : MonoBehaviour {

	private AudioSource[] everything;

	private AudioSource[] enemyDamageSE, enemyDieSE, pbAbsorption;

	private AudioSource pbExplosion;

	// Use this for initialization
	void Start () {
		everything = GetComponents<AudioSource> ();

		enemyDamageSE = new AudioSource[5];
		enemyDieSE = new AudioSource[5];
		pbAbsorption = new AudioSource[3];

		for (int i = 0; i < 5; i++) {
			enemyDamageSE.SetValue (everything [i], i);
		}
		enemyDieSE = enemyDamageSE; //for now

		for (int i = 0; i < 3; i++) {
			pbAbsorption.SetValue (everything [i+5], i);
		}
		pbExplosion = everything [8];

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void enemyDamagedSE(){
		enemyDamageSE [(int)(Random.Range (0, enemyDamageSE.Length - 0.01f))].Play();
	}

	public void enemyDefeatedSE(){
		enemyDieSE [(int)(Random.Range (0, enemyDieSE.Length - 0.01f))].Play();
	}

	public void paintballAbsorptionSE(){
		pbAbsorption [(int)(Random.Range (0, pbAbsorption.Length - 0.01f))].Play();
	}

	public void paintballExplosionSE(){
		pbExplosion.Play ();
	}
}
