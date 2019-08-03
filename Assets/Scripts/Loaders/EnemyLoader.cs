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

    void Start()
    {
        loadDone = new bool[1];

        StartCoroutine(LoadScene.processCSV(loadDone, enemyCsv, setData)); //processCSV will call setData
        StartCoroutine(parseEnemyData());

    }

    // Update is called once per frame
    void Update()
    {
        
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

            createEnemyAnimator("Assets/Prefabs/L1/Temp/" + enemyName[r - 1] + ".controller");
        }


        //TODO: parse
    }

    public void setData(string[,] d)
    {
        data = d;
    }

    static void createEnemyAnimator(string path)
    {
        // Creates the controller
        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(path);

        // Add parameters
        controller.AddParameter("State", AnimatorControllerParameterType.Int);

        // Add StateMachines
        var rootStateMachine = controller.layers[0].stateMachine;

        // Add States
        var state0 = rootStateMachine.AddState("state0");
        var state1 = rootStateMachine.AddState("state1");
        var state2 = rootStateMachine.AddState("state2");

        // Add Transitions

        var tr0 = rootStateMachine.AddAnyStateTransition(state0);
        tr0.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0, "State");
        tr0.duration = 0;

        var tr1 = rootStateMachine.AddAnyStateTransition(state1);
        tr1.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 1, "State");
        tr1.duration = 0;

        var tr2 = rootStateMachine.AddAnyStateTransition(state2);
        tr2.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 2, "State");
        tr2.duration = 0;

        rootStateMachine.AddEntryTransition(state0);

    }
}
