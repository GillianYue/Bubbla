using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteToRectOnStart : MonoBehaviour
{
    public enum Mode { X, Y, BothWays };
    public Mode how;

    // Start is called before the first frame update
    void Start()
    {
        switch (how)
        {
            case Mode.X:
                Global.resizeSpriteToRectX(gameObject);
                break;
            case Mode.Y:
                Global.resizeSpriteToRectY(gameObject);
                break;
            case Mode.BothWays:
                Global.resizeSpriteToRectXY(gameObject);
                break;
            default:
                Global.resizeSpriteToRectXY(gameObject);
                break;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
