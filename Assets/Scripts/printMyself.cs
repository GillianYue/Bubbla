using UnityEngine;
using System.Collections;

public class printMyself : MonoBehaviour {

	// Use this for initialization
	void Start () {
		print ("Name "+gameObject.name+ "global " + gameObject.transform.position + 
			" localScale" + transform.localScale +
		" lossyScale" + gameObject.transform.lossyScale);
	}
	
	// Update is called once per frame
	void Update () {
		print ("Name "+gameObject.name+ "global " + gameObject.transform.position + 
			" localScale" + transform.localScale +
			" lossyScale" + gameObject.transform.lossyScale);
	}
}
