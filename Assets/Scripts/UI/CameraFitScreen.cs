using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFitScreen : MonoBehaviour
{

    public RectTransform canvasToScale;
    private Camera cam;

    /**
     * this script fits the camera to the screen by adjusting its orthographic size.
     */
    void Awake()
    {
        cam = GetComponent<Camera>();
        //getting resolution of phone: see below commented line
        Vector2 gameViewSize = new Vector2(Screen.width, Screen.height);


            
        Global.setGlobalConstants(cam);

        if (canvasToScale)
        {
            float scale = gameViewSize.x / canvasToScale.sizeDelta.x;
            canvasToScale.transform.localScale = new Vector3(scale, scale, scale);

        }


        if (cam.orthographic)
        {
            cam.orthographicSize = Screen.width / Camera.main.aspect / 2;
        }
        else
        {
            float canvasScale = canvasToScale.localScale.x;
            //angle between cam's forward and vector pointing from cam to top of target screen
            float dist = Mathf.Abs((canvasToScale.transform.position - cam.transform.position).z); 
            float fov = Mathf.Atan(canvasToScale.rect.height*canvasScale/2 / dist) * Mathf.Rad2Deg * 2f;
            cam.fieldOfView = fov;
        }

    }


    void Update()
    {
        
    }



}
