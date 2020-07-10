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



    // supposed to be getting game view resolution; failed to work
	Vector2 getScreenWidthAndHeightFromEditorGameViewViaReflection()
	{
		//Taking game view using the method shown below	
		var gameView = GetMainGameView();
		var prop = gameView.GetType().GetProperty("currentGameViewSize", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		var gvsize = prop.GetValue(gameView, new object[0] { });
		var gvSizeType = gvsize.GetType();

		//I have 2 instance variable which this function sets:
		int ScreenHeight = (int)gvSizeType.GetProperty("height", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0] { });
		int ScreenWidth = (int)gvSizeType.GetProperty("width", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).GetValue(gvsize, new object[0] { });

        return new Vector2(ScreenHeight, ScreenWidth);
	}

	UnityEditor.EditorWindow GetMainGameView()
	{
		System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
		System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
		System.Object Res = GetMainGameView.Invoke(null, null);
		return (UnityEditor.EditorWindow)Res;
	}

}
