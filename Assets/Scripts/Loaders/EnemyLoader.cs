﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoader : MonoBehaviour
{
    private bool[] loadDone; //when loadDone[0] == true, loading is done for the csv file
    public TextAsset enemyCsv;
    private string[,] data; //double array that stores all info 

    int[] enemyCode; //this one's kinda stupid, cuz enemyCode[0] = 0, enemyCode[1] = 1...
    string[] enemyName;
    int[] life, attack;
    float[] sizeScale, colliderScale;
    string[] s0_anim, s1_anim, s2_anim; //path to animations
    AnimationClip[] S0_ANIM, S1_ANIM, S2_ANIM;
    int[] movement;
    int[] moveSpeed;

    GameObject enemyMold;

    public bool enemyLoaderDone; //this will be set to true once EnemyLoader is ready for usage


    void Start()
    {
        loadDone = new bool[1];

        loadEnemyMold(); //ready the mold prefab(s)
        StartCoroutine(LoadScene.processCSV(loadDone, enemyCsv, setData, false)); //processCSV will call setData
        StartCoroutine(parseEnemyData()); //data will be parsed into local type arrays for speedy data retrieval

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this function should only be called by EnemySpawner, as it deals with base level data
    public GameObject getEnemyInstance(int eCode)
    {
        if (enemyMold == null)
        {
            Debug.LogError("enemyMold is null... Can't create enemy instance");
            return null;
        }

        GameObject e = Instantiate(enemyMold) as GameObject; //duplicate
        Animator animator = e.GetComponent<Animator>();

        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);

        if(S0_ANIM[eCode] != null)
        animatorOverrideController["State0anim"] = S0_ANIM[eCode];

        if (S1_ANIM[eCode] != null)
            animatorOverrideController["State1anim"] = S1_ANIM[eCode];

        if (S2_ANIM[eCode] != null)
            animatorOverrideController["State2anim"] = S2_ANIM[eCode];

        animator.runtimeAnimatorController = animatorOverrideController;
        animator.Update(0.0f);

        e.name = enemyName[eCode];
        Enemy eScript = e.GetComponent<Enemy>();
        //NOTE: it's crucial that setLife is AFTER instantiation!
        eScript.setValues(life[eCode], attack[eCode]);
        eScript.setSizeScale(sizeScale[eCode]);
        eScript.setColliderScale(colliderScale[eCode]);

        EnemyMover mover = e.GetComponent<EnemyMover>();
        mover.enemyType = movement[eCode];
        mover.setSpeed(moveSpeed[eCode]);

        return e;
    }

    void loadEnemyMold()
    {
        enemyMold = Resources.Load("EnemyMold") as GameObject;
        if (enemyMold == null) Debug.LogError("load EnemyMold failed"); Debug.Log("lossy scale of e: " + enemyMold.transform.lossyScale);
    }

    IEnumerator parseEnemyData()
    {
        yield return new WaitUntil(() => loadDone[0]); //this would mean that data is ready to be parsed

        int numRows = data.GetLength(1); 
        enemyCode = new int[numRows-1]; //num rows, int[] is for the entire column
        enemyName = new string[numRows-1];
        life = new int[numRows-1]; attack = new int[numRows-1];
        sizeScale = new float[numRows-1]; colliderScale = new float[numRows-1];
        s0_anim = new string[numRows-1]; s1_anim = new string[numRows-1]; s2_anim = new string[numRows-1];
        S0_ANIM = new AnimationClip[numRows-1];  S1_ANIM = new AnimationClip[numRows-1]; S2_ANIM = new AnimationClip[numRows-1];
        movement = new int[numRows-1]; moveSpeed = new int[numRows - 1];

        //skip row 0 because those are all descriptors
        for(int r = 1; r < numRows; r++) //-1 because title row doesn't count
        {
            enemyName[r-1] = data[1, r]; //r-1 is for such that enemyName[enemyCode] matches that with the data
            int.TryParse(data[2, r], out life[r - 1]);
            int.TryParse(data[3, r], out attack[r - 1]);
            float.TryParse(data[4, r], out sizeScale[r - 1]);
            float.TryParse(data[5, r], out colliderScale[r - 1]);
            s0_anim[r - 1] = data[6, r];
            s1_anim[r - 1] = data[7, r];
            s2_anim[r - 1] = data[8, r];
            int.TryParse(data[9, r], out movement[r - 1]);
            int.TryParse(data[10, r], out moveSpeed[r - 1]);
        }

        loadAnimationClips(S0_ANIM, S1_ANIM, S2_ANIM);
        yield return new WaitUntil(() => {
            return (S0_ANIM[S0_ANIM.Length - 1] != null); }); //anim loaded, theoretically everything all set

        enemyLoaderDone = true;
        Debug.Log("EnemyLoader ready");
    }

    public void setData(string[,] d)
    {
        data = d;
    }

    private void loadAnimationClips(AnimationClip[] s0, AnimationClip[] s1, AnimationClip[] s2)
    {

        //load the animation clips into the arrays
        for (int eCode = 0; eCode < s0.GetLength(0); eCode++)
        {
            if (!s0_anim[eCode].Equals(""))
            {
                var tmpAnim0 = Resources.Load("Animation/"+s0_anim[eCode]) as AnimationClip;

            if (tmpAnim0 == null)
            {
                Debug.LogError("Animation 0 NOT found");
            }
            else
            {
                s0[eCode] = tmpAnim0;
            }
        }


            if (!s1_anim[eCode].Equals(""))
            {
                var tmpAnim1 = Resources.Load("Animation/" + s1_anim[eCode]) as AnimationClip;

                if (tmpAnim1 == null)
            {
                Debug.LogError("Animation 1 NOT found");
            }
            else
            {
                s1[eCode] = tmpAnim1;
            }
        }

            if (!s2_anim[eCode].Equals(""))
            {
                var tmpAnim2 = Resources.Load("Animation/" + s2_anim[eCode]) as AnimationClip;

                if (tmpAnim2 == null)
                {
                    Debug.LogError("Animation 2 NOT found");
                }
                else
                {
                    s2[eCode] = tmpAnim2;
                }
            }
        }
    }

}
