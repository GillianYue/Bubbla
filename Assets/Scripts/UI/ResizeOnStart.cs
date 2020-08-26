using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeOnStart : MonoBehaviour
{
    public enum VisualType { Image, Sprite, SpriteMask, BoxCollider2D }
    public VisualType type;
    public RectTransform referenceRect;


    /// <summary>
    /// Sprites won't change size as the dimension of the GO transform changes, so to achieve flexible size based on rect transform we scale the 
    /// sprite so that its size becomes the original rect transform dimension (though after scaling the rect transform dimension will be messed up
    /// </summary>
    public enum SpriteOptions { scaleToRectX, scaleToRectY, scaleToRectBothWays };
    public SpriteOptions spriteOptions;
    public SpriteRenderer spriteSource; //sprite to be compared to, not necessarily on the GO this script is attached to
    public bool useMyRectTransform;

    /// <summary>
    /// Images will scale accordingly to rect transforms. However, there are times when we wish to preserve the ratio of an image (e.g. UI box to preserve
    /// a certain shape)
    /// 
    /// changeX/YWithRatio will require the input of widthOverHeightRatio so that width or height can be calculated accordingly (assuming the other axis is properly set)
    /// changeX/YWithPercentage will require another RectTransform and a percentage (e.g. our image should be 10% vertically of the given rect)
    /// </summary>
    public enum ImageOptions { changeXWithRatio, changeYWithRatio, changeXWithPercentage, changeYWithPercentage };
    public ImageOptions imageOptions;
    public float widthOverHeightRatio;
    public float percentInRefRect;

    public SpriteMask spriteMaskSource;

    private bool resizeDone = false;


    void Start()
    {
        StartCoroutine(waitForCanvasScaler());

    }

    /// <summary>
    /// the canvas scale will only be correct after canvas scaler has taken effect; since this script depends on the correct scaling of canvases,
    /// it waits for a short amount of time on start for canvas scaler to finish
    /// 
    /// </summary>
    /// <returns></returns>
    IEnumerator waitForCanvasScaler()
    {
        yield return new WaitForSeconds(0.5f);
        resizeOnStart();
    }

    void resizeOnStart()
    {

        if (type == VisualType.Sprite || type == VisualType.SpriteMask)
        {
            Sprite sprite = (type == VisualType.Sprite) ? spriteSource.sprite : spriteMaskSource.sprite;

            switch (spriteOptions)
            {
                case SpriteOptions.scaleToRectX:
                    if (useMyRectTransform)
                        Global.resizeSpriteToRectX(gameObject);
                    else
                        Global.resizeSpriteToRectX(gameObject, sprite, referenceRect.gameObject);
                    break;
                case SpriteOptions.scaleToRectY:
                    if (useMyRectTransform)
                        Global.resizeSpriteToRectY(gameObject);
                    else
                        Global.resizeSpriteToRectY(gameObject, sprite, referenceRect.gameObject);
                    break;
                case SpriteOptions.scaleToRectBothWays:
                    if (useMyRectTransform)
                        Global.resizeSpriteToRectXY(gameObject);
                    else
                        Global.resizeSpriteToRectXY(gameObject, sprite, referenceRect.gameObject);
                    break;
            }

        }
        else if (type == VisualType.Image)
        {
            switch (imageOptions)
            {
                case ImageOptions.changeXWithRatio:
                    Global.setRectShape(gameObject, widthOverHeightRatio, true);
                    break;
                case ImageOptions.changeYWithRatio:
                    Global.setRectShape(gameObject, widthOverHeightRatio, false);
                    break;
                case ImageOptions.changeXWithPercentage:
                    Global.setRectShape(gameObject, referenceRect.rect.width * percentInRefRect, GetComponent<RectTransform>().rect.height);
                    break;
                case ImageOptions.changeYWithPercentage:
                    Global.setRectShape(gameObject, GetComponent<RectTransform>().rect.width, referenceRect.rect.height * percentInRefRect);
                    break;
            }
        }
        else if (type == VisualType.BoxCollider2D)
        {
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            box.size = new Vector2(referenceRect.rect.size.x * referenceRect.localScale.x,
                referenceRect.rect.size.y * referenceRect.localScale.y);
        }

        resizeDone = true;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public bool checkResizeDone()
    {
        return resizeDone;
    }
}
