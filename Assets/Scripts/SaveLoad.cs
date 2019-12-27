using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

public class SaveLoad : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SavePlayerInfo()
    {
        BinaryFormatter bf = new BinaryFormatter();
      //  Debug.Log("persistentDataPath: " + Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData playerData = new PlayerData();
        playerData.rankPoints = 50; //test
        playerData.coins = 10;

        bf.Serialize(file, playerData);
        file.Close();

        Debug.Log("player info saved to playerInfo.dat");
    }

    public PlayerData LoadPlayerInfo()
    {
        if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            Debug.Log("loaded. rankPoints: " + data.rankPoints + " coins: " + data.coins);
            return data;
        }
        else
        {
            Debug.Log("file not found in LoadPlayerInfo(), returning new instance");
            return new PlayerData();
        }
    }

    public void SaveQuestStatus(QuestStatusData data)
    {
        BinaryFormatter bf = new BinaryFormatter();
       // Debug.Log("persistentDataPath: " + Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/questStatus.dat");

        bf.Serialize(file, data);
        file.Close();

        Debug.Log("quest saved to questStatus.dat");
    }

    public QuestStatusData LoadQuestStatus()
    {
       // Debug.Log("persistentDataPath: " + Application.persistentDataPath);
        if (File.Exists(Application.persistentDataPath + "/questStatus.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/questStatus.dat", FileMode.Open);
            QuestStatusData data = (QuestStatusData)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            Debug.Log("file not found in LoadQuestStatus(), returning new instance");
            return new QuestStatusData();
        }
    }


}

[Serializable]
public class PlayerData
{
    public float rankPoints;
    public float coins;

}

/// <summary>
/// player's quests status
///
/// - should store all past completed quests (those quests will be inactive and not checked in compareQuests())
/// - current ongoing quests
/// - should store quest objects (quest is a class in QuestLoader)
/// 
/// </summary>
[Serializable]
public class QuestStatusData
{
    public ArrayList pastQuests, availableQuests, ongoingQuests; //TODO there's more

    public QuestStatusData()
    {
        pastQuests = new ArrayList(); availableQuests = new ArrayList();
        ongoingQuests = new ArrayList();
    }

}