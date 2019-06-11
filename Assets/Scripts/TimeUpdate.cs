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
            var hr = (System.DateTime.Now.Hour -
            (System.DateTime.Now.Hour > 11 ? 12 : 0));
            var mode = (System.DateTime.Now.Hour > 11 ? "PM" : "AM");
            if (hr == 0 && mode == "PM")
            {
                hr = 12;
            }
            GetComponent<Text> ().text = ( hr +
			":" + (System.DateTime.Now.Minute < 10 ? "0" : "")
			+ System.DateTime.Now.Minute + " "
			+ mode);
		}
	}
}
