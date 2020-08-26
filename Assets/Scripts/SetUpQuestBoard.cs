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

    public QuestStatusData currentQuestStatus;

    public GameObject questGO; //the game object that can visualize quests

    private float questHeight;
    private ArrayList ongoingQuests, availableQuests, pastQuests;


    // Use this for initialization
    void Start () {

        //first set the dimensions of our questBoard rect (based on num of quests
        //the "height" of the rect in RectTransform of quests should always be 80
        //NOTE: this MUST be done before quests are generated

        StartCoroutine(compareQuests()); //eventually this is called during scene load
	}
	
	// Update is called once per frame
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
        yield return new WaitUntil(() => questLoader.questLoadDone() && questStatus != null);
        //so that the quest roster is ready to be compared

        //TODO this and that
        ongoingQuests = questStatus.ongoingQuests; //store quests here
        pastQuests = questStatus.pastQuests; //in case it's a first time
        availableQuests = questStatus.availableQuests;

        currentQuestStatus = questStatus; //this instance is modified as game progresses, and will be taken to use for saving

        ///////only for testing purposes, delete later
        ///
        //availableQuests.Add(questLoader.getQuest(1));
        //availableQuests.Add(questLoader.getQuest(2));
        //availableQuests.Add(questLoader.getQuest(3));
        //availableQuests.Add(questLoader.getQuest(4));
        //availableQuests.Add(questLoader.getQuest(5));

        ////

        setup(); //set up questBoard now that we know how many/what quests we need to create
    }

    private void setup()
    {

        questHeight = Mathf.Abs(questGO.GetComponent<RectTransform>().rect.height);
        var qbHeight = GetComponent<RectTransform>().rect.height;


        float heightNeeded = availableQuests.Count * questHeight; //numOfQuests needed to be gen is not gna be from questLoader,
        //but result from the compare function
        if (heightNeeded > qbHeight)
        {
            GetComponent<RectTransform>().offsetMin =
                new Vector2(0, -1 * (heightNeeded - qbHeight));
            //OffsetMin.x = left, OffsetMin.y = bottom
            GetComponent<RectTransform>().offsetMax =
                new Vector2(0, 0);
            //OffsetMax.x = -right, OffsetMax.y = -top

        }//if scroll bar doesn't need to be stretched (numQuest<4), don't stretch qb rect

        genQuests(availableQuests.Count); //gen quests after setup 
    }

	private void genQuests(int numOfQuests){

		for (int i = 0; i < numOfQuests; i++) {
			genSingleQuest (i, (Quest)availableQuests[i]); //gives quest the row to refer to in string[,] data
		}
	}

	void genSingleQuest(int which, Quest quest){
		GameObject q = questGO; //GO with the aiming sprite
		q = Instantiate (q, GetComponent<RectTransform>().position,
			GetComponent<RectTransform>().rotation) as GameObject;

		q.transform.SetParent (transform);
		q.transform.localScale = new Vector3 (1, 1, 1);


		q.transform.Find ("Description").GetComponent<Text> ().text = 
			quest.type + " "+ quest.description;
		Text msg = q.transform.Find ("Message").GetComponent<Text> ();
			msg.text = quest.message;
        msg.color = quest.message_color;
		q.GetComponent<QuestSelect>().setGoToScene(quest.scene_to_load);
		q.GetComponent<QuestSelect> ().setQuestSpecifics (quest.specifics,
			quest.long_message);
		
		float height = GetComponent<RectTransform> ().rect.height;

        Global.setRectShape(q, 0, 0, -which * (questHeight) - 2,
            height - (which + 1) * questHeight);

			//q.GetComponent<RectTransform> ().offsetMax = 
			//	new Vector2(0,  - which * (questHeight) -2);
			//q.GetComponent<RectTransform> ().offsetMin = 
				//new Vector2(0, height - (which+1) * questHeight);

	}
}
