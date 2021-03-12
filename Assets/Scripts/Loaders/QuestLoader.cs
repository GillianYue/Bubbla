﻿using System.Collections;
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
    public SaveLoad saveLoad;

    [Inject(InjectFrom.Anywhere)]
    public QuestConditionManager questConditionManager;

    

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

        allQuests = new Quest[numOfQuests + 1]; //b/c 0 doesn't count; the first quest's index is 1
        allQuests[0] = null;

        for(int q=1; q<=numOfQuests; q++)
        {
            Quest quest = new Quest(q);
            quest.type = questsData[0, q]; quest.description = questsData[1, q];
            quest.message = questsData[2, q];


            int s = -1;
            int.TryParse(questsData[3, q], out s);
            if(s != -1) quest.scene_to_load = s;

            int col1 = -1, col2 = -1, col3 = -1;
            int.TryParse(questsData[4, q], out col1); int.TryParse(questsData[5, q], out col2);
            int.TryParse(questsData[6, q], out col3);
            if(col1 == -1 || col2 == -1 || col3 == -1) { print("parse error"); }

            quest.message_color = new Color(col1/255.0f, col2 / 255.0f, col3 / 255.0f);

            quest.specifics = questsData[7, q]; 
            quest.long_message = questsData[8, q];

            quest.setupFilePath = questsData[9, q];


            allQuests[q] = quest;
        }

        print("quest loader done");
        questLoaderDone = true; //data loaded and parsed
    }

    /// <summary>
    /// parses setup data for a single quest, assumes quest's setupFilePath is a valid csv
    /// </summary>
    /// <returns></returns>
    IEnumerator parseAndAssignQuestSetupData(Quest q)
    {

        TextAsset setupCSV = Resources.Load(q.setupFilePath) as TextAsset;
        if (!setupCSV) Debug.LogError("invalid csv path: " + q.setupFilePath);

        bool[] done = new bool[1];
        string[,] setupData; //contains chunks of events to this quest
        yield return LoadScene.processCSV(done, setupCSV, (string[,] d) => { setupData = d; }, false);

        //TODO assign setupData to right places
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
    public int index, scene_to_load; //the official index of the quest
    public string type, description, message, specifics, long_message;
    public string setupFilePath; //TODO csv to ready the quest in game (setups, file writeups, etc.)
    public SerializableColor message_color; 
    public SerializableVector2 location; //coordinate on map; convertable between location name (string) and location coordinates
    public bool ongoing = false; //defaults to false upon parsed, will be set to true if indicated by questStatus

    public Quest(int INDEX, string TYPE, string DESCRIPTION, string MSG, int SCENE_TO_LOAD, string SPECIFICS, string LONG_MSG,
        Color MSG_COLOR, Vector2 LOCATION) //need not be serializable in params b/c of implicit operator casting
    {
        index = INDEX;
        type = TYPE; description = DESCRIPTION; message = MSG; scene_to_load = SCENE_TO_LOAD;
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
    public string[,] currQuestScript; //csv script for event chunks; line pointers of events refer to lines here
    public List<QuestEvent> currQuestEvents;

    public ActiveQuestData()
    {
        activeQuestIndex = -1; //defaults to none
    }

    /// <summary>
    /// mark a new active quest upon taking it
    /// </summary>
    /// <param name="index"></param>
    public void setActiveQuest(int index, string[,] questScript, List<QuestEvent> questEvents) { 
        activeQuestIndex = index;

        currQuestProgress = new QuestProgress(); //TODO might need to specialize based on quest 
        Debug.Log("new QuestProgress created");

        currQuestScript = questScript;
        currQuestEvents = questEvents;
    }

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
public struct QuestEvent
{
    public Func<bool> conditionsMet; //TODO make this serializable, or think of alternative way to solve this
    public string eventName; //tag
    public int startLineNumber;
    public bool canBeTriggered;

}