using UnityEngine;
using System.Collections;
/**
 * heart checks if it's out of screen. Self-destructs if yes.
 */
public class DestroyByBoundary : MonoBehaviour {

	void OnTriggerExit(Collider other){
        if (other.CompareTag("Hearts"))
        {
            Destroy(other.gameObject);
        }
	}

}
