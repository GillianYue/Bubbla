using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeOnStart : MonoBehaviour
{
    public enum VisualType { Image, Sprite, SpriteMask }
    public VisualType type;
    /// <summary>
    /// Sprites won't change size as the dimension of the GO transform changes, so to achieve flexible size based on rect transform we scale the 
    /// sprite so that its size becomes the original rect transform dimension (though after scaling the rect transform dimension will be messed up
    /// </summary>
    public enum SpriteOptions { scaleToRectX, scaleToRectY, scaleToRectBothWays };
    public SpriteOptions spriteOptions;
    public SpriteRenderer spriteSource; //sprite to be compared to, not necessarily on the GO this script is attached to
    public bool useMyRectTransform;
    public RectTransform spriteReferenceRect;
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
    public RectTransform imageReferenceRect;
    public float percentInRefRect;

    public SpriteMask spriteMaskSource;


    void Start()
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
                        Global.resizeSpriteToRectX(gameObject, sprite, spriteReferenceRect.gameObject);
                    break;
                case SpriteOptions.scaleToRectY:
                    if (useMyRectTransform)
                        Global.resizeSpriteToRectY(gameObject);
                    else
                        Global.resizeSpriteToRectY(gameObject, sprite, spriteReferenceRect.gameObject);
                    break;
                case SpriteOptions.scaleToRectBothWays:
                    if (useMyRectTransform)
                        Global.resizeSpriteToRectXY(gameObject);
                    else
                        Global.resizeSpriteToRectXY(gameObject, sprite, spriteReferenceRect.gameObject);
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
                    Global.setRectShape(gameObject, imageReferenceRect.rect.width * percentInRefRect, GetComponent<RectTransform>().rect.height);
                    break;
                case ImageOptions.changeYWithPercentage:
                    Global.setRectShape(gameObject, GetComponent<RectTransform>().rect.width, imageReferenceRect.rect.height * percentInRefRect);
                    break;
            }
        }
    }
       

    // Update is called once per frame
    void Update()
    {
        
    }
}
