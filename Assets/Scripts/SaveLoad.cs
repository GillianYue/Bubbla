using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        Debug.Log("persistentDataPath: " + Application.persistentDataPath);
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");

        PlayerData playerData = new PlayerData();
        playerData.rankPoints = 50; //test
        playerData.coins = 10;

        bf.Serialize(file, playerData);
        file.Close();
    }

    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();

            Debug.Log("loaded. rankPoints: " + data.rankPoints + " coins: " + data.coins);
        }
    }


}

[Serializable]
class PlayerData
{
    public float rankPoints;
    public float coins;

}