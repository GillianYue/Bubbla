using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outing : MonoBehaviour
{

    public GameObject outing;
    [Inject(InjectFrom.Anywhere)]
    public Dialogue dialogue;

    public Vector3 startPos;

    void Start()
    {
        if (outing == null) outing = gameObject;

        startPos = outing.transform.position;
    }

    void Update()
    {
        
    }

    public void openMapUI()
    {
        if (outing != null)
        {
            Global.changePos(outing, 0, 0);

            outing.SetActive(true);
        }
    }

    public void closeMapUI()
    {
        if (outing != null)
        {
            Global.changePos(outing, (int)startPos.x, (int)startPos.y);

            outing.SetActive(false);
            dialogue.gameObject.SetActive(true);
        }
    }
}
