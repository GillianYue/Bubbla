using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/// <summary>
/// loads and stores all Quests
/// </summary>
public class QuestLoader : Loader
{

    public TextAsset questCsv; //contains info for quests
    private string[,] questsData;
    public int numOfQuests;

    private bool[] loadDone;
    private bool questLoaderDone;
    private Quest[] allQuests;

    [Inject(InjectFrom.Anywhere)]
    public GameFlow gameFlow;

    [Inject(InjectFrom.Anywhere)]
    public SaveLoad saveLoad;

    [Inject(InjectFrom.Anywhere)]
    public QuestConditionManager questConditionManager;
    [Inject(InjectFrom.Anywhere)]
    public ActionListenerManager actionListenerManager;


    protected override void Start()
    {
        base.Start();

        loadDone = new bool[1];
        questLoaderDone = false;

        StartCoroutine(startLoad());
    }

    void Update()
    {
        
    }

    IEnumerator startLoad()
    {
        yield return StartCoroutine(parseQuestsData()); //will set the correct numOfQuests


    }

    /// <summary>
    /// overrides parent
    /// </summary>
    /// <returns></returns>
    public override bool isLoadDone()
    {
        return questLoaderDone;
    }

    public void setQuestsData(string[,] d)
    {
        questsData = d;
    }

    //returns num of all quests available in loader
    public int getNumQuests()
    {
        return numOfQuests; 
    }

    /// <summary>
    /// parses data for all quest entries (general quests load, not setting up any of them)
    /// </summary>
    /// <returns></returns>
    IEnumerator parseQuestsData()
    {
        //data[col,1] will be the first quest (tho in excel visually it's at line 2)
        yield return LoadScene.processCSV(loadDone, questCsv, setQuestsData, false);

        numOfQuests = questsData.GetLength(1) - 1; //exclude title row; data[col, 1_to_numOfQuests] are the quests

        allQuests = new Quest[numOfQuests]; //b/c 0 doesn't count; the first quest's index is 1
        allQuests[0] = null;

        for(int q=1; q<=numOfQuests; q++)
        {
            Quest quest = new Quest(q);
            quest.type = questsData[0, q]; quest.description = questsData[1, q];
            quest.message = questsData[2, q];


            int s = -1;
            int.TryParse(questsData[3, q], out s);
            if(s != -1) quest.defaultSite = s;

            int col1 = -1, col2 = -1, col3 = -1;
            int.TryParse(questsData[4, q], out col1); int.TryParse(questsData[5, q], out col2);
            int.TryParse(questsData[6, q], out col3);
            if(col1 == -1 || col2 == -1 || col3 == -1) { print("parse error"); }

            quest.message_color = new Color(col1/255.0f, col2 / 255.0f, col3 / 255.0f);

            quest.specifics = questsData[7, q]; 
            quest.long_message = questsData[8, q];

            quest.setupFilePath = questsData[9, q];


            allQuests[q-1] = quest; //so quest on questsData[row 1] will be quest 0 in allQuests, and will be assigned a questIndex of 0
        }

        print("quest loader done, "+numOfQuests);
        questLoaderDone = true; //data loaded and parsed
    }

