using UnityEngine;
using System.Collections;

public class TrailFollowBall : MonoBehaviour {
	
	private GameObject myBullet;
	public float lastingTime;
	public DestroyByTime dbt;
	public ParticleSystem smokeTrail;

	void Start () {
		dbt.enabled = false;
		smokeTrail = gameObject.transform.GetChild (0).gameObject.GetComponent<ParticleSystem> ();
		smokeTrail.startColor = 
		myBullet.gameObject.GetComponent<MeshRenderer> ().material.color;

		//print (gameObject.transform.GetChild (0).gameObject.GetComponent<TrailRenderer> ().material.color);
	}
		
	void Update () {
		if (myBullet != null) {
			transform.position = myBullet.transform.position;
		} else {
			if (!dbt.enabled) {
				dbt.enabled = true;
				ParticleSystem.EmissionModule em = smokeTrail.emission;
				em.enabled = false;
			}
		}
		print (smokeTrail.emission.enabled);
	}

	public void setMyBullet(GameObject b){
		myBullet = b;
	}

	public GameObject getMyBullet(){
		return myBullet;
	}

}
