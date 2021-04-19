using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLoader : Loader
{

    private bool[] loadDone; //when loadDone[0] == true, loading is done for the csv file
    public TextAsset characterCsv;
    private string[,] data; //double array that stores all info 

    int[] characterCode; 
    ArrayList cName;
    public List<List<string>> baseStateAnimationClipNames, Part1AnimationClipNames, Part2AnimationClipNames; //holds transition conditions for each character
    string[] pathName; //animators, voices will have the same local path (e.g. CaptainBuns) for files
    RuntimeAnimatorController[] cAnimators;
    AudioClip[] voiceClips;
    public Color[] bgColor; 

    GameObject cMold;

    private bool characterLoaderDone; //this will be set to true once is ready for usage


    protected override void Start()
    {
        base.Start();
        loadDone = new bool[1];

        loadCharacterMold(); //ready the mold prefab(s)
        StartCoroutine(LoadScene.processCSV(loadDone, characterCsv, setData, false)); //processCSV will call setData
        StartCoroutine(parseCharacterData()); //data will be parsed into local type arrays for speedy data retrieval

    }

    void Update()
    {

    }

    /// <summary>
    /// overrides parent
    /// </summary>
    /// <returns></returns>
    public override bool isLoadDone()
    {
        return characterLoaderDone;
    }
    void loadCharacterMold()
    {
        cMold = Resources.Load("Prefabs/cMold") as GameObject;
        if (cMold == null) Debug.LogError("load CharacterMold failed");
    }

    IEnumerator parseCharacterData()
    {
        yield return new WaitUntil(() => loadDone[0]); //this would mean that data is ready to be parsed

        int numRows = 0;
        for(int i=0; i< data.GetLength(1); i++) //we do this because we only want to record valid entries (number coded)
        {
            if (!data[1, i].Equals(""))
            {
                numRows++; //valid row++
            }
            else
            {
                break;
            }
        }

        characterCode = new int[numRows - 1]; //num rows, int[] is for the entire column
        cName = new ArrayList();
        baseStateAnimationClipNames = new List<List<string>>();
        Part1AnimationClipNames = new List<List<string>>();
        Part2AnimationClipNames = new List<List<string>>();

        //bool1Name = new string[numRows - 1]; bool2Name = new string[numRows - 1];
        pathName = new string[numRows - 1]; 
        cAnimators = new RuntimeAnimatorController[numRows - 1];
        voiceClips = new AudioClip[numRows - 1];
        bgColor = new Color[numRows - 1];

        //skip row 0 because those are all descriptors
        for (int r = 1; r < numRows; r++) 
        {
            cName.Add(data[1, r]);

            //////will change to string[] with actual size afterwards
            baseStateAnimationClipNames.Add(new List<string>(new string[5])); //only a holder
            Part1AnimationClipNames.Add(new List<string>(new string[5]));
            Part2AnimationClipNames.Add(new List<string>(new string[5]));

            pathName[r - 1] = data[4, r]; //col 4 is file path for animator
            int R, G, B;
            int.TryParse(data[5, r], out R);
            int.TryParse(data[6, r], out G);
            int.TryParse(data[7, r], out B);
            bgColor[r - 1] = new Color(R/255f, G/255f, B/255f);
        }

        
        loadVoices(voiceClips);
        yield return loadAnimators(cAnimators); //waits until all animators are loaded

        characterLoaderDone = true;
        Debug.Log("CharacterLoader ready");
    }

    public void setData(string[,] d)
    {
        data = d;
    }

    //given data for a character's animator transition conitions, set variables accordingly
    //in-game state transitions will depend on those variables
    public void setStateMachineData(string[,] d, int cCode)
    {

        for (int i = 1; i < d.GetLength(1); i++) 
        {
            string animClipName = d[0, i];
            int stateNum = -1;

            if (d[1, i] != "") //clip belongs to BASE layer
            {
                //print("part 0" + animClipName + " " + cCode);
                int.TryParse(d[1, i], out stateNum);
                checkExpandList(baseStateAnimationClipNames[cCode], stateNum, "");
                baseStateAnimationClipNames[cCode][stateNum] = animClipName;
            } else if (d[2, i] != "") //PART 1 layer
            {
                //print("part 1" + animClipName + " " + cCode);
                int.TryParse(d[2, i], out stateNum);
                checkExpandList(Part1AnimationClipNames[cCode], stateNum, "");
                Part1AnimationClipNames[cCode][stateNum] = animClipName;
                //print("part 1 for char " + cCode + " state " + stateNum + " recorded: " + animClipName);
            } else if (d[3, i] != "") //PART 2 layer
            {
                //print("part 2" + animClipName + " " + cCode);
                int.TryParse(d[3, i], out stateNum);
                checkExpandList(Part2AnimationClipNames[cCode], stateNum, "");
                Part2AnimationClipNames[cCode][stateNum] = animClipName;
            }
            else{
                Debug.LogError(d[0, i] + " no proper transition condition found");
            }
        }
    }

    /// <summary>
    /// expands a list to a certain size (fills new spaces with default value)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="insertIndex"></param>
    /// <param name="defaultVal"></param>
    private void checkExpandList<T>(List<T> list, int insertIndex, T defaultVal)
    {
        if (insertIndex >= list.Count)
        {
            int slotsNeeded = Mathf.Max(insertIndex - list.Count + 1, 5);
            for (int s = 0; s < slotsNeeded; s++)
            {
                list.Add(defaultVal);
            }
        }
    }

    private IEnumerator loadAnimators(RuntimeAnimatorController[] cAnim)
    {

        List<Coroutine> coroutines = new List<Coroutine>();

        //load the animation clips into the arrays
        for (int cCode = 0; cCode < cAnim.GetLength(0); cCode++)
        {
            if (!pathName[cCode].Equals(""))
            {
                //load in animator
                var amt = Resources.Load("Animator/" + pathName[cCode]) as RuntimeAnimatorController;
                var transitionConditionCSV = Resources.Load("Animator/StateMachineCSV/" + pathName[cCode]+"Animator") as TextAsset;

                if (amt == null)
                {
                    Debug.Log("Animator for c"+cCode+" NOT found");
                }
                else
                {

                    cAnim[cCode] = amt;

                    bool[] csvLoadDone = new bool[1];
                    //sets transition data for each character based on csv
                    if (transitionConditionCSV != null)
                    {
                        Coroutine c = StartCoroutine(LoadScene.processCSV(csvLoadDone, transitionConditionCSV, (string[,] d) => setStateMachineData(d, cCode), false));
                        coroutines.Add(c);

                    }
                    else
                    {
                        Debug.Log("Transition Condition for c" + cCode + " NOT found, should have a " + "Animator/StateMachineCSV/" + pathName[cCode] + "Animator");
                    }
                }
            }
        }

        //will start the coroutines in parallel and wait for all to finish 
        foreach (Coroutine c in coroutines)
        {
            yield return c;
        }

    }


    private void loadVoices(AudioClip[] voices)
    {

        for (int cCode = 0; cCode < voices.GetLength(0); cCode++)
        {

                var v = Resources.Load("Voices/" + pathName[cCode]) as AudioClip;

                if (v == null)
                {
                    Debug.LogError("Voice for c" + cCode + " NOT found");
                }
                else
                {
                    voices[cCode] = v;
                }
        }

    }

    public RuntimeAnimatorController getAnimatorByName(string name)
    {
        int index = cName.IndexOf(name);
        return getAnimatorByIndex(index);
    }

    public RuntimeAnimatorController getAnimatorByIndex(int index)
    {
        if (index != -1)
        {
            return cAnimators[index];
        }
        else
        {
            Debug.LogError("no animator for character " + index + " found");
            return null;
        }
    }

    public AudioClip getVoiceByName(string name)
    {
        int index = cName.IndexOf(name);
        return getVoiceByIndex(index);
    }

    public AudioClip getVoiceByIndex(int index)
    {
        if (index != -1)
        {
            return voiceClips[index];
        }
        else
        {
            Debug.LogError("no voice for character " + index + " found");
            return null;
        }
    }

    //TODO can optimize (maybe with a dictionary)
    public int getIndex(string name)
    {
        int index = cName.IndexOf(name);
        return index;
    }

    public string getName(int index) { return (string)cName[index]; }

    public Color getColorByIndex(int index)
    {
        return bgColor[index];
    }

    /// <summary>
    /// plays base layer animation of a certain character
    /// </summary>
    /// <param name="a"></param>
    /// <param name="cCode"></param>
    /// <param name="state"></param>
    public void playBaseAnimation(Animator a, int cCode, int state)
    {
        //print("playing base anim for character " + cCode + " with state " + state);
        if (baseStateAnimationClipNames[cCode] != null && baseStateAnimationClipNames[cCode][state] != null)
        {
            a.Play(baseStateAnimationClipNames[cCode][state]);
            print("playing " + baseStateAnimationClipNames[cCode][state]);
        }
        else
        {
            Debug.Log("character " + cCode + " does not have base state " + state + ", "+ baseStateAnimationClipNames[cCode].Count);
        }
    }

    /// <summary>
    /// plays part I layer animation of a certain character
    /// </summary>
    /// <param name="a"></param>
    /// <param name="cCode"></param>
    /// <param name="state"></param>
    public void playPart1Animation(Animator a, int cCode, int state)
    {
        //int("ccode " + cCode);
        if (Part1AnimationClipNames[cCode] != null && Part1AnimationClipNames[cCode][state] != null)
        {
            a.Play(Part1AnimationClipNames[cCode][state]);
        }
        else
        {
            Debug.Log("character " + cCode + " does not have part 1 state " + state);
        }
    }


    /// <summary>
    /// plays part II layer animation of a certain character
    /// </summary>
    /// <param name="a"></param>
    /// <param name="cCode"></param>
    /// <param name="state"></param>
    public void playPart2Animation(Animator a, int cCode, int state)
    {
        if (Part2AnimationClipNames[cCode] != null && Part2AnimationClipNames[cCode][state] != null)
        {
            a.Play(Part2AnimationClipNames[cCode][state]);
        }
        else
        {
            Debug.Log("character " + cCode + " does not have part 2 state " + state);
        }
    }
}

