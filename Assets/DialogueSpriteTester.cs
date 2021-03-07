using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueSpriteTester : MonoBehaviour
{
    public InputField charIndex, baseState, partIState, partIIState, textBlock;
    public Animator charAnimator;
    public Image charBG;
    [Inject(InjectFrom.Anywhere)]
    public CharacterLoader characterLoader;

    public Dialogue dialogue;

    private int currCharIndex;

    public bool useDialogueTester;

    [Inject(InjectFrom.Anywhere)]
    public GameControl gameControl;

    void Awake()
    {
        
    }

    void Start()
    {
        currCharIndex = 0;

        if (!useDialogueTester) gameObject.SetActive(false); else gameControl.ckTouch = false;
    }


    void Update()
    {
        
    }

    public void onChangeCharIndex()
    {
        int val = -1;
        int.TryParse(charIndex.text, out val);
        if(val != -1)
        {
            charAnimator.runtimeAnimatorController = characterLoader.getAnimatorByIndex(val);
            charBG.color = characterLoader.getColorByIndex(val);
            currCharIndex = val;
        }
    }

    public void onChangeBase()
    {
        int val = -1;
        int.TryParse(baseState.text, out val);
        if (val != -1)
        {
            characterLoader.playBaseAnimation(charAnimator, currCharIndex, val);
            print("base play " + val + " for " + currCharIndex);
        }
    }

    public void onChangePartI()
    {
        int val = -1;
        int.TryParse(partIState.text, out val);
        if (val != -1)
        {
            characterLoader.playPart1Animation(charAnimator, currCharIndex, val);
        }
    }

    public void onChangePartII()
    {
        int val = -1;
        int.TryParse(partIIState.text, out val);
        if (val != -1)
        {
            characterLoader.playPart2Animation(charAnimator, currCharIndex, val);
        }
    }

    public void onChangeTextBlock()
    {

        dialogue.simpleDisplayOneLine(characterLoader.getName(currCharIndex), textBlock.text, baseState.text);
    }

}
