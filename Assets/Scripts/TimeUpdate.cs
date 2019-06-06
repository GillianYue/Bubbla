using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeUpdate : MonoBehaviour {
	public bool updt;
	// Use this for initialization
	void Start () {
		GetComponent<Text> ().text = ((System.DateTime.Now.Hour -
			(System.DateTime.Now.Hour > 11 ? 12 : 0)) +
			":" + (System.DateTime.Now.Minute < 10 ? "0" : "")
			+ System.DateTime.Now.Minute + " "
			+ (System.DateTime.Now.Hour > 11 ? "PM" : "AM"));
		updt = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (updt) {
			GetComponent<Text> ().text = ((System.DateTime.Now.Hour -
			(System.DateTime.Now.Hour > 11 ? 12 : 0)) +
			":" + (System.DateTime.Now.Minute < 10 ? "0" : "")
			+ System.DateTime.Now.Minute + " "
			+ (System.DateTime.Now.Hour > 11 ? "PM" : "AM"));
		}
	}
}
