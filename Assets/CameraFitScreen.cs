using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFitScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject canv = GameObject.FindGameObjectWithTag("Canvas");
        GetComponent<Camera>().orthographicSize = (canv.
            GetComponent<RectTransform>().rect.height / 2) *
            canv.transform.localScale.y ;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
