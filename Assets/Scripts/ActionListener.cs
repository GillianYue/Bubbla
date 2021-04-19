using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// single action listener class
/// 
/// </summary>
public class ActionListener : MonoBehaviour
{
    public enum Listener { onStart, enterSite, enterArea, interactWithObject, interactWithCharacter, hitCollider };
    public Listener myListenerType;

    [Inject(InjectFrom.Anywhere)]
    public ActionListenerManager actionListenerManager;

    void Start()
    {
        if (!actionListenerManager) actionListenerManager = FindObjectOfType<ActionListenerManager>();
    }


    void Update()
    {
        
    }


}
