using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {

	public int seconds;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, seconds);
	}

}
