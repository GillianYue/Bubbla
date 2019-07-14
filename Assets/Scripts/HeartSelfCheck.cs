using UnityEngine;
using System.Collections;

public class HeartSelfCheck : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    //TODO this is not good, take 5 out and swap with some sort of calculation with Global.constants
	void Update () {
		if(Mathf.Abs(gameObject.transform.localPosition.x)>5 ||
			Mathf.Abs(gameObject.transform.localPosition.y)>5){
			//Destroy (gameObject);
		}
	}
}
