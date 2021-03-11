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
    private string[,] questData;
    public int numOfQuests;

    private bool[] loadDone;
    private bool questLoaderDone;
    private Quest[] allQuests;

    [Inject(InjectFrom.Anywhere)]
    public SaveLoad saveLoad;

    [Inject(InjectFrom.Anywhere)]
    public QuestConditionManager questConditionManager;

    public QuestStatusData questStatus; //stores (in)active statuses for all quests as well as activeQuestData

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
        yield return StartCoroutine(parseQuestData()); //will set the correct numOfQuests

        questStatus = saveLoad.LoadQuestStatus(numOfQuests);
    }

    /// <summary>
    /// overrides parent
    /// </summary>
    /// <returns></returns>
    public override bool isLoadDone()
    {
        return questLoaderDone;
    }

    public void setData(string[,] d)
    {
        questData = d;
    }

    //returns num of all quests available in loader
    public int getNumQuests()
    {
        return numOfQuests; 
    }

    /// <summary>
    /// parses data for all quest entries 
    /// </summary>
    /// <returns></returns>
    IEnumerator parseQuestData()
    {
        //data[col,1] will be the first quest (tho in excel visually it's at line 2)
        yield return LoadScene.processCSV(loadDone, questCsv, setData, false);

        numOfQuests = questData.GetLength(1) - 1; //exclude title row; data[col, 1_to_numOfQuests] are the quests

        allQuests = new Quest[numOfQuests + 1]; //b/c 0 doesn't count; the first quest's index is 1
        allQuests[0] = null;

        for(int q=1; q<=numOfQuests; q++)
        {
            Quest quest = new Quest(q);
            quest.type = questData[0, q]; quest.description = questData[1, q];
            quest.message = questData[2, q];


            int s = -1;
            int.TryParse(questData[3, q], out s);
            if(s != -1) quest.scene_to_load = s;

            int col1 = -1, col2 = -1, col3 = -1;
            int.TryParse(questData[4, q], out col1); int.TryParse(questData[5, q], out col2);
            int.TryParse(questData[6, q], out col3);
            if(col1 == -1 || col2 == -1 || col3 == -1) { print("parse error"); }

            quest.message_color = new Color(col1/255.0f, col2 / 255.0f, col3 / 255.0f);

            quest.specifics = questData[7, q]; 
            quest.long_message = questData[8, q];

            quest.setupFilePath = questData[9, q];


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
        string[,] setupData;
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
        activeQuestData = new ActiveQuestData();
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

    public ActiveQuestData()
    {
        activeQuestIndex = -1; //defaults to none
    }

    public void setActiveQuestIndex(int i) { 
        activeQuestIndex = i;
        currQuestProgress = new QuestProgress(); //TODO might need to specialize based on quest
    }

    //TODO etc etc
}


[Serializable]
public class QuestProgress
{
    //TODO
}