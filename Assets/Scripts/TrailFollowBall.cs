using UnityEngine;
using System.Collections;

public class TrailFollowBall : MonoBehaviour {
	
	private GameObject myBullet;
	public float lastingTime;
	public DestroyByTime dbt;

	void Start () {
		dbt.enabled = false;
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

	public void setMyBall(GameObject b){
		myBullet = b;
	}


}
