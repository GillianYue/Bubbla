using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetUpQuestBoard : MonoBehaviour {
	public TextAsset questCsv; //contains info for quests
	private string[,] questData; 
	public int numOfQuests;
	public GameObject quest;
	private bool loadDone = false;
    private float questHeight;

    // Use this for initialization
    void Start () {
		StartCoroutine (processCSV ()); //start reading quests from csv file

        //first set the dimensions of our questBoard rect (based on num of quests
        //the "height" of the rect in RectTransform of quests should always be 80
        //NOTE: this MUST be done before quests are generated

        questHeight = Mathf.Abs(quest.GetComponent<RectTransform>().rect.height);
        var qbHeight = GetComponent<RectTransform>().rect.height;


        float heightNeeded = numOfQuests * questHeight;
		if (heightNeeded > qbHeight) {
			GetComponent<RectTransform> ().offsetMin = 
				new Vector2 (0, -1 * (heightNeeded - qbHeight));
			//OffsetMin.x = left, OffsetMin.y = bottom
			GetComponent<RectTransform> ().offsetMax = 
				new Vector2(0, 0);
			//OffsetMax.x = -right, OffsetMax.y = -top

		}//if scroll bar doesn't need to be stretched (numQuest<4), don't stretch qb rect


		StartCoroutine (genQuests ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator processCSV(){ //TODO move this to loading, as opposed to in game
		questData = CSVReader.SplitCsvGrid (questCsv.text); 
		while (!(questData.Length > 0)) {
			yield return null;
		}
		loadDone = true;
		gatherOverallData ();
	}

	private void gatherOverallData(){
		numOfQuests = questData.GetLength (1) - 1; //exclude title row
	}

	private IEnumerator genQuests(){
		while (!loadDone) {
			yield return null;
		}

		for (int i = 0; i < numOfQuests; i++) {
			genSingleQuest (i, i+1); //gives quest the row to refer to in string[,] data
		}
	}

	void genSingleQuest(int which, int rowInData){
		GameObject q = quest; //GO with the aiming sprite
		q = Instantiate (q, GetComponent<RectTransform>().position,
			GetComponent<RectTransform>().rotation) as GameObject;

		q.transform.SetParent (transform);
		q.transform.localScale = new Vector3 (1, 1, 1);


		q.transform.Find ("Description").GetComponent<Text> ().text = 
			questData [0, rowInData] + " "+ questData [1, rowInData];
		Text msg = q.transform.Find ("Message").GetComponent<Text> ();
			msg.text = questData [2, rowInData];
		msg.color = new Color (int.Parse(questData[4, rowInData])/255.0f,
			int.Parse(questData[5, rowInData])/255.0f, int.Parse(questData[6, rowInData])/255.0f);
		q.GetComponent<QuestSelect>().setGoToScene(int.Parse(questData[3, rowInData]));
		q.GetComponent<QuestSelect> ().setQuestSpecifics (questData [7, rowInData],
			questData [8, rowInData]);
		
		float height = GetComponent<RectTransform> ().rect.height;

        Global.setToRectTransform(q, 0, 0, -which * (questHeight) - 2,
            height - (which + 1) * questHeight);

			//q.GetComponent<RectTransform> ().offsetMax = 
			//	new Vector2(0,  - which * (questHeight) -2);
			//q.GetComponent<RectTransform> ().offsetMin = 
				//new Vector2(0, height - (which+1) * questHeight);

	}
}
