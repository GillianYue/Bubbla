using System.Collections;
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
    GameObject enemyMold;

    public bool enemyLoaderDone;


    void Start()
    {
        loadDone = new bool[1];

        loadEnemyMold();
        StartCoroutine(LoadScene.processCSV(loadDone, enemyCsv, setData)); //processCSV will call setData
        StartCoroutine(parseEnemyData());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        animator.runtimeAnimatorController = animatorOverrideController;

        if(S0_ANIM[eCode] != null)
        animatorOverrideController["State0"] = S0_ANIM[eCode];

        if (S1_ANIM[eCode] != null)
            animatorOverrideController["State1"] = S1_ANIM[eCode];

        if (S2_ANIM[eCode] != null)
            animatorOverrideController["State2"] = S2_ANIM[eCode];



    }

    void loadEnemyMold()
    {
        enemyMold = Instantiate(Resources.Load("EnemyMold", typeof(GameObject))) as GameObject;
        if (enemyMold == null) Debug.LogError("load EnemyMold failed");
    }

    IEnumerator parseEnemyData()
    {
        yield return new WaitUntil(() => loadDone[0]); //this would mean that data is ready to be parsed

        int numRows = data.GetLength(0);
        enemyCode = new int[numRows]; //num rows, int[] is for the entire column
        enemyName = new string[numRows];
        life = new int[numRows]; attack = new int[numRows];
        sizeScale = new float[numRows]; colliderScale = new float[numRows];
        s0_anim = new string[numRows]; s1_anim = new string[numRows]; s2_anim = new string[numRows];
        S0_ANIM = new AnimationClip[numRows];  S1_ANIM = new AnimationClip[numRows]; S2_ANIM = new AnimationClip[numRows]; 

        //skip row 0 because those are all descriptors
        for(int r = 1; r < numRows; r++)
        {
            enemyName[r-1] = data[1, r]; //r-1 is for such that enemyName[enemyCode] matches that with the data
            int.TryParse(data[2, r], out life[r - 1]);
            int.TryParse(data[3, r], out attack[r - 1]);
            float.TryParse(data[4, r], out sizeScale[r - 1]);
            float.TryParse(data[5, r], out colliderScale[r - 1]);
            s0_anim[r - 1] = data[6, r];
            s1_anim[r - 1] = data[7, r];
            s2_anim[r - 1] = data[8, r];

            //createEnemyAnimator("Assets/Prefabs/L1/Temp/" + enemyName[r - 1] + ".controller", r-1);
        }

        loadAnimationClips(S0_ANIM, S1_ANIM, S2_ANIM);
        yield return new WaitUntil(() => (S0_ANIM[S0_ANIM.Length - 1] != null)); //loaded

        enemyLoaderDone = true;

    }

    public void setData(string[,] d)
    {
        data = d;
    }

    void loadAnimationClips(AnimationClip[] s0, AnimationClip[] s1, AnimationClip[] s2)
    {

        //load the animation clips into the arrays
        for (int eCode = 0; eCode < s0.GetLength(0); eCode++)
        {
            if (!s0_anim[eCode].Equals(""))
            {
                GameObject tmpAnim0 = Resources.Load("Assets/Resources/Animation/" + s0_anim[eCode], typeof(GameObject)) as GameObject;

            if (tmpAnim0 != null)
            {
                Debug.LogError("Animation 0 NOT found");
            }
            else
            {
                Animation animation0 = tmpAnim0.GetComponent<Animation>();
                AnimationClip animanClip0 = animation0.clip;

                s0[eCode] = animanClip0;
            }
        }


            if (!s1_anim[eCode].Equals(""))
            {
                GameObject tmpAnim1 = Resources.Load("Assets/Resources/Animation/" + s1_anim[eCode], typeof(GameObject)) as GameObject;

            if (tmpAnim1 != null)
            {
                Debug.LogError("Animation 1 NOT found");
            }
            else
            {
                Animation animation1 = tmpAnim1.GetComponent<Animation>();
                AnimationClip animanClip1 = animation1.clip;

                s1[eCode] = animanClip1;
            }
        }

            if (!s2_anim[eCode].Equals(""))
            {
                GameObject tmpAnim2 = Resources.Load("Assets/Resources/Animation/" + s2_anim[eCode], typeof(GameObject)) as GameObject;

                if (tmpAnim2 != null)
                {
                    Debug.LogError("Animation NOT found");
                }
                else
                {
                    Animation animation2 = tmpAnim2.GetComponent<Animation>();
                    AnimationClip animanClip2 = animation2.clip;

                    s2[eCode] = animanClip2;
                }
            }
        }
    }

    //void createEnemyAnimator(string path, int eCode)
    //{
    //    // Creates the controller
    //    var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path);

    //    // Add parameters
    //    controller.AddParameter("State", AnimatorControllerParameterType.Int);

    //    // Add StateMachines
    //    var rootStateMachine = controller.layers[0].stateMachine;

    //    // Add States
    //    var state0 = rootStateMachine.AddState("state0");
    //    var state1 = rootStateMachine.AddState("state1");
    //    var state2 = rootStateMachine.AddState("state2");

    //    // Add Transitions

    //    var tr0 = rootStateMachine.AddAnyStateTransition(state0);
    //    tr0.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "State");
    //    tr0.duration = 0;

    //    var tr1 = rootStateMachine.AddAnyStateTransition(state1);
    //    tr1.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 1, "State");
    //    tr1.duration = 0;

    //    var tr2 = rootStateMachine.AddAnyStateTransition(state2);
    //    tr2.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 2, "State");
    //    tr2.duration = 0;

    //    rootStateMachine.AddEntryTransition(state0);

    //}
}
