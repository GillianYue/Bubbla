using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{

    [Inject(InjectFrom.Anywhere)]
    public Backpack backpack;
    [Inject(InjectFrom.Anywhere)]
    public Dialogue dialogue;
    [Inject(InjectFrom.Anywhere)]
    public Outing outing;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openBackpack()
    {
        backpack.openBackpackUI(true); //fullscreen
        dialogue.gameObject.SetActive(false);
    }

    public void openSettings()
    {

    }

    public void openGoOutMenu()
    {
        outing.openMapUI();
        dialogue.gameObject.SetActive(false);
    }

    public void openPurchaseMenu()
    {
        LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync("level1"));
    }
}
