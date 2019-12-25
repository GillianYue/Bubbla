using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLoader : MonoBehaviour
{

    public TextAsset questCsv; //contains info for quests
    private string[,] questData;
    private int numOfQuests;

    private bool[] questLoaderDone, loadDone;
    private Quest[] allQuests;

    // Start is called before the first frame update
    void Start()
    {
        loadDone = new bool[1];
        questLoaderDone = new bool[1];

        StartCoroutine(LoadScene.processCSV(loadDone, questCsv, setData, false)); //data[col,1] will be the first quest (tho in excel visually it's at line 2)
        StartCoroutine(parseQuestData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool questLoadDone()
    {
        return questLoaderDone[0];
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

    IEnumerator parseQuestData()
    {
        while (!(loadDone[0]))
        {
            yield return null;
        }

        numOfQuests = questData.GetLength(1) - 1; //exclude title row; data[col, 1_to_numOfQuests] are the quests
        allQuests = new Quest[numOfQuests + 1]; //b/c 0 doesn't count; the first quest's index is 1
        allQuests[0] = null;

        for(int q=1; q<=numOfQuests; q++)
        {
            Quest quest = new Quest(q);
            quest.type = questData[0, q]; quest.description = questData[1, q];
            quest.message = questData[2, q];

            quest.scene_to_load = int.Parse(questData[3, q]);

            quest.message_color = new Color(int.Parse(questData[4, q]) / 255.0f,
            int.Parse(questData[5, q]) / 255.0f, int.Parse(questData[6, q]) / 255.0f);

            quest.specifics = questData[7, q]; quest.long_message = questData[8, q];

            allQuests[q] = quest;
        }


        questLoaderDone[0] = true; //data loaded and parsed
    }

    public Quest getQuest(int index)
    {
        return allQuests[index];
    }
}

// class for one single quest
public class Quest
{
    public int index, scene_to_load; //the official index of the quest
    public string type, description, message, specifics, long_message;
    public Color message_color;
    public Vector2 location; //coordinate on map; convertable between location name (string) and location coordinates

    public Quest(int INDEX, string TYPE, string DESCRIPTION, string MSG, int SCENE_TO_LOAD, string SPECIFICS, string LONG_MSG,
        Color MSG_COLOR, Vector2 LOCATION)
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
