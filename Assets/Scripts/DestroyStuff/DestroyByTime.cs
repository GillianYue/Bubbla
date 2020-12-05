using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour {

	public float seconds;

	// Use this for initialization
	void Start () {
		Destroy (gameObject, seconds);
	}

}
