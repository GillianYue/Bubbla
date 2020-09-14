using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
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
    /**
     * world is consistently 640*1136; screen depends on resolution. 
     * So for XR screen (828*1792), WTSfactor would allow world * WTSfactor = screen.
     * So WTSfactor in this case would be 1.3, 1.3    
     */
     public static Vector2 WTSfactor, STWfactor;
    public static Vector2 gameViewSize;
    public static int scaleRatio; 
    /**raw sprite presented on screen would be too small
    * as some sprites have low resolution when drawn (a 30 by 30, for example) but need
    * to be larger on screen. This ratio should be the same for most things for visual
    * consistency. This value will be set from player's scale once we enter game mode.   
    */

        public static Camera mainCamera;

    public static Dictionary<string, int> intVariables = new Dictionary<string, int>();
    public static Dictionary<string, bool> boolVariables = new Dictionary<string, bool>();
    public static Dictionary<string, string> stringVariables = new Dictionary<string, string>();

    //~~~~~~~~~~~~~~~~~~~~~~~~~~CONSTANTS~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~


    public static int Scene_To_Load;

    public static void setGlobalConstants(Camera mainCamera)
    {
        Global.mainCamera = mainCamera;
        Vector3 zero = mainCamera.WorldToScreenPoint(new Vector3(0,
    0,0));
        Vector3 one = mainCamera.WorldToScreenPoint(new Vector3(1,
           1,0));
        WTSfactor = new Vector2((one.x - zero.x), (one.y - zero.y));

        STWfactor = new Vector2((1.0f / WTSfactor.x), (1.0f /WTSfactor.y));
    
        gameViewSize = UnityEditor.Handles.GetMainGameViewSize();
        //Debug.Log("WTS: " + WTSfactor + " STW: " + STWfactor);
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~game play logic~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /**
     * checks if a 2-d touch is hitting a 2D obj of certain width and height
     * NOTE: for this to properly work, it's assumed that itemPos is the center of the sprite, not the border corners
     */
    public static bool touching(Vector2 touch, Vector2 itemPos,
        double itemwidth, double itemheight){
        if (touch.x > (itemPos.x - itemwidth/2) &&
            touch.x < (itemPos.x + itemwidth/2)
            && touch.y > (itemPos.y - itemheight/2)
            && touch.y < (itemPos.y + itemheight/2)) {
            return true;
        } else {
            return false;
        }
    }

    //same as above, only radius for balls
    public static bool touching(Vector2 touch, Vector2 itemPos,
        double radius){
        if (touch.x > (itemPos.x - radius) &&
            touch.x < (itemPos.x + radius)
            && touch.y > (itemPos.y - radius)
            && touch.y < (itemPos.y + radius)) {
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

    /// <summary>
    /// returns the distance between two points
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static float findVectorDist(Vector2 v1, Vector2 v2)
    {
        float d = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2));
        return d;
    }


    public static Vector2 ScreenToWorld(Vector2 mousePos)
    {
        Vector3 res = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 0));
        return new Vector2(res.x, res.y);
    }

    public static Vector3 ScreenToWorld(Vector2 mousePos, float z)
    {
        return mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, z)); 
    }

    public static Vector2 WorldToScreen(Vector3 pos)
    {
        Vector3 res = mainCamera.WorldToScreenPoint(pos);
        return new Vector2(res.x, res.y);

    }

    public static void changePos(GameObject e, int x, int y)
    {
        e.transform.position.Set(x, y, e.transform.position.z);
    }

    /*
     * moves a GO with Rigidbody2D towards a target destination while checking for raycast collisions
     */
    public static void nudgeTowards(GameObject e, int x, int y, float spd)
    {
        Rigidbody2D rb = e.GetComponent<Rigidbody2D>();

        float xDist = x - e.transform.position.x;
        float yDist = y - e.transform.position.y;
        float ratio = xDist / yDist;
        if (Math.Abs(xDist) > 1 && Math.Abs(yDist) > 1)
        {
            float dy = ((yDist > 0) ? 1 : -1) * (float)Math.Sqrt((Math.Pow(spd, 2) / (Math.Pow(ratio, 2) + 1)));
            float dx = ((xDist > 0) ? 1 : -1) * (float)Math.Sqrt(Math.Pow(spd, 2) - Math.Pow(dy, 2));

            Vector3 deltaPos = new Vector3(dx * 100, dy * 100, 0) * Time.deltaTime;
            double r = Math.Sqrt(Math.Pow(deltaPos.x, 2) + Math.Pow(deltaPos.y, 2));

            RaycastHit2D[] hits = new RaycastHit2D[5];
            ///////collision checking with raycast
            e.GetComponent<Collider2D>().Raycast(new Vector2(dx, dy), hits);

            if(hits[0].collider != null)
            {
                float halfSprite = e.GetComponent<Collider2D>().bounds.extents.y; // edge of sprite, not center of sprite counts
                if ((hits[0].distance - halfSprite) < r) //can not go as much as usual b/c of collider
                {
                    deltaPos = deltaPos.normalized * (hits[0].distance - halfSprite - 0.07f); //this is so that player never goes into objs
                }
            }
            //////end raycast check

            Vector3 newPos = e.transform.position + deltaPos;

            float xDistNew = x - newPos.x; float yDistNew = y - newPos.y;

        if ((xDistNew / xDist < 0) || (yDistNew / yDist < 0))
        //projected newPos went over mouse input point, the target direction has changed; don't go as far, only as much as needed
        {
            rb.MovePosition(new Vector2(x, y)); //not for collision
        }
        else
        {
            rb.MovePosition(newPos);
               
        }

        } //close enough
    }

    /// <summary>
    /// moves any GO towards target destination (one call to the IEnumerator)
    /// </summary>
    /// <returns></returns>
    public static IEnumerator moveTo(GameObject e, int x, int y, float spd, bool[] done)
    {
        float hyp = Mathf.Sqrt(Mathf.Pow(x - e.transform.position.x, 2) +  Mathf.Pow(y - e.transform.position.y, 2));

        Vector2 temp = new Vector2(x - e.transform.position.x, y - e.transform.position.y);
        temp = temp.normalized * spd;

        e.GetComponent<Rigidbody2D>().velocity = temp; 
        //Debug.Log("setting velo to " + e.GetComponent<Rigidbody2D>().velocity + " and pos" + e.transform.position);

        float timeTakes = (x - e.transform.position.x) / temp.x;

        yield return new WaitForSeconds(timeTakes);
        // Debug.Log("move done; supposedly " + x + " " + y + " and now at " + e.transform.position);
        e.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        done[0] = true;
    }

    /// <summary>
    /// moves any GO towards target destination (one call to the IEnumerator)
    /// </summary>
    public static IEnumerator moveTo(GameObject e, Vector2 dest, float spd, bool[] done)
    {
        return moveTo(e, (int)dest.x, (int)dest.y, spd, done);
    }

    public static IEnumerator moveToInSecs(GameObject e, int x, int y, float sec, bool[] done)
    {
        float xDist = x - e.transform.position.x;
        float yDist = y - e.transform.position.y;
        float dx = xDist / sec;
        float dy = yDist / sec;

        e.GetComponent<Rigidbody2D>().velocity = new Vector2(dx, dy);

        yield return new WaitForSeconds(sec);

        e.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0); //stops the GO at dest
        done[0] = true;
    }

    public static IEnumerator moveToInSecs(GameObject e, Vector2 dest, float sec, bool[] done)
    {
        return moveToInSecs(e, (int)dest.x, (int)dest.y, sec, done);
    }

    /// <summary>
    /// 
    /// checks whether a gameobject's position is within screen view
    /// 
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static bool gameObjectInView(Transform t)
    {
        Vector3 p = t.localPosition;
        float sw = Screen.width, sh = Screen.height;

        return (p.x >= -sw / 2 && p.x <= sw / 2 && p.y >= -sh / 2 && p.y <= sh / 2);
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
        RectTransform rt = DLGbg.GetComponent<RectTransform>();
        var ratio = rt.rect.height * rt.localScale.y / sSize.y;
        Vector3 scale = new Vector3(ratio, ratio, 1);
        scale *= 0.98f; //this is because DLG background isn't a perfect rect, so sprite should be a little smaller
        character.GetComponent<RectTransform>().localScale = scale;
    }

    /*
     * resizes sprite of a gameobj to the rect transform of this gameobj based on its x scale (ignoring y)
     * 
     * result: sprite will visually appear in the size of the *original* rect transform width; the rect transform's scale will be modified (both in x and y)
     */
    public static void resizeSpriteToRectX(GameObject obj)
    {
        Vector3 sSize = obj.GetComponent<SpriteRenderer>().sprite.bounds.size;
        RectTransform rt = obj.GetComponent<RectTransform>();
        var ratio = rt.rect.width * rt.lossyScale.x / sSize.x; //needs to find "true size" of rectTransform vs true sprite size

        Vector3 gs = obj.transform.parent.lossyScale; //the original global scale of the current obj's parent, in order to offset difference
        Vector3 scale = new Vector3(ratio / gs.x, ratio / gs.y, 1);
        obj.GetComponent<RectTransform>().localScale = scale;
    }

    /*
    * resizes sprite of GO sprHolder to the rect transform of rectObj based on its x scale (ignoring y)
    * 
    * although spr is usually from SpriteRenderer on sprHolder, there are exception cases: spr on child, spr on SpriteMask, etc. 
    * And so spr is separated from obj, which will be scaled based on ratio between rectObj dimensions and the sprite
    */
    public static void resizeSpriteToRectX(GameObject obj, Sprite spr, GameObject rectObj)
    {
        Vector3 sSize = spr.bounds.size;
        RectTransform rt = rectObj.GetComponent<RectTransform>();
        var ratio = rt.rect.width * rt.lossyScale.x / (sSize.x);
        // Debug.Log("rect width: " + rt.rect.width + " scale " + rt.lossyScale.x);

        Vector3 gs = obj.transform.parent.lossyScale; 
        //bc the ratios calculated are only valid for a sprite with no parent/lossy scale 1 in the world; we need to adjust for that
        Vector3 scale = new Vector3(ratio / gs.x, ratio / gs.y, 1);
        obj.GetComponent<RectTransform>().localScale = scale;
    }

    /*
 * resizes sprite of a gameobj to the rect transform of this gameobj based on its y scale (ignoring x)
 */
    public static void resizeSpriteToRectY(GameObject obj)
    {
        Vector3 sSize = obj.GetComponent<SpriteRenderer>().sprite.bounds.size;
        RectTransform rt = obj.GetComponent<RectTransform>();
        var ratio = rt.rect.height * rt.lossyScale.y / sSize.y;

        Vector3 gs = obj.transform.parent.lossyScale;
        Vector3 scale = new Vector3(ratio / gs.x, ratio / gs.y, 1);
        obj.GetComponent<RectTransform>().localScale = scale;
    }

    public static void resizeSpriteToRectY(GameObject obj, Sprite spr, GameObject rectObj)
    {
        Vector3 sSize = spr.bounds.size;
        RectTransform rt = rectObj.GetComponent<RectTransform>();
        var ratio = rt.rect.height * rt.lossyScale.y / sSize.y;

        Vector3 gs = obj.transform.parent.lossyScale; 
        Vector3 scale = new Vector3(ratio / gs.x, ratio / gs.y, 1);
        obj.GetComponent<RectTransform>().localScale = scale;
    }

    /*
    * resizes sprite of a gameobj to the rect transform of this gameobj completely based on rect dimensions
    */
    public static void resizeSpriteToRectXY(GameObject obj)
    {
        Vector3 sSize = obj.GetComponent<SpriteRenderer>().sprite.bounds.size;
        RectTransform rt = obj.GetComponent<RectTransform>();
        var ratioX = rt.rect.width * rt.lossyScale.x / sSize.x;
        var ratioY = rt.rect.height * rt.lossyScale.y / sSize.y;

        Vector3 gs = obj.transform.parent.lossyScale;
        Vector3 scale = new Vector3(ratioX / gs.x, ratioY / gs.y, 1);
        obj.GetComponent<RectTransform>().localScale = scale;
    }

    public static void resizeSpriteToRectXY(GameObject obj, Sprite spr, GameObject rectObj)
    {
        Vector3 sSize = spr.bounds.size;
        RectTransform rt = rectObj.GetComponent<RectTransform>();
        var ratioX = rt.rect.width * rt.lossyScale.x / sSize.x;
        var ratioY = rt.rect.height * rt.lossyScale.y / sSize.y;

        Vector3 gs = obj.transform.parent.lossyScale;
        Vector3 scale = new Vector3(ratioX / gs.x, ratioY / gs.y, 1);
        obj.GetComponent<RectTransform>().localScale = scale;
    }

    // center rect transform x
    public static void zeroX(GameObject obj)
    {
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
    public static void setToRectShape(GameObject obj, GameObject target)
    {
        setRectShape(obj, target.GetComponent<RectTransform>().rect.width,
           target.GetComponent<RectTransform>().rect.height);
    }

    /**
     * variant of the upper function, assumes the anchors are properly set (to parents)
     * does not modify the anchors in any way    
     *     
     * only requires the offset values    
     */
    public static void setRectShape(GameObject obj,
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
     public static void setRectShape(GameObject obj, float width, float height)
    {
        setRectTransformAnchorsIndependent(obj);
        RectTransform rt = obj.GetComponent<RectTransform>();

        rt.sizeDelta = new Vector2(width, height);

    }

    /// <summary>
    /// should be called when one side (x or y) of the rect is ideal, but the other needs to be adjusted based on given ratio
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="widthOverHeightRatio"></param>
    /// <param name="referX"></param>
    public static void setRectShape(GameObject obj, float widthOverHeightRatio, bool referX)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (referX)
        {
            float height = rt.rect.width / widthOverHeightRatio;
            setRectShape(obj, rt.rect.width, height);
        }
        else
        {
            float width = rt.rect.height * widthOverHeightRatio;
            setRectShape(obj, width, rt.rect.height);
        }
    }

    /**
     * sets the gameobject's rect transform width to target,
     * and scales GO's height to the same scale    
     * 
     * Note: with a GO that has spriteRenderer, the sprite will not be stretched with
     * width/height! Always scale instead of changing dimensions    
     */
     public static void copyRectTransformX(GameObject obj, GameObject target)
    {
        RectTransform rt = obj.GetComponent<RectTransform>();
        RectTransform trt = target.GetComponent<RectTransform>();

        float w = trt.rect.width;
        float ratio = (float)w / (float)rt.rect.width;

        setRectShape(obj, w, rt.rect.height * ratio);
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

    /**
     * centers the sprite of withSpr in the rect transform of centerIn
     *     
     * assumes withSpr is child of centerIn, and that the sprite GO has transform as opposed to rectTransform
     */
    public static void centerSpriteInGO(GameObject withSpr, GameObject centerIn)
    {
        SpriteRenderer sr = withSpr.GetComponent<SpriteRenderer>();
        RectTransform rt = withSpr.GetComponent<RectTransform>();
        RectTransform RT = centerIn.GetComponent<RectTransform>();

        float dX = 0, dY = 0; //local
        float targetWidth = RT.rect.width, targetHeight = RT.rect.height;
        float sizeScale = withSpr.GetComponent<ItemBehav>().sizeScale;
        float srWidth = sr.sprite.rect.width * sizeScale,  //sprite pic dimensions
            srHeight = sr.sprite.rect.height * sizeScale;
        float rtWidth = rt.rect.width * sizeScale, rtHeight = rt.rect.height * sizeScale; //withSpr rectTransform rect dimensions

        if (targetWidth > srWidth && targetHeight > srHeight) //sprite has to fit in bg box
        {
            dX = rtWidth / 2;
            dY = rtHeight / 2;
        }
        else
        {
            Debug.Log("sprite size bigger than bg box, resize needed");
        }

        //assuming anchors are all centered at pivot
        rt.offsetMax = new Vector2(dX, dY);
        rt.offsetMin = new Vector2(-dX, -dY);

    }

    public static void SetGlobalScale(Transform t, Vector3 globalScale)
    {
        t.localScale = Vector3.one;
        t.localScale = new Vector3(globalScale.x / t.lossyScale.x, globalScale.y / t.lossyScale.y, globalScale.z / t.lossyScale.z);
    }

    public static void SetGlobalScale(RectTransform t, Vector3 globalScale)
    {
        t.localScale = Vector3.one;
        t.localScale = new Vector3(globalScale.x / t.lossyScale.x, globalScale.y / t.lossyScale.y, globalScale.z / t.lossyScale.z);
    }


    //~~~~~~~~~~~~~~~~~~~~~~~~~~UI logic~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    //~~~~~~~~~~~~~~~~~~~~~~~~~~Code Helper Methods~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /**
      * Usage: StartCoroutine(Global.Chain(...))
      * For example:
      *     StartCoroutine(Global.Chain(
      *         Global.Do(() => Debug.Log("A")),
      *         Global.WaitForSeconds(2),
      *         Global.Do(() => Debug.Log("B"))));
      */
    public static IEnumerator Chain(MonoBehaviour g, params IEnumerator[] actions)
    {
        foreach (IEnumerator action in actions)
        {
            yield return g.StartCoroutine(action);
        }
    }

    /**
     * Usage: StartCoroutine(Global.DelaySeconds(action, delay))
     * For example:
     *     StartCoroutine(Global.DelaySeconds(
     *         () => DebugUtils.Log("2 seconds past"),
     *         2);
     */
    public static IEnumerator DelaySeconds(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    public static IEnumerator WaitUntilThenDo(Action action, bool condition)
    {
        yield return new WaitUntil(() => condition);
        action();
    }

    public static IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public static IEnumerator WaitUntil(bool condition)
    {
        yield return new WaitUntil(() => condition);
    }

    public static IEnumerator Do(Action action)
    {
        action();
        yield return 0;
    }

/// <summary>
/// helper function to copy the components of one GO to another; haven't tested out yet
/// </summary>
/// <returns></returns>
    public static Component CopyComponent(Component original, GameObject destination)
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        // Copied fields can be restricted with BindingFlags
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy;
    }

    public static void clearDictionaries()
    {
        intVariables = new Dictionary<string, int>();
        boolVariables = new Dictionary<string, bool>();
        stringVariables = new Dictionary<string, string>();
    }

    public static bool percentChance(int percentage)
    {
        float ran = UnityEngine.Random.Range(0f, 1f);
        return (ran <=  percentage / 100f) ;
    }

    public static bool percentChance(float percentage)
    {
        float ran = UnityEngine.Random.Range(0f, 1f);
        return (ran <= percentage);
    }

    /// <summary>
    /// takes in tuple of (value, erraticity (percentage))
    /// </summary>
    /// <param name="instance"></param>
    /// <returns>returns a calculated value applied with erraticity</returns>
    public static float getValueWithNoise((float, float) instance)
    {
        float val = instance.Item1, err = instance.Item2;
        float sign = UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f ? 1 : -1;
        float pickErr = UnityEngine.Random.Range(0.0f, err);
        val *= 1 + sign * pickErr;
        return val;
    }


    public static float getValueWithNoise(Vector2 instance)
    {
        return getValueWithNoise((instance.x, instance.y));
    }

    /// <summary>
    /// the y channel/error is percentage error
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public static float getValueWithNoise(float val, float noise)
    {
        return getValueWithNoise((val, noise));
    }

    //~~~~~~~~~~~~~~~~~~~~~~~~~~Code Helper Methods~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

}

