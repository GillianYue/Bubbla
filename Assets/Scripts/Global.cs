using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;

/**
 * contains some essential global functions that are commonly used
 */
public class Global : MonoBehaviour
{

    //~~~~~~~~~~~~~~~~~~~~~~~~~~CONSTANTS~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public static float orthographicSize; //entered manually by ME, here

    public static float aspectRatio = (640.0f / 1136.0f);
    public static float MainCanvasWidth = 640.0f;
    public static float MainCanvasHeight = 1136.0f;
    public static Vector2 WTSfactor, STWfactor;
    public static int scaleRatio; 
    /**raw sprite presented on screen would be too small
    * as some sprites have low resolution when drawn (a 30 by 30, for example) but need
    * to be larger on screen. This ratio should be the same for most things for visual
    * consistency. This value will be set from player's scale once we enter game mode.   
    */
    //~~~~~~~~~~~~~~~~~~~~~~~~~~CONSTANTS~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


    public static int Scene_To_Load;


    public static void setGlobalConstants(Camera mainCamera)
    {
        Vector3 zero = mainCamera.WorldToScreenPoint(new Vector3(0,
    0,0));
        Vector3 one = mainCamera.WorldToScreenPoint(new Vector3(1,
           1,0));
        WTSfactor = new Vector2((one.x - zero.x), (one.y - zero.y));

        Debug.Log("wtsFactor: " + WTSfactor);
        STWfactor = new Vector2((1 / WTSfactor.x), (1/WTSfactor.y));
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~game play logic~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /**
     * checks if a 2-d touch is hitting a 2D obj of certain width and height
     */
	public static bool touching(Vector2 touch, Vector2 itemPos,
		double itemwidth, double itemheight){
		if (touch.x > (itemPos.x) &&
		    touch.x < (itemPos.x + itemwidth)
			&& touch.y > (itemPos.y)
			&& touch.y < (itemPos.y + itemheight)) {
			return true;
		} else {
			return false;
		}
	}

    //same as above, only radius for balls
	public static bool touching(Vector2 touch, Vector2 itemPos,
		double radius){
		if (touch.x > (itemPos.x - radius / 2) &&
			touch.x < (itemPos.x + radius / 2)
			&& touch.y > (itemPos.y - radius / 2)
			&& touch.y < (itemPos.y + radius / 2)) {
			return true;
		} else {
			return false;
		}
	}

    //finds out how "close" two colors are for our game's purpose
    public static float find2ColorDist(Color c1, Color c2)
    {
        float d = Mathf.Sqrt(Mathf.Pow((c1.r - c2.r), 2) +
            Mathf.Pow((c1.g - c2.g), 2) + Mathf.Pow((c1.b - c2.b), 2));
        return d;
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~game play logic~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~





    //~~~~~~~~~~~~~~~~~~~~~~~~~~load scene logic~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public static IEnumerator LoadAsyncScene(int sceneNO)
	{
		// The Application loads the Scene in the background as the current Scene runs.
		// This is particularly good for creating loading screens.
		// You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
		// a sceneBuildIndex of 1 as shown in Build Settings.


		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNO);

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}

    //~~~~~~~~~~~~~~~~~~~~~~~~~~load scene logic~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~




    //~~~~~~~~~~~~~~~~~~~~~~~~~~UI logic~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    /*
 * resizes the sprite of a character to the right size in the dialogue box UI. 
 * will change the scale of the original rect transform.
 * Needs to pass in both the character gameObject and the DLG background box for profile sprite.
 * 
 * Here, we're scaling the character gameObject, as the sprite is pixel-to-pixel.
 * Prioritizes y-axis of the bg box for scaling, and scales that ratio in x to prevent distortion.    
 */
    public static void resizeSpriteToDLG(GameObject character, GameObject DLGbg)
    {
        Vector3 sSize = character.GetComponent<SpriteRenderer>().sprite.bounds.size;
        var ratio = DLGbg.GetComponent<RectTransform>().rect.height / sSize.y;
        Vector3 scale = new Vector3(ratio, ratio, 1);
        character.GetComponent<RectTransform>().localScale = scale;
    }

    /*
     * resizes sprite of a gameobj to the rect transform of this gameobj based on its x scale (ignoring y)
     */
    public static void resizeSpriteToRectX(GameObject obj)
    {
        Vector3 sSize = obj.GetComponent<SpriteRenderer>().sprite.bounds.size;
        // Debug.Log("sSize: " + sSize);
        var ratio = obj.GetComponent<RectTransform>().rect.width / sSize.x;
        //  Debug.Log("ratio: " + ratio);
        Vector3 scale = new Vector3(ratio, ratio, 1);
        obj.GetComponent<RectTransform>().localScale = scale;
        // Debug.Log("after set: " + obj.GetComponent<RectTransform>().localScale);
    }

    /*
 * resizes sprite of a gameobj to the rect transform of this gameobj based on its y scale (ignoring x)
 */
    public static void resizeSpriteToRectY(GameObject obj)
    {
        Vector3 sSize = obj.GetComponent<SpriteRenderer>().sprite.bounds.size;
        // Debug.Log("sSize: " + sSize);
        var ratio = obj.GetComponent<RectTransform>().rect.height / sSize.y;
        //  Debug.Log("ratio: " + ratio);
        Vector3 scale = new Vector3(ratio, ratio, 1);
        obj.GetComponent<RectTransform>().localScale = scale;
        // Debug.Log("after set: " + obj.GetComponent<RectTransform>().localScale);
    }

    /*
 * resizes sprite of a gameobj to the rect transform of this gameobj completely based on rect dimensions
 */
    public static void resizeSpriteToRectXY(GameObject obj)
    {
        Vector3 sSize = obj.GetComponent<SpriteRenderer>().sprite.bounds.size;
        // Debug.Log("sSize: " + sSize);
        var ratioX = obj.GetComponent<RectTransform>().rect.width / sSize.x;
        var ratioY = obj.GetComponent<RectTransform>().rect.height / sSize.y;
        //  Debug.Log("ratio: " + ratio);
        Vector3 scale = new Vector3(ratioX, ratioY, 1);
        obj.GetComponent<RectTransform>().localScale = scale;
        // Debug.Log("after set: " + obj.GetComponent<RectTransform>().localScale);
    }

    // center rect transform x
    public static void centerX(GameObject obj)
    {
        // Debug.Log(obj.GetComponent<RectTransform>().localPosition);
        Vector3 n = obj.GetComponent<RectTransform>().localPosition;
        n.x = 0;
        obj.GetComponent<RectTransform>().localPosition = n;
    }

    /**
     * rect transform setter
     * 
     * this method sets one rect transform to be the exact same shape as the other
     * GO's rect transform. We deal with this simply by setting the anchors to all be 
     * 0 (so fixed size), then forcefully set the width and height of the rect. 
     * This function does not move the obj to WHERE the other GO is.     
     *     
     * OFFSET MAX def: The offset of the upper right corner of the rectangle 
     * relative to the upper right anchor.    
     *     
     * OFFSET MIN def: The offset of the lower left corner of the rectangle 
     * relative to the lower left anchor.    
     * 
     * ANCHOR MAX def: The normalized position in the parent RectTransform
     * that the upper right corner is anchored to. this is the 0 to 1 thing
     * 
     * ANCHOR MIN def: The normalized position in the parent RectTransform 
     * that the lower left corner is anchored to. also the 0 to 1 thing
     * 
     * SIZEDELTA def: If the anchors are together, sizeDelta is the same as size.
     * If the anchors are in each of the four corners of the parent, the sizeDelta 
     * is how much bigger or smaller the rectangle is compared to its parent.    
     * 
     */
    public static void setToRectTransform(GameObject obj, GameObject target)
    {
        setRectTransform(obj, target.GetComponent<RectTransform>().rect.width,
           target.GetComponent<RectTransform>().rect.height);
    }

    /**
     * variant of the upper function, assumes the anchors are properly set (to parents)
     * does not modify the anchors in any way    
     *     
     * only requires the offset values    
     */
    public static void setRectTransform(GameObject obj,
   float leftOffset, float rightOffset, float upperOffset, float lowerOffset)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();

        Vector2 upperRight = new Vector2(rightOffset, upperOffset);
        Vector2 lowerLeft = new Vector2(leftOffset, lowerOffset);
        rt.offsetMax = upperRight;
        rt.offsetMin = lowerLeft;
    }

    /**
     * You know the dimensions of the rect you want it to be, and voila, here's
     * a function just for that.
     * 
     * This function will set the anchors to be 0-0, 0-0 to keep the sizes fixed.     
     */
     public static void setRectTransform(GameObject obj, float width, float height)
    {
        setRectTransformAnchorsIndependent(obj);
        RectTransform rt = obj.GetComponent<RectTransform>();

        rt.sizeDelta = new Vector2(width, height);

    }

    /**
     * sets the gameobject's rect transform width to target,
     * and scales GO's height to the same scale    
     * 
     * Note: with a GO that has spriteRenderer, the sprite will not be stretched with
     * width/height! Always scale instead of changing dimensions    
     */
     public static void setRectTransformX(GameObject obj, GameObject target)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        RectTransform trt = target.GetComponent<RectTransform>();

        float w = trt.rect.width;
        float ratio = (float)w / (float)rt.rect.width;

        setRectTransform(obj, w, rt.rect.height * ratio);
    }

    /**
     * so that it's always fixed size (width and height)
     */
    public static void setRectTransformAnchorsIndependent(GameObject obj)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();

        //setting the anchors
        rt.anchorMax = new Vector2(0, 0);
        rt.anchorMin = new Vector2(0, 0);
    }

    /**
 * so that it's always 100% dependent on parent size
 */
    public static void setRectTransformAnchorsDependent(GameObject obj)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();

        //setting the anchors
        rt.anchorMax = new Vector2(1, 1);
        rt.anchorMin = new Vector2(0, 0);
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~UI logic~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


}

