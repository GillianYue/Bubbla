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

    private static float worldY;
    private static float worldX;

    public static Vector2 WorldUnitsInCamera;
    public static Vector2 WorldToPixelFactor;
    public static Vector2 PixelToWorldFactor;

    //is called in CameraFitScreen, which is called on start, attached to the camera
    public static void setGlobalConstants(float orthSize)
    {
        orthographicSize = orthSize;

           worldY = orthographicSize * 2;
           worldX = worldY * Screen.width / Screen.height;

         WorldUnitsInCamera = new Vector2(worldX, worldY);
        WorldToPixelFactor = new Vector2(Screen.width / worldX,
        Screen.height / worldY);
        PixelToWorldFactor = new Vector2(worldX / Screen.width,
        worldY / Screen.height);
}

    //~~~~~~~~~~~~~~~~~~~~~~~~~~CONSTANTS~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


    public static int Scene_To_Load;

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


    //~~~~~~~~~~~~~~~~~~~~~~~~~~UI logic~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


}

