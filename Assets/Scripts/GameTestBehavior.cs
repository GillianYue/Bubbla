using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTestBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space")) //space for speeding time up
        {
            Time.timeScale = 5.0f;
        }

        if (Input.GetKeyUp("space")) //release to go back to normal time
        {
            Time.timeScale = 1.0f;
        }
    }
}