    public void acceptQuest(int qIndex)
    {
        StartCoroutine(acceptQuestCoroutine(qIndex));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="qIndex"></param>
    /// <param name="setupDataRef"></param>
    /// <returns></returns>
    IEnumerator acceptQuestCoroutine(int qIndex)
    {
        //gets the setup csv content to questScript of active quest
        string[,] setupScript = new string[1, 1];
        print("qIndex: " + qIndex + " quests " + allQuests.Length);
        yield return assignQuestSetupData(allQuests[qIndex], (string[,] d)=> { setupScript = d; });

        //parse events of the quest
        List<QuestEvent> qes = parseQuestEvents(setupScript);

        //assign, mark the accept status in save file
        GlobalSingleton.Instance.questStatus.onAcceptQuest(qIndex, setupScript, qes);
        gameFlow.setData(setupScript, new bool[1]); //sets gameFlow's active script to the active quest's script

        //trigger on start event
        actionListenerManager.onTriggerListener(ActionListener.Listener.onStart, new string[1]);
    }

    /// <summary>
    /// used to test quest scripts 
    /// 
    /// to use, assign quest setup script to DlgCsv in GameFlow, will call this command to parse the events within,
    /// and register the quest as the currently active quest (with a false quest id)
    /// </summary>
    public void acceptQuestTestUse(string[,] setupScript)
    {
        List<QuestEvent> qes = parseQuestEvents(setupScript);

        //assign, mark the accept status in save file
        GlobalSingleton.Instance.questStatus.onAcceptQuest(0, setupScript, qes);
        gameFlow.setData(setupScript, new bool[1]); //sets gameFlow's active script to the active quest's script

        //trigger on start event
        actionListenerManager.onTriggerListener(ActionListener.Listener.onStart, new string[1]);
    }


    /// <summary>
    /// assumes quest's setupFilePath is a valid csv
    /// 
    /// setupDataRef is a double array that contains chunks of events to this quest
    /// </summary>
    /// <returns></returns>
    IEnumerator assignQuestSetupData(Quest q, setterDelegate setter)
    {
        print("quest setup " + q.description);

        TextAsset setupCSV = Resources.Load<TextAsset>("Quests/Setup/"+q.setupFilePath);
        if (!setupCSV) Debug.LogError("invalid csv path: " + "Quests/Setup/" + q.setupFilePath);

        bool[] done = new bool[1];

        yield return LoadScene.processCSV(done, setupCSV, setter, true); //keep same numbering as in excel sheet

    }

    List<QuestEvent> parseQuestEvents(string[,] questScript)
    {
        List<QuestEvent> questEvents = new List<QuestEvent>();

        int numRows = questScript.GetLength(1); bool toggle = false;
        for (int r = 1; r < numRows; r++) //-1 because title row doesn't count
        {
            if (questScript[0, r].Length >=2 && questScript[0, r].Substring(0, 2).Equals("##"))
            {
                if (!toggle) //start of event
                {
                    
                    string eventName = questScript[0, r].Substring(2); //from the third character to end
                    int startLineNumber = r+1, endLineNumber = -1; //endline will be set later
                    bool retriggerable = false; //defaults to false
                    bool.TryParse(questScript[1, r], out retriggerable);

                    string[] conditionStrings = questScript[2, r].Split(',');
                    List<(QuestCondition, string[])> conditions = new List<(QuestCondition, string[])>();

                    //parse the conditions and store into list instantiated above
                    foreach (string conditionString in conditionStrings)
                    {

                        int leftBracket = conditionString.IndexOf('['), rightBracket = conditionString.IndexOf(']');
                        string conditionS = "", conditionParams = "";

                        if (leftBracket != -1) //has params inside [ ]
                        {
                            conditionS = conditionString.Substring(0, leftBracket);
                            conditionParams = conditionString.Substring(leftBracket+1, rightBracket - leftBracket-1);
                        }
                        else
                        {
                            conditionS = conditionString;
                        }

                        //Debug.Log("condition: " + conditionS + " params: " + conditionParams);

                        QuestCondition condition;
                        if (Enum.TryParse<QuestCondition>(conditionS, out condition)) //parse condition
                        {
                            string[] eParams = conditionParams.Split(','); //parse condition params

                            conditions.Add((condition, eParams));
                        }
                        else
                        {
                            Debug.LogError("parse condition failed: " + conditionString);
                        }
                    }

                    QuestEvent qe = new QuestEvent(eventName, startLineNumber, endLineNumber, retriggerable, conditions);

                    questEvents.Add(qe);
                    //print("start of event " + eventName + " at " + startLineNumber);
                }
                else //end of event
                {
                    QuestEvent qe = questEvents[questEvents.Count - 1];
                    qe.endLineNumber = r-1;
                    //print("end of event at " + r);
                }
                toggle = !toggle;
            }
        }

        return questEvents;
    }



    public Quest getQuest(int index)
    {
        return allQuests[index];
    }



    /// <summary>
    /// will return literally all quests that ever existed
    /// </summary>
    /// <returns></returns>
    public Quest[] getAllQuests() { return allQuests;  }
}

// class for one single quest
/// <summary>
/// for now a quest is serializable and can be saved, iffy if should do this; for now will parse and load quests, then questStatus save 
/// data (int[]) indicates completion statuses of quests (questboard will use that as reference to set up stuff)
/// </summary>
[Serializable]
public class Quest
{
    public int index, defaultSite; //the official index of the quest
    public string type, description, message, specifics, long_message;
    public string setupFilePath; //TODO csv to ready the quest in game (setups, file writeups, etc.)
    public SerializableColor message_color; 
    public SerializableVector2 location; //coordinate on map; convertable between location name (string) and location coordinates
    public bool ongoing = false; //defaults to false upon parsed, will be set to true if indicated by questStatus

    public Quest(int INDEX, string TYPE, string DESCRIPTION, string MSG, string SPECIFICS, string LONG_MSG,
        Color MSG_COLOR, Vector2 LOCATION) //need not be serializable in params b/c of implicit operator casting
    {
        index = INDEX;
        type = TYPE; description = DESCRIPTION; message = MSG; 
        specifics = SPECIFICS; long_message = LONG_MSG;
        message_color = MSG_COLOR;
        location = LOCATION;
    }

    public Quest(int INDEX)
    {
        index = INDEX;
    }

}


/// <summary>
/// player's questS' status
///
/// - should store all past completed quests (those quests will be inactive and not checked in compareQuests())
/// - current ongoing quests
/// - should store quest objects (quest is a class in QuestLoader)
/// 
/// </summary>
[Serializable]
public class QuestStatusData
{
    //type: Quest
    public int[] allQuestsStatus; //0 inactive/locked, 1 available, 2 active/ongoing, 3 completed; index corresponds to quest id of the quest
    public ActiveQuestData activeQuestData;

