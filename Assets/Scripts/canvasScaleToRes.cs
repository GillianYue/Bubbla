using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class canvasScaleToRes : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector2 gameViewSize = UnityEditor.Handles.GetMainGameViewSize();
        Debug.Log("Game View resolution: " + gameViewSize);
        Global.setRectTransform(gameObject, gameViewSize.x, gameViewSize.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
