using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSublocationTransition : MonoBehaviour
{
    private bool collidePlayer = false;
    private GameObject myEnterIcon;
    public enum TransitionType { DIRECT, HINT };
    public TransitionType myTransitionType;

    public int transitionToSublocationIndex, mySublocationIndex; //will be assigned in SublocationTransitionManager on start

    [Inject(InjectFrom.Anywhere)]
    public SublocationTransitionManager sublocationTransitionManager;



    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            switch (myTransitionType)
            {
                case TransitionType.HINT:
                    //show hint for transition TODO

                    collidePlayer = true;
                    break;
                case TransitionType.DIRECT:
                    //do the transition
                    sublocationTransitionManager.triggerSublocationTransition(transitionToSublocationIndex, mySublocationIndex);
                    break;
            }

        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            switch (myTransitionType)
            {
                case TransitionType.HINT:
                    //get rid of hint for transition

                    collidePlayer = false;
                    break;
                case TransitionType.DIRECT:
                    break;
            }
        }
    }

    void OnMouseDown()
    {
        if (collidePlayer && myTransitionType == TransitionType.HINT)
        {
            //trigger sublocation transition
            sublocationTransitionManager.triggerSublocationTransition(transitionToSublocationIndex, mySublocationIndex);
        }
    }


}
