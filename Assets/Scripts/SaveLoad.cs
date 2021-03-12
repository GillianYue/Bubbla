using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;

public class SaveLoad : MonoBehaviour
{
    [Inject(InjectFrom.Anywhere)]
    public LoadScene loadScene;

    string playerInfo = "/playerInfo.dat", allQuestsStatus = "/questStatus.dat";

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    /// <summary>
    /// loads things that need to be loaded in the beginning of the game TODO 
    /// 
    /// will wait for the csv loaders to be done first, then load in data from the file system
    /// </summary>
    public void LoadAllOnStart()
    {
        StartCoroutine(LoadAllOnStartCoroutine());
    }

    IEnumerator LoadAllOnStartCoroutine()
    {
        yield return loadScene.waitForLoadDone();

        GlobalSingleton.Instance.questStatus = LoadQuestStatus(loadScene.questLoader.numOfQuests);
    }

    public void SavePlayerInfo(PlayerData playerData)
    {
        SaveFileOfType<PlayerData>(playerData, playerInfo);
    }

    public PlayerData LoadPlayerInfo()
    {
        PlayerData d;
        if ((d = LoadFileOfType<PlayerData>(playerInfo)) != null)
        {
            return d;
        }
        else
        {
            return new PlayerData();
        }
    }

    public void SaveQuestStatus(QuestStatusData data)
    {
        SaveFileOfType<QuestStatusData>(data, allQuestsStatus);
    }

    /// <summary>
    /// returns the loaded questStatusData from file, if nonexistent will create new data based on numQuests
    /// </summary>
    /// <param name="numQuests"></param>
    /// <returns></returns>
    public QuestStatusData LoadQuestStatus(int numQuests)
    {
        QuestStatusData d;
        if((d = LoadFileOfType<QuestStatusData>(allQuestsStatus)) != null){
            return d;
        }
        else
        {
            //if save file for questStatusData doesn't exist, will create new 
            Debug.Log("new questStatusData created");
            return new QuestStatusData(numQuests);
        }
    }


    /// <summary>
    /// loads a file of type T, relativePath should be the name of the file to load. e.g. "/questStatus.dat"
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    public T LoadFileOfType<T>(string relativePath)
    {
        if (File.Exists(Application.persistentDataPath + relativePath))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + relativePath, FileMode.Open);
            T data = (T)bf.Deserialize(file);
            file.Close();

            return data;
        }
        else
        {
            Debug.Log("load file not found for file " + relativePath); ;
            return default(T);
        }
    }

    
    public void SaveFileOfType<T>(T data, string relativePath)
    {
        BinaryFormatter bf = new BinaryFormatter();

        FileStream file = File.Create(Application.persistentDataPath + relativePath);

        bf.Serialize(file, data);
        file.Close();

        Debug.Log(data.ToString() + " saved to " + relativePath);
    }

}



[Serializable]
public class PlayerData
{
    public float rankPoints;
    public float coins;

}

