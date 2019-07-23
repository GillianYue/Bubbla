using UnityEngine;
using System.Collections;

public class LevelScript : MonoBehaviour {

    protected GameControl gameControl;
    protected GameFlow gameFlow;
    protected EnemySpawner eSpawner;
    protected PaintballSpawner pSpawner;

	// Use this for initialization
	void Start () {
        GameObject GC = GameObject.FindWithTag("GameController");
        gameControl = GC.GetComponent<GameControl>();
        gameFlow = GC.GetComponent<GameFlow>();
        eSpawner = GC.GetComponent<EnemySpawner>();
        pSpawner = GC.GetComponent<PaintballSpawner>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    /**
     * the method for the actual levelScripts to override
     * 
     * contains a switch statement for different custom events
     * the custom events are defined locally in the level scripts    
     */
    public virtual void customEvent(int index) {
        /**
         * e.g.
         * 
         * switch (index):
         * case 1:
         *    StartCoroutine(MakeBunnyPranceUpAndDown());
         *    break;
         * default: break;
         *         
         */
    }

}
