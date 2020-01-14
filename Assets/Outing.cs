using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outing : MonoBehaviour
{

    public GameObject outing;
    [Inject(InjectFrom.Anywhere)]
    public Dialogue dialogue;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openMapUI()
    {
        if (outing != null)
        {
            outing.SetActive(true);
            outing.transform.position = new Vector3(0, 0, -30);
        }
    }

    public void closeMapUI()
    {
        if (outing != null)
        {
            outing.SetActive(false);
            dialogue.gameObject.SetActive(true);
        }
    }
}
