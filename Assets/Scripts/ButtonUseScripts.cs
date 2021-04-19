using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// scripts generally useful for buttons of various purposes
/// </summary>
public class ButtonUseScripts : MonoBehaviour
{
    [Inject(InjectFrom.Anywhere)]
    public CustomEvents customEvents;

    void Start()
    {
        if (!customEvents) customEvents = FindObjectOfType<CustomEvents>();
    }

    public void setBoolToTrue(string boolName) {
        string[] prms = Global.makeParamString("0", boolName, "true");
        
        customEvents.customEvent(30, prms); 
    }

    public void closeItem(string itemIdentifier)
    {
        GameObject item = customEvents.findByIdentifier(itemIdentifier);
        StartCoroutine(closeItemCoroutine(item));
    }

    public IEnumerator closeItemCoroutine(GameObject item)
    {
        Vector3 orig = item.gameObject.transform.localScale;

        yield return Global.scaleToInSecs(item, 0.01f, 1.5f, new bool[0]);
        item.gameObject.SetActive(false);
        item.gameObject.transform.localScale = orig;
    }

}
