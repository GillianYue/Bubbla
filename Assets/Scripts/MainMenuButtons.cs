using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuButtons : MonoBehaviour
{

    [Inject(InjectFrom.Anywhere)]
    public Backpack backpack;

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
    }

    public void openSettings()
    {

    }

    public void openGoOutMenu()
    {

    }

    public void openPurchaseMenu()
    {
        LoadingScreen.Instance.Show(SceneManager.LoadSceneAsync("level1"));
    }
}
