using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ListScroller : MonoBehaviour
{
    /// <summary>
    /// sets up a list object with instances of its list items. 
    /// 
    /// dataSpecificCallback(listItemGO, index) will be called after spawning the list items
    /// </summary>
    /// <param name="listRect"></param>
    /// <param name="listItemPrefab"></param>
    /// <param name="numItems"></param>
    /// <param name="dataSpecificCallback"></param>
    public static void setupList(GameObject listRect, GameObject listItemPrefab, int numItems, System.Action<GameObject, int> dataSpecificCallback)
    {
        setupListComponents(listRect, listItemPrefab, numItems);
        genListItems(listItemPrefab, numItems, listRect, dataSpecificCallback);
    }

    /// <summary>
    /// Sets the physical dimensions for all components involved in a list scroll.
    /// 
    /// NOTE: RectTransform here is not the rect for the actual scrollRect Gameobject, but the rect that will BE scrolled
    /// 
    /// assumes everything passed has a RectTransform
    /// </summary>
    static void setupListComponents(GameObject listRect, GameObject listItemPrefab, int numItems)
    {
        //first destroy previously existing items in list
        foreach(Transform prevChild in listRect.transform)
        {
            Destroy(prevChild.gameObject);
        }

        var prefHeight = Mathf.Abs(listItemPrefab.GetComponent<RectTransform>().rect.height);
        var rectHeight = listRect.GetComponent<RectTransform>().rect.height;


        float heightNeeded = numItems * prefHeight;

        //stretches rect so it's long enough to fit all the prefabs; this thing will then be scrolled within scrollRect
        if (heightNeeded > rectHeight)
        {
            listRect.GetComponent<RectTransform>().offsetMin = new Vector2(0, -1 * (heightNeeded - rectHeight));
            listRect.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);

        }//if doesn't need to be stretched, don't stretch rect
    }

    /// <summary>
    /// spawns the list items in the right positions. 
    /// 
    /// </summary>
    /// <param name="listItemPrefab"></param>
    /// <param name="numPrefs"></param>
    /// <param name="parent"> parent is the listContainer rect GO</param>
    /// <param name="dataSpecificCallback">dataSpecificCallback takes a GameObject as param and does the data setup 
    /// necessary for each generated list item based on index in list</param>
    static void genListItems(GameObject listItemPrefab, int numPrefs, GameObject parent, System.Action<GameObject, int> dataSpecificCallback)
    {

        var prefHeight = Mathf.Abs(listItemPrefab.GetComponent<RectTransform>().rect.height);

        for (int i = 0; i < numPrefs; i++)
        {
            GameObject item = genSingleListItem(i, listItemPrefab, prefHeight, parent);
            dataSpecificCallback(item, i);
        }
    }

    /// <summary>
    /// other data specific setup processes on the item will be done in the local scripts that call this function
    /// 
    /// IMPORTANT: rect transform config on list item prefab: Anchors Min (0,1) Max (1,1) Pivot (0.5,1) so height is fixed with adaptive width
    /// </summary>
    /// <param name="which"></param>
    /// <param name="listItemPrefab"></param>
    /// <param name="prefHeight"></param>
    /// <param name="parent"></param>
    static GameObject genSingleListItem(int which, GameObject listItemPrefab, float prefHeight, GameObject parent)
    {
        GameObject q = listItemPrefab; 
        q = Instantiate(q, parent.transform.position, Quaternion.identity) as GameObject;

        q.transform.SetParent(parent.transform);
        q.transform.localScale = new Vector3(1, 1, 1);

        //rectHeight at this point is properly set up, so can use as reference
        float rectHeight = parent.GetComponent<RectTransform>().rect.height;

        Global.setRectShape(q, 0, 0, -(which) * (prefHeight), -(which+1) * prefHeight);

        return q;
    }
}

