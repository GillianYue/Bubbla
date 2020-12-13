using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{
    //TODO can also take care of open animation in this script

    [Inject(InjectFrom.Anywhere)]
    public Backpack backpack;
    [Inject(InjectFrom.Anywhere)]
    public Dialogue dialogue;
    [Inject(InjectFrom.Anywhere)]
    public Outing outing;

    public GameObject basePanel; //needs to be disabled when upper ui shows up

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void toggleMainUI(bool active)
    {
        basePanel.SetActive(active);
        dialogue.gameObject.SetActive(active);
    }

    public void openBackpack()
    {
        backpack.openBackpackUI(true); //fullscreen
        toggleMainUI(false);
    }

    public void openSettings()
    {

        toggleMainUI(false);
    }

    public void openGoOutMenu()
    {
        outing.openMapUI();
        toggleMainUI(false);
    }

    public void openPurchaseMenu()
    {
        //  LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync("level1"));
        toggleMainUI(false);
    }

    //close functions
    //assign to respective close buttons

    public void closeBackpack()
    {
        backpack.closeBackpackUI();
        toggleMainUI(true);
    }

    public void closeSettings()
    {

        toggleMainUI(true);
    }

    public void closeGoOutMenu()
    {
        outing.closeMapUI();
        toggleMainUI(true);
    }

    public void closePurchaseMenu()
    {

        toggleMainUI(true);
    }
}
