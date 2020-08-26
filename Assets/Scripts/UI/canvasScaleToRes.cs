using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//not in use
public class canvasScaleToRes : MonoBehaviour
{
    /**
     * this script sets the dimensions of the main canvas
     * dimension is always the same: 640*1136, iphone 5
     * if resolution is different than default,
     * main camera will adjust orthographic size to show only 640*???, where the extra space
     * in ???-1136 will be covered by black space.    
     */
    void Start()
    {
        GameObject blackSpace = GameObject.FindWithTag("BlackSpace");
        Vector2 gameViewSize = UnityEditor.Handles.GetMainGameViewSize();
        Debug.Log("Game View resolution: " + gameViewSize);
        //setting black border background to exactly the size of resolution
        Global.setRectShape(blackSpace, gameViewSize.x, gameViewSize.y);

        //the main canvas' dimension is constant, independent of resolution.
        Global.setRectShape(gameObject, Global.MainCanvasWidth, 
            Global.MainCanvasHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
