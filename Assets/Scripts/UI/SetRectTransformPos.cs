using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRectTransformPos : MonoBehaviour {
	public RectTransform copyRect;

	// Use this for initialization
	void Start () {
		RectTransform rt = GetComponent<RectTransform>();
		rt.anchoredPosition = copyRect.anchoredPosition;
		// Debug.Log( gameObject.transform.position + " " + rt.anchoredPosition ) ;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
