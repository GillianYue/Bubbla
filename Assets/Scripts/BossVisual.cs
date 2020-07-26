using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVisual : MonoBehaviour
{
    /// <summary>
    /// arraylist of vector2: consists of (keyframe, polyColliderIndex) pairs which provides instructions for 
    /// </summary>
    public ArrayList freeze = new ArrayList(), 
        idle = new ArrayList(), 
        shoot_anticip = new ArrayList(), 
        shoot = new ArrayList(), 
        direct_attack_anticip = new ArrayList(), 
        direct_attack = new ArrayList(), 
        defeat = new ArrayList();
    public PolygonCollider2D[] polyColliders;
    public PolygonCollider2D currCollider; //current Collider that's enabled
    public Animator myAnimator; //needs editor assignment
    //public AnimatorOverrideController aoc;

    // Start is called before the first frame update
    void Start()
    {
        myAnimator = transform.parent.gameObject.GetComponent<Animator>();

	//the collider of the GO itself will be included in the list returned;
	//that collider is on index 0, and is disabled (need not be used in principle??)
        polyColliders = transform.GetComponentsInChildren<PolygonCollider2D>(true);

        foreach(PolygonCollider2D p in polyColliders)
        {
            Debug.Log("parent name: " + p.name);

        }

        //aoc = new AnimatorOverrideController(myAnimator.runtimeAnimatorController);
        switchToCollider(1); //defaults to one
	

	    idle.Add(new Vector2(0, 1)); 
	    //if unstated, assumes default collider is same as idle
	    direct_attack.Add(new Vector2(15, 2));
	    direct_attack_anticip.Add(new Vector2(0, 3));
	    shoot.Add(new Vector2(0, 3));

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
		StartCoroutine(changeColliderOnMode(idle));
                break;
            case BossBehavior.bossMode.ANTICIP:
                myAnimator.Play("shoot_anticipation");
		StartCoroutine(changeColliderOnMode(direct_attack_anticip));
                break;
            case BossBehavior.bossMode.SHOOT_ATTK:
                myAnimator.Play("shoot_attack");
		StartCoroutine(changeColliderOnMode(shoot));
                break;
            case BossBehavior.bossMode.DIR_ATTK:
                myAnimator.Play("direct_attack");
		StartCoroutine(changeColliderOnMode(direct_attack));
                break;
            case BossBehavior.bossMode.DEFEATED:
                myAnimator.Play("defeat");
		StartCoroutine(changeColliderOnMode(defeat));
                break;
            default:
                myAnimator.Play("test_boss");
                break;
        }
    }

	private IEnumerator changeColliderOnMode(ArrayList l)
    {
	        ArrayList list = (l.Count == 0)? idle : l; //if undefined, treats as idle
	        float sec = 0;


	        foreach(Vector2 pair in list){

	        float frame = pair.x, to = pair.y;
	        yield return new WaitForSeconds(frame/100 - sec); //always positive since 

        //arraylist should be in ascending order
            switchToCollider((int)to);
	        sec = frame/100;

		        }
	}

    private void switchToCollider(int to)
    {
        if(currCollider) currCollider.gameObject.SetActive(false);
        currCollider = polyColliders[to];
        currCollider.gameObject.SetActive(true); //a different obj from first line
    }
}
