using UnityEngine;
using System.Collections;

public class TrailFollowBall : MonoBehaviour {
	
	private GameObject myBullet;
	public DestroyByTime dbt;
	public ParticleSystem[] trail;

	void Start () {
		if(!dbt) dbt = GetComponent<DestroyByTime>();
		dbt.enabled = false;
	}
		
	void Update () {
		if (myBullet != null) {
			transform.position = myBullet.transform.position;
		} else {
			if (!dbt.enabled) {
				dbt.enabled = true;
				foreach (ParticleSystem t in trail)
				{
					ParticleSystem.EmissionModule em = t.emission;
					em.enabled = false;
				}
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
