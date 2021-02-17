using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFitScreen : MonoBehaviour
{

    public RectTransform canvasToScale; 

    /**
     * this script fits the camera to the screen by adjusting its orthographic size.
     */
    void Awake()
    {

        //getting resolution of phone: see below commented line
        Vector2 gameViewSize = new Vector2(Screen.width, Screen.height);
/*        Debug.Log("gameView size: " + gameViewSize);*/

        float gvRatio = (float)Screen.height / (float)Screen.width;

        //GameObject canv = GameObject.FindGameObjectWithTag("Canvas");
        // GetComponent<Camera>().orthographicSize = (gvRatio * Global.MainCanvasWidth / 2)
        //     /canv.transform.localScale.y;
        GetComponent<Camera>().orthographicSize = Screen.width / Camera.main.aspect / 2;
            
        Global.setGlobalConstants(GetComponent<Camera>());

        if (canvasToScale)
        {
            float scale = gameViewSize.x / canvasToScale.sizeDelta.x;
            canvasToScale.transform.localScale = new Vector3(scale, scale, scale);

        }

/*        Debug.Log("main canvas width: " + Global.MainCanvasWidth + " canv local scales: " + canv.transform.localScale);*/


       // Vector2 cPos = canv.GetComponent<RectTransform>().localPosition;
        //Vector3 pos = new Vector3(cPos.x, cPos.y, -500); //camera is the farthest away, sees everything
//******* thus, in our game, the more negative (smaller) the z axis is, the higher priority of the visibility
      //  transform.position = pos;
        
    }


    void Update()
    {
        
    }



}
