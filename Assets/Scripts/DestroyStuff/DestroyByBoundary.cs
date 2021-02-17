using UnityEngine;
using System.Collections;
/**
 * heart checks if it's out of screen. Self-destructs if yes.
 */
public class DestroyByBoundary : MonoBehaviour {

	void OnTriggerExit2D(Collider2D other){

		Debug.Log("about to destroy: " + other.name);

		if(!other.CompareTag("Boss")) //boss can be enabled and disabled, which would count as triggering the exit event
	   Destroy(other.gameObject);

	}

}
