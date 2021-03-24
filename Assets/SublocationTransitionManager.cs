using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// attached to the main Site prefab, manages the transitions of sublocations
/// </summary>
public class SublocationTransitionManager : MonoBehaviour
{
    public List<GameObject> sublocationGOs; //each should have a UIText child named "SublocationIndex", this is how we match sublocation GOs with their indices
    public Dictionary<int, GameObject> sublocations; //created with list above, which requires in-editor assignment

    [Inject(InjectFrom.Anywhere)] [HideInInspector]
    public Player p;

    [Inject(InjectFrom.Anywhere)]
    [HideInInspector]
    public CameraFollow camFollow;

    void Awake()
    {
        sublocations = new Dictionary<int, GameObject>();

        //readies the dictionary that matches sublocation gameObjects with their indices
        foreach (GameObject sublocationGO in sublocationGOs)
        {
            int subIndex = int.Parse(sublocationGO.transform.Find("SublocationIndex").GetComponent<Text>().text);
            sublocations.Add(subIndex, sublocationGO);

            foreach(TriggerSublocationTransition connection in sublocationGO.GetComponentsInChildren<TriggerSublocationTransition>())
            {
                connection.mySublocationIndex = subIndex;
            }
        }
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    /// <summary>
    /// transitions in between sublocations might require a different starting location depending on where the player was transitioning from (fromSublocation)
    /// 
    /// a Sublocation will have multiple "TransitionEnterSpot"s at times, and if we are coming from a connection point, then we check if this connection 
    /// point has a custom starting point (e.g. going from clinic to town will start the player in from of the town's clinic, as opposed to center of town)
    /// 
    /// if custom starting point not found, will use town's default starting point (also from a "TransitionEnterSpot" GO)
    /// 
    /// a fromSublocation of -1 means not coming from any other sublocation
    /// </summary>
    /// <param name="player"></param>
    /// <param name="transitionTo"></param>
    /// <param name="fromSublocation"></param>
    public void triggerSublocationTransition(int transitionTo, int fromSublocation)
    {
        GameObject player = p.gameObject;

        GameObject startSpot = null;

        //if coming from valid sublocation
        if (sublocations.ContainsKey(fromSublocation))
        {
            foreach(TriggerSublocationTransition connection in sublocations[transitionTo].transform.GetComponentsInChildren<TriggerSublocationTransition>())
            {
                if(connection.transitionToSublocationIndex == fromSublocation) //finds the connection for sublocation that we came from
                {
                    Transform enter = connection.transform.Find("TransitionEnterSpot");
                    if (enter != null) startSpot = enter.gameObject;
                    break;
                }
            }
        }

        if (startSpot == null) startSpot = sublocations[transitionTo].transform.Find("TransitionEnterSpot").gameObject;

        player.transform.position = startSpot.transform.position;
        camFollow.resetPosition();

    }
}
