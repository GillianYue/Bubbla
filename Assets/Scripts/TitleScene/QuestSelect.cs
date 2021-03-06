﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// UI helper script for single quest object, attached to the minimized quests (buttons)
/// </summary>
public class QuestSelect : MonoBehaviour {

	public QuestLoader questLoader;

	private Button btn;
	private int goToSceneNO = -1;
	private String longDesc, longMSG;
	public GameObject questDetails;
	public int questIndex;

	void Start()
	{

		btn = gameObject.GetComponent<Button>();

		btn.onClick.AddListener(spawnQuestDetail);

	}
		
	/// <summary>
	/// UI event for mini quest display onclick (will show quest detail)
	/// </summary>
	public void spawnQuestDetail(){ //happens when pressed
		GameObject qd = questDetails; //GO with the aiming sprite
		qd = Instantiate (qd) as GameObject;

		qd.transform.Find("Description").GetComponent<Text>().text = longDesc;
		qd.transform.Find("Message").GetComponent<Text>().text = longMSG;

		qd.transform.SetParent (GameObject.Find("CanvasBG").transform, false);
		qd.transform.localRotation = new Quaternion (0, 0, 0, 0);

		qd.SetActive (true);

		QuestDetail qDetail = qd.GetComponent<QuestDetail>();

		qDetail.questLoader = questLoader;
		qDetail.questIndex = questIndex;
		/*
		foreach (Button b in transform.parent.GetComponentsInChildren<Button>()) {
			b.interactable = false;
		}
		*/
	}

	public void setQuestSpecifics(int qIndex, String longD, String longM){
		longDesc = longD;
		longMSG = longM;
		questIndex = qIndex;
	}

}
