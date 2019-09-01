using UnityEngine;
using System.Collections;

public class EnemyMover : MonoBehaviour {

	public float speed, accl = 0;
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
        if(needVelo) rb.velocity = direction * speed;

		switch(enemyType){
		case 1: //crab
			StartCoroutine (movePause (8, speed));
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
    public void setSpeed(float spd)
    {
        speed = spd;
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = direction * spd;
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


	IEnumerator movePause (int turns, float speed){

		for (int t = 0; t < turns; t++) {
			rb.velocity += new Vector2(speed * Mathf.Pow(-1, t%2), 0);
			GetComponent<SpriteRenderer> ().flipX = (t%2 == 0 ? false : true);
			//even numbers: positive velocity
			if (t % 2 == 0) {
				while (rb.transform.position.x < 
					(Screen.width * Global.STWfactor.x/2)*0.8) {
					yield return new WaitForSeconds (.5f);
					//not changing the velocity (stuck in this loop) until reaching the target
				}
			} else { //else, odd, negative velocity
				while (rb.transform.position.x > 
					-1*(Screen.width * Global.STWfactor.x/2)*0.8) {
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
