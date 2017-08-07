using UnityEngine;
using System.Collections;

public class Spiee : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(tag.Equals("Bullet")){
			print("Imma bullet "+GetComponent<Rigidbody>().velocity);
		}
	}
}
