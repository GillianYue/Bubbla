using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

public class SaveLoad : MonoBehaviour
{
    void Start()
    {
        
    }

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
            Debug.Log("quest status file not found in LoadQuestStatus()");
            return null;
        }
    }


}



[Serializable]
public class PlayerData
{
    public float rankPoints;
    public float coins;

}

