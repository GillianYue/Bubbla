using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVisual : MonoBehaviour
{

    public AnimationClip freeze, idle, shoot_anticip, shoot, direct_attack_anticip, direct_attack, defeat;
    public Animator myAnimator;
    //public AnimatorOverrideController aoc;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = transform.parent.GetComponent<Animator>();

        //aoc = new AnimatorOverrideController(myAnimator.runtimeAnimatorController);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateModeSprite(BossBehavior.bossMode mode)
    {
        switch (mode)
        {
            case BossBehavior.bossMode.IDLE:
                myAnimator.Play("idle");
                break;
            case BossBehavior.bossMode.ANTICIP:
                myAnimator.Play("shoot_anticipation");
                break;
            case BossBehavior.bossMode.SHOOT_ATTK:
                myAnimator.Play("shoot_attack");
                break;
            case BossBehavior.bossMode.DIR_ATTK:
                myAnimator.Play("direct_attack");
                break;
            case BossBehavior.bossMode.DEFEATED:
                myAnimator.Play("defeat");
                break;
            default:
                myAnimator.Play("test_boss");
                break;
        }
    }
}
