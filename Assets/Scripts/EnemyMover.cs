using UnityEngine;
using System.Collections;

//utilizes rigidbody.velocity to move enemies around
public class EnemyMover : MonoBehaviour {

	public float speedHorizontal, speedVertical, accl = 0;
	private Rigidbody2D rb;
	public Vector3 direction;
	public int enemyType;
	//type 0 inanimate, 1 move-stop, 2 screen span, 3 track
	public int scnRange; //half OffMeshLink world Screen width
	public Camera cam;
    public bool needVelo = true, needAccl = false; //sets velo on start if not already set


	void Start() {
		//Rigidbody
		rb = GetComponent<Rigidbody2D> ();
        if(needVelo) rb.velocity = new Vector3(speedHorizontal * direction.x, speedVertical * direction.y, 0);
		scnRange = Screen.width / 2 - 90; //TODO fix here

		switch(enemyType){
		case 1: //crab
			StartCoroutine (movePause (8));
			break;
		case 2: 
		//	StartCoroutine (screenSpan ());
			break;
		default:
			//just sticks with the original velocity
			break;
		}
	}

    /**
     * sets spd; assumes that direction is already in place
     */
    public void setSpeed(float spdHoriz, float spdVert)
    {
        speedHorizontal = spdHoriz;
		speedVertical = spdVert;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(speedHorizontal * direction.x, speedVertical * direction.y, 0);
    }

    public void setVelocity(Vector2 velo)
    {
        GetComponent<Rigidbody2D>().velocity = velo;
        needVelo = false;
    }

    public void setAcceleration(float acc)
    {
        accl = acc;
        needAccl = true;
    }

    void Update(){
        //acceleration
        if (needAccl)
            GetComponent<Rigidbody2D>().velocity += new Vector2
            (GetComponent<Rigidbody2D>().velocity.x * accl, GetComponent<Rigidbody2D>().velocity.y * accl);
    }


	//does not interfere with movement on the y axis
	IEnumerator movePause (int turns){

		for (int t = 0; t < turns; t++) {
			rb.velocity += new Vector2(speedHorizontal * Mathf.Pow(-1, t%2), 0);
			GetComponent<SpriteRenderer> ().flipX = (t%2 == 0 ? false : true);
			//even numbers: positive velocity
			if (t % 2 == 0) {
				while (rb.transform.position.x < scnRange) {
					yield return new WaitForSeconds (.5f);
					//not changing the velocity (stuck in this loop) until reaching the target
				}
			} else { //else, odd, negative velocity
				while (rb.transform.position.x > 
					-1*(scnRange)) {
					yield return new WaitForSeconds (.5f);
				}
			}

			//reset speed upon reaching the target
			rb.velocity = new Vector2(0, rb.velocity.y);
		}
	}

	/*IEnumerator screenSpan(){
		//enemy travels through screen

	}
	*/

}
