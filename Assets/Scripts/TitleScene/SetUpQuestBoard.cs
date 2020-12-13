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

    QuestStatusData currentQuestStatus;

    public GameObject questGO; //the game object that can visualize quests

    private float questHeight;
    private ArrayList ongoingQuests, availableQuests, pastQuests;


    void Start () {

        //first set the dimensions of our questBoard rect (based on num of quests
        //the "height" of the rect in RectTransform of quests should always be 80
        //NOTE: this MUST be done before quests are generated

        StartCoroutine(compareQuests()); //eventually this is called during scene load
	}
	
	void Update () {
		
	}

    /// <summary>
    /// compares player progress with the conditions of all quests to determine what should be rendered/not
    ///
    /// </summary>
    /// <returns></returns>
    private IEnumerator compareQuests()
    {
        QuestStatusData questStatus;
        questStatus = saveLoad.LoadQuestStatus();
        //questStatus is current player's progress on quests; questLoadDone is for loading all quests that exist
        yield return new WaitUntil(() => questLoader.questLoadDone() && questStatus != null);
        //so that the quest roster is ready to be compared

        //TODO this and that
        ongoingQuests = questStatus.ongoingQuests; //store quests here
        pastQuests = questStatus.pastQuests; //in case it's a first time
        availableQuests = questStatus.availableQuests;

        currentQuestStatus = questStatus; //this instance is modified as game progresses, and will be taken to use for saving

        ///////only for testing purposes, delete later
        ///
        availableQuests.Add(questLoader.getQuest(1));
        availableQuests.Add(questLoader.getQuest(2));
        availableQuests.Add(questLoader.getQuest(3));
        //availableQuests.Add(questLoader.getQuest(4));
        //availableQuests.Add(questLoader.getQuest(5));

        ////

        setupList(); //set up questBoard now that we know how many/what quests we need to create
    }

    /// <summary>
    /// set up the physical dimensions for the quest prefab instances
    /// </summary>
    private void setupList()
    {
        ListScroller.setupListComponents(this.gameObject, questGO, availableQuests.Count);

        questHeight = Mathf.Abs(questGO.GetComponent<RectTransform>().rect.height);
        ListScroller.genListItems(questGO, availableQuests.Count, this.gameObject, setSingleQuestData);
    }

    //callback after the quests are generated as list items; used for setting data on quests
	void setSingleQuestData(GameObject q, int which){

        Quest quest = (Quest)availableQuests[which];

		q.transform.Find ("Description").GetComponent<Text> ().text = 
			quest.type + " "+ quest.description;
		Text msg = q.transform.Find ("Message").GetComponent<Text> ();
			msg.text = quest.message;
        msg.color = quest.message_color;
		q.GetComponent<QuestSelect>().setGoToScene(quest.scene_to_load);
		q.GetComponent<QuestSelect> ().setQuestSpecifics (quest.specifics,
			quest.long_message);
	}


    public QuestStatusData getCurrentQuestStatus()
    {
        return currentQuestStatus;
    }
}
