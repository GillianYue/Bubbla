using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    public bool gradualChange, loopColors;
    public Color[] colors; //non-inclusive of the starting color
    public SpriteRenderer sprite;
    public float stayTime, lerpTime; //colors will persist for stayTime, and take lerpTime to transition from one to another

    public Color initialColor;

    private bool pause;
    void Start()
    {
        if (initialColor.r == 0 && initialColor.g == 0 && initialColor.b == 0) initialColor = sprite.color;
        else sprite.color = initialColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// if called whilst another process was taking place, will end that process and restart from color 0 in the array
    /// </summary>
    public void startColorLoop()
    {
        StopAllCoroutines();
        sprite.color = initialColor;
        StartCoroutine(loopThroughColors());
    }

    public void pauseProcess()
    {
        pause = true;
    }

    public void resumeProcess()
    {
        pause = false;
    }

    //TODO pause mechanism not ideal
    IEnumerator loopThroughColors()
    {
        bool loop = true;
        while (loop)
        {
            for(int i=0; i<colors.Length; i++)
            {
                Color curr = sprite.color;
                yield return new WaitForSeconds(stayTime);

                if (gradualChange)
                    for (int p = 1; p <= 100; p++)
                    {
                        sprite.color = Color.Lerp(curr, colors[i], p / 100.0f);
                        yield return new WaitForSeconds(lerpTime / 100);
                    }
                else sprite.color = colors[i];

                if (pause) yield return new WaitUntil(() => !pause);
            }
            if (!loopColors) loop = false; //in effect loop runs only once if this is true
        }
    }
}
