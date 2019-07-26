using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLoader : MonoBehaviour
{
    private bool[] loadDone; //when loadDone[0] == true, loading is done for the csv file
    public TextAsset enemyCsv;
    private string[,] data; //double array that stores all info 


    void Start()
    {
        loadDone = new bool[1];

        StartCoroutine(LoadScene.processCSV(loadDone, enemyCsv, setData));
        StartCoroutine(parseEnemyData());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator parseEnemyData()
    {
        yield return new WaitUntil(() => loadDone[0]); //this would mean that data is ready to be parsed

        //TODO: parse
    }

    public void setData(string[,] d)
    {
        data = d;
    }
}
