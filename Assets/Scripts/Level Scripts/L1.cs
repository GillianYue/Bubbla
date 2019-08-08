using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L1 : LevelScript
{


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override IEnumerator levelScriptEvent(int code, bool[] done)
    {
        switch (code)
        {
            case 0:
                break;
            case 1:
                break;
            default:
                break;

        }

        yield return new WaitUntil(() => done[0]);
    }


}
