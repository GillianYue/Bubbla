using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class QuestSelect : MonoBehaviour {

	private Button btn;
	private int goToSceneNO = -1;
	private String longDesc, longMSG;
	public GameObject questDetails;

		void Start()
		{

			btn = gameObject.GetComponent<Button>();

		btn.onClick.AddListener(openQuestDetail);

		}
		

	public void openQuestDetail(){ //happens when pressed
		GameObject qd = questDetails; //GO with the aiming sprite
		qd = Instantiate (qd, GetComponent<RectTransform>().position,
			GetComponent<RectTransform>().rotation) as GameObject;

		qd.transform.Find("Description").GetComponent<Text>().text = longDesc;
		qd.transform.Find("Message").GetComponent<Text>().text = longMSG;

		qd.transform.SetParent (GameObject.Find("Canvas").transform, false);
		qd.transform.localRotation = new Quaternion (0, 0, 0, 0);

		qd.SetActive (true);

		qd.GetComponent<QuestDetail> ().setGoToScene (goToSceneNO);
		/*
		foreach (Button b in transform.parent.GetComponentsInChildren<Button>()) {
			b.interactable = false;
		}
		*/
	}

	public void setGoToScene(int no){
		goToSceneNO = no;
	}

	public void setQuestSpecifics(String longD, String longM){
		longDesc = longD;
		longMSG = longM;
	}

}
