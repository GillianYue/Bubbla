using UnityEngine;
using System.Collections;

public class Accelerometer : MonoBehaviour 
{
	void Update () 
	{
		transform.Translate(Input.acceleration.x, 0, -Input.acceleration.z);
	}
}