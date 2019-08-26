using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSelect : MonoBehaviour
{

    public Text itemName, itemDescription;
    public Button useBtn, throwBtn;
    GameObject selectedItem;
    private GameObject prevSelected;

    void Start()
    {
        selectedItem = transform.Find("ItemMold").gameObject;

    }

    void Update()
    {
        
    }

    /**
     *  when clicked on itemBG (a button) it calls this function and passes itself as parameter
     * 
     */
    public void setItemSelected(GameObject itemBG)
    {
        Button bgBtn = itemBG.GetComponent<Button>();
        bgBtn.Select();

        Color col = itemBG.GetComponent<Image>().color;
        col.a = 230;
        itemBG.GetComponent<Image>().color = col; //"selected highlight"

        string id = itemBG.GetComponent<identifier>().id;

        if (prevSelected != null)
        {
            string prevId = prevSelected.GetComponent<identifier>().id;

            if (id != prevId)
            {
                Color c = prevSelected.GetComponent<Image>().color;
                c.a = 0.5f;
                prevSelected.GetComponent<Image>().color = c; //"selected highlight"
            }
        }

        prevSelected = itemBG;


        if (itemBG.transform.childCount > 0)
        {
            GameObject item = itemBG.transform.GetChild(0).gameObject; //the only child is the item itself
            ItemBehav itemBehav = item.GetComponent<ItemBehav>();
            itemName.text = itemBehav.itemName;
            itemDescription.text = itemBehav.description;
            float s = item.GetComponent<ItemBehav>().sizeScale;
            selectedItem.GetComponent<RectTransform>().localScale = new Vector3(s, s, s);
            selectedItem.GetComponent<SpriteRenderer>().sprite = item.GetComponent<SpriteRenderer>().sprite;
        }
        else { //empty bg
            itemName.text = "";
            itemDescription.text = "";
            selectedItem.GetComponent<SpriteRenderer>().sprite = null;
        }
    }


}
