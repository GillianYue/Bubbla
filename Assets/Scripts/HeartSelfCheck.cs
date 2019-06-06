using UnityEngine;
using System.Collections;

public class HeartSelfCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Mathf.Abs(gameObject.transform.localPosition.x)>5 ||
			Mathf.Abs(gameObject.transform.localPosition.y)>5){
			Destroy (gameObject);
		}
	}
}
