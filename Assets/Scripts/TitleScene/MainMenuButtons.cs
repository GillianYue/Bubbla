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

    void Start()
    {
        
    }

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
      //  LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync("level1"));
    }

    //close functions
    //assign to respective close buttons

    public void closeBackpack()
    {
        backpack.closeBackpackUI(); 
        dialogue.gameObject.SetActive(true);
    }

    public void closeSettings()
    {

    }

    public void closeGoOutMenu()
    {
        outing.closeMapUI();
        dialogue.gameObject.SetActive(true);
    }

    public void closePurchaseMenu()
    {

    }
}
