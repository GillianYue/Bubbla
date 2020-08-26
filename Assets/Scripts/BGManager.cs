using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGManager : MonoBehaviour
{

    public BGMover[] backgrounds;
    public GameObject fixedBG;
    [Inject(InjectFrom.Anywhere)]
    public ColorChanger colorChanger;
    bool hasInstance; //used to keep track whether there's a running instance; if not, starts anew, else pause/resume

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// depending on whether a previous instance is present, will either start bg related activities or pause/resume
    /// </summary>
    /// <param name="active"></param>
    public void setBackgroundsActive(bool active)
    {
        if (active)
        {
            activateAllbgMovers();
            if (colorChanger) {
                if (hasInstance) colorChanger.resumeProcess(); else colorChanger.startColorLoop(); 
            }
        }
        else
        {
            stopAllbgMovers();
            if (colorChanger) colorChanger.pauseProcess();
        }

        hasInstance = true;
    }

    public void resetEverything(bool continueMovement)
    {
        if (hasInstance) { 
            foreach (BGMover m in backgrounds)
            {
                m.revertToStartingPos();
                if (colorChanger) colorChanger.startColorLoop(); //resets
            }
        }
        setBackgroundsActive(continueMovement); //determines whether freezes or continue movement after reset
    }

    void stopAllbgMovers()
    {
        foreach (BGMover m in backgrounds)
        {
            m.stopBGScroll();
        }
    }

    void activateAllbgMovers()
    {
        foreach (BGMover m in backgrounds)
        {
            m.StartScrolling();
        }
    }
}
