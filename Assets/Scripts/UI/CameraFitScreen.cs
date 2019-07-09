using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFitScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject canv = GameObject.FindGameObjectWithTag("Canvas");
        GetComponent<Camera>().orthographicSize = (canv.
            GetComponent<RectTransform>().rect.height / 2) *
            canv.transform.localScale.y ;
        Global.setGlobalConstants(GetComponent<Camera>().orthographicSize);

        Vector2 cPos = canv.GetComponent<RectTransform>().localPosition;
        Vector3 pos = new Vector3(cPos.x, cPos.y, -500); //camera is the farthest away, sees everything
//******* thus, in our game, the more negative (smaller) the z axis is, the higher priority of the visibility
        transform.position = pos;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
