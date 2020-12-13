using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestDetail : MonoBehaviour {
	
	public GameObject cancelScn;
	private GameObject cancel;
	private Button cclB;
	public Button go;
	private int go_to_scene = -1;

	void Start () {
		cancel = cancelScn; //cancel will block interaction w background UIs
		cancel = Instantiate (cancel) as GameObject;
		cancel.transform.SetParent (GameObject.Find ("CanvasBG").transform, false);
		cancel.SetActive (true);
		//this.transform.SetAsLastSibling (); //move to front most position
		transform.SetAsLastSibling(); 

		cclB = cancel.GetComponent<Button>();
		cclB.onClick.AddListener(cancelClicked);

		go.onClick.AddListener (startQuest);
	}
		
	void Update () {
	
	}

	void cancelClicked(){
		GameObject.Destroy (cancel);
		GameObject.Destroy (gameObject);
	}

	void startQuest(){
		Global.Scene_To_Load = go_to_scene;
		StartCoroutine(Global.LoadAsyncScene (1)); //loads the loading scene, which
	}

	public void setGoToScene(int s){ //individual quests will transfer this info to quest detail
		go_to_scene = s;
	}
}
