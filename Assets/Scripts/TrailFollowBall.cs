using UnityEngine;
using System.Collections;

public class TrailFollowBall : MonoBehaviour {
	
	private GameObject myBullet;
	public float lastingTime;
	public DestroyByTime dbt;

	void Start () {
		dbt.enabled = false;
		gameObject.transform.GetChild (0).gameObject.GetComponent<ParticleSystem> ().startColor = 
		myBullet.gameObject.GetComponent<MeshRenderer> ().material.color;

		//print (gameObject.transform.GetChild (0).gameObject.GetComponent<TrailRenderer> ().material.color);
	}
		
	void Update () {
		if (myBullet != null) {
			transform.position = myBullet.transform.position;
		} else {
			if (!dbt.enabled) {
				dbt.enabled = true;
			}
		}
	}

	public void setMyBullet(GameObject b){
		myBullet = b;
	}

	public GameObject getMyBullet(){
		return myBullet;
	}

}
