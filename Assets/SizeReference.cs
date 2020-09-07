using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeReference : MonoBehaviour
{
    public Rect sizeRef;
    public float percent = 0.1f;

    void Start()
    {
        sizeRef = GetComponent<RectTransform>().rect;
        Global.setRectShape(gameObject, Screen.width * percent, Screen.width * percent);

        transform.localPosition = Vector3.zero;
        Debug.Log("size ref scale lossy " + GetComponent<RectTransform>().lossyScale);
        Global.SetGlobalScale(GetComponent<RectTransform>(), Vector2.one);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
