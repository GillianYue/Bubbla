using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// in charge of visualizing given quests, not in charge of loading them (see questLoader)
/// </summary>
public class SetUpQuestBoard : MonoBehaviour {

    [Inject(InjectFrom.Anywhere)]
    public QuestLoader questLoader;
    [Inject(InjectFrom.Anywhere)]
    public SaveLoad saveLoad;

    public GameObject questGO; //the game object that can visualize quests

    private ArrayList availableQuests;


    void Start () {

        //type: Quest
        availableQuests = new ArrayList();

        //first set the dimensions of our questBoard rect (based on num of quests
        //the "height" of the rect in RectTransform of quests should always be 80
        //NOTE: this MUST be done before quests are generated

        StartCoroutine(startSetup());
	}
	
	void Update () {
		
	}

    IEnumerator startSetup()
    {
        yield return new WaitUntil(() => GlobalSingleton.Instance.loadAllDone);
        StartCoroutine(setupQuestBoard()); //eventually this is called during scene load
    }

    /// <summary>
    /// compares player progress with the conditions of all quests to determine what should be rendered/not
    ///
    /// </summary>
    /// <returns></returns>
    private IEnumerator setupQuestBoard()
    {
        QuestStatusData currentQuestStatus = GlobalSingleton.Instance.questStatus;
        //questStatus is current player's progress on quests; questLoadDone is for loading all quests that exist
        yield return new WaitUntil(() => questLoader.isLoadDone());
        //so that the quest roster is ready to be compared

        //initialize questStatus if non-existent
        if (currentQuestStatus == null) currentQuestStatus = new QuestStatusData(questLoader.getNumQuests());

        Quest currActive = null;
        //quest #0 doesn't exist, skipped
        for (int _num=1; _num<currentQuestStatus.allQuestsStatus.Length; _num++)
        {
            switch (currentQuestStatus.allQuestsStatus[_num])
            {
                case 0: break; //inactive
                case 1: //available
                    availableQuests.Add(questLoader.getQuest(_num));
                    break;
                case 2:
                    currActive = questLoader.getQuest(_num);
                    currActive.ongoing = true; 
                    break;
                case 3: break; //finished
            }
        }
        if (currActive != null) availableQuests.Insert(0, currActive); //insert at top of list

        //TODO temp
        availableQuests.Add(questLoader.getQuest(1));
        currActive = questLoader.getQuest(2);
        currActive.ongoing = true;
        availableQuests.Insert(0, currActive);
        //

        setupList(); //set up questBoard now that we know how many/what quests we need to create
    }

    /// <summary>
    /// set up the physical dimensions for the quest prefab instances
    /// </summary>
    private void setupList()
    {
        if (availableQuests.Count == 0) return;

        ListScroller.setupList(this.gameObject, questGO, availableQuests.Count, setSingleQuestData);
    }

    //callback after the quests are generated as list items; used for setting data on quests
	void setSingleQuestData(GameObject q, int which){

        Quest quest = (Quest)availableQuests[which];

		q.transform.Find("Description").GetComponent<Text>().text = 
			quest.type + " "+ quest.description;
		Text msg = q.transform.Find ("Message").GetComponent<Text> ();
			msg.text = quest.message;
        msg.color = quest.message_color;
        QuestSelect qs = q.GetComponent<QuestSelect>();
		qs.setQuestSpecifics (which, quest.specifics,
			quest.long_message);
        qs.questLoader = questLoader;

        if (quest.ongoing) q.GetComponent<Image>().color = Color.cyan; //diff color for ongoing quest
	}


}
