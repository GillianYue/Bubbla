using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// attached to quest objects instantiated under a questboard. Will trigger startQuest when player chooses to start the given quest
/// </summary>
public class QuestDetail : MonoBehaviour {

	public QuestLoader questLoader;

	public GameObject cancelScnPrefab;
	private GameObject cancel;
	private Button cclB;
	public Button register, go;
	public int questIndex;


	void Start () {
		//cancel will block interaction w background UIs
		cancel = Instantiate (cancelScnPrefab) as GameObject;
		cancel.transform.SetParent (GameObject.Find ("CanvasBG").transform, false);
		cancel.SetActive (true);
		//move to front most position
		transform.SetAsLastSibling(); 

		cclB = cancel.GetComponent<Button>();
		cclB.onClick.AddListener(cancelClicked);

		register.onClick.AddListener (acceptQuest);
		go.onClick.AddListener(goToQuestDefaultSite);
	}
		
	void Update () {
	
	}

	void cancelClicked(){
		GameObject.Destroy (cancel);
		GameObject.Destroy (gameObject);
	}

	/// <summary>
	/// upon taking a quest
	/// </summary>
	void acceptQuest(){

		//Global.Scene_To_Load = go_to_scene;
		//StartCoroutine(Global.LoadAsyncScene (1)); //TODO revise

		questLoader.acceptQuest(questIndex);

	}

	void goToQuestDefaultSite() {
		StartCoroutine(LoadTravelSceneWithSiteCoroutine(int siteIndex)
	}

}
