﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// singleton script that will be instantiated on awake and will persist throughout scene transitions
/// 
/// can be used to store data that should not be destroyed during the entire game session
/// </summary>
public class GlobalSingleton : MonoBehaviour
{
    public static GlobalSingleton Instance;

    public QuestStatusData questStatus; //stores (in)active statuses for all quests as well as activeQuestData


    [Inject(InjectFrom.Anywhere)]
    public QuestConditionManager questConditionManager; //stored event conditions for a quest should persist through scenes
    [Inject(InjectFrom.Anywhere)]
    public CustomEvents customEvents;

    [Inject(InjectFrom.Anywhere)]
    public LoadScene loadScene;
    [Inject(InjectFrom.Anywhere)]
    public SaveLoad saveLoad;
    [Inject(InjectFrom.Anywhere)]
    public ResolveScene resolveScene;

    public GameObject siteInstance; //holds the instantiated site instance if in travel mode (from site prefabs)

    //TODO temp cache data on current quest progress

    void Awake()
    {
        //global singleton persist
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {

    }

}