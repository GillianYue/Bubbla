using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFitScreen : MonoBehaviour
{
    /**
     * this script fits the camera to the screen by adjusting its orthographic size.
     */
    void Start()
    {

        //getting resolution of phone: see below commented line
        Vector2 gameViewSize = UnityEditor.Handles.GetMainGameViewSize();
/*        Debug.Log("gameView size: " + gameViewSize);*/

        float gvRatio = (float)gameViewSize.y / (float)gameViewSize.x;

        GameObject canv = GameObject.FindGameObjectWithTag("Canvas");
        GetComponent<Camera>().orthographicSize = (gvRatio * Global.MainCanvasWidth / 2)
            /canv.transform.localScale.y;
        Global.setGlobalConstants(GetComponent<Camera>());

/*        Debug.Log("main canvas width: " + Global.MainCanvasWidth + " canv local scales: " + canv.transform.localScale);*/


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