    public QuestStatusData(int numQuests)
    {
        allQuestsStatus = new int[numQuests+1]; //[0] is null, since quest 0 doesn't exist
        activeQuestData = new ActiveQuestData(); Debug.Log("new activeQuestData created");
    }

    /// <summary>
    /// should be called when a quest is accepted, its setup file parsed and split into events with their conditions
    ///
    /// will set allQuestsStatus[q] to be "accepted" (2)
    /// </summary>
    /// <param name="questIndex"></param>
    /// <param name="questScript"></param>
    public void onAcceptQuest(int questIndex, string[,] questScript, List<QuestEvent> questEvents)
    {
        allQuestsStatus[questIndex] = 2;
        activeQuestData.setActiveQuest(questIndex, questScript, questEvents);
    }

    public int getSingleQuestStatus(int index) { if (index>=allQuestsStatus.Length) return -1; else return allQuestsStatus[index]; }
}


/// <summary>
/// tracks the ongoing quest and its progress
/// </summary>
[Serializable]
public class ActiveQuestData
{
    public int activeQuestIndex; //quest id of the quest, if == -1, means not taking any quest at the moment
    public QuestProgress currQuestProgress;
    [SerializeField] private string[,] currQuestScript; //csv script for event chunks; line pointers of events refer to lines here
    [SerializeField] private List<QuestEvent> currQuestEvents;

    public ActiveQuestData()
    {
        activeQuestIndex = -1; //defaults to none
    }

    /// <summary>
    /// mark a new active quest upon taking it, generating a new instance of questProgress
    /// </summary>
    /// <param name="index"></param>
    public void setActiveQuest(int index, string[,] questScript, List<QuestEvent> questEvents) { 
        activeQuestIndex = index;

        currQuestProgress = new QuestProgress(); //TODO might need to specialize based on quest 
        Debug.Log("new QuestProgress created");

        currQuestScript = questScript;
        currQuestEvents = questEvents;
    }

    public List<QuestEvent> getQuestEvents() { return currQuestEvents; }

    public string[,] getQuestScript() { return currQuestScript; }

    //TODO etc etc
}


[Serializable]
public class QuestProgress
{
    //TODO
}


/// <summary>
/// corresponds to a block of event in a csv file (but only stores pointers to start and end line), has trigger conditions
/// </summary>
[Serializable]
public class QuestEvent
{
    //public Func<bool> conditionsMet; //TODO make this serializable, or think of alternative way to solve this
    public List<(QuestCondition, string[])> conditions; //list of tuples (each with conditionType and params)
    public string eventName; //tag
    public int startLineNumber, endLineNumber;
    public bool canBeTriggered, retriggerable;

    public QuestEvent(string sEventName, int startLine, int endLine, bool bRetriggerable, List<(QuestCondition, string[])> lConditions)
    {
        eventName = sEventName;
        startLineNumber = startLine;
        endLineNumber = endLine;
        retriggerable = bRetriggerable;
        conditions = lConditions;
    }

    /// <summary>
    /// checks if all conditions are met for this event.
    /// 
    /// a condition's param comes from its quest setup file; each event has conditions that hold certain parameters
    /// a listener's param comes from the object that triggers the listener
    /// 
    /// </summary>
    /// <param name="listener"></param>
    /// <param name="listenerParams"></param>
    /// <returns></returns>
    public bool conditionsMet(ActionListener.Listener listener, string[] listenerParams)
    {
        foreach((QuestCondition, string[]) conditionItem in conditions)
        {
            QuestCondition condition = conditionItem.Item1;
            string[] conditionParams = conditionItem.Item2;

            switch (condition)
            {
                case QuestCondition.onStart:
                    if (listener != ActionListener.Listener.onStart) return false;
                    Debug.Log("on start event triggered!");
                    break;
                case QuestCondition.tap: //conditionParam: (objIdentifier)
                    if (listener == ActionListener.Listener.interactWithCharacter ||
                        listener == ActionListener.Listener.interactWithObject)
                    {
                        if (conditionParams.Length == 1 && conditionParams[0] == "") return false;
                        //for now, tap only checks for one obj

                        if (!conditionParams[0].Equals(listenerParams[0])) return false; //tapped obj identifier != condition required identifier
                        //else, proceed to next condition

                        Debug.Log("condition met, tapped on " + conditionParams[0]);
                    }
                    else
                    {
                        return false; //condition not met
                    }
                    break;
                case QuestCondition.varEqual: //conditionParam: (varName, compareValue)

                    break;
                case QuestCondition.varOver: //same as above

                    break;
                case QuestCondition.varUnder: //same as above

                    break;
                default:
                    Debug.LogError("condition not recognized: " + condition);
                    break;

            }
        }

        return true; //counts as condition met if all conditions to this event are met
    }
}