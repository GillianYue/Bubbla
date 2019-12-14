using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DlgOptions : MonoBehaviour
{

    public GameObject[] options;
    public Text[] optionTexts;
    private int[] goToLines;

    [Inject(InjectFrom.Anywhere)]
    public GameFlow gameFlow;

    // Start is called before the first frame update
    void Start()
    {
        hideOptions();
    }

    public void showOptions(int numOptions, string[] messages, int[] goTo)
    {
        for(int i = 0; i < numOptions; i++)
        {
            optionTexts[i].text = messages[i];
            options[i].SetActive(true);
        }

        goToLines = goTo; //matches with button index
    }

    public void hideOptions()
    {
        for(int i = 0; i<options.Length; i++)
        {
            options[i].SetActive(false);
            optionTexts[i].text = "";
        }
    }

    public void buttonPressed(int buttonIndex)
    {
        gameFlow.setPointer(goToLines[buttonIndex]);
        hideOptions();
        gameFlow.canMovePointer = true;
    }
}
