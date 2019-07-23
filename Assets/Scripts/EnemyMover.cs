using UnityEngine;
using System.Collections;

public class EnemyMover : MonoBehaviour {

	public float speed;
	private Rigidbody rb;
	public Vector3 direction;
	public int enemyType;
	//type 0 inanimate, 1 move-stop, 2 screen span, 3 track
	public int scnRange; //half OffMeshLink world Screen width
	public Camera cam;



	void Start() {
		//Rigidbody
		rb = GetComponent<Rigidbody> ();
		rb.velocity = direction * speed;

		switch(enemyType){
		case 1: //crab
			StartCoroutine (movePause (8, 2f));
			break;
		case 2: 
		//	StartCoroutine (screenSpan ());
			break;
		default:
			//just sticks with the original velocity
			break;
		}
	}

	void Update(){
		
	}

	IEnumerator movePause (int turns, float speed){

		for (int t = 0; t < turns; t++) {
			rb.velocity += new Vector3(speed * Mathf.Pow(-1, t%2), 0, 0);
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
			rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
		}
	}

	/*IEnumerator screenSpan(){
		//enemy travels through screen

	}
	*/

    public static void changePos(GameObject e, int x, int y)
    {
        e.transform.position.Set(x, y, e.transform.position.z);
    }

    public static IEnumerator moveTo(GameObject e, int x, int y, float spd, bool[] done)
    {
        float hyp = Mathf.Sqrt(Mathf.Pow(x - e.transform.position.x, 2) +
            Mathf.Pow(y - e.transform.position.y, 2));
        float dx = spd * x / hyp * (x > e.transform.position.x ? 1 : -1); //amount of x changed each little move
        float dy = spd * y / hyp * (y > e.transform.position.y ? 1 : -1);
        Vector2 dir = new Vector2((dx > 0) ? 1 : -1, (dy > 0) ? 1 : -1);

        e.GetComponent<Rigidbody>().velocity = new Vector3(dx, dy, 0);
        /**
         * messy looking check here, but basically makes sure it keeps moving until it gets to target
         */
        while( (e.transform.position.x) * dir.x <= x * dir.x &&
            (e.transform.position.y) * dir.y <= y * dir.y) {
            //e.transform.position += new Vector3(dx, dy, 0);
            yield return new WaitForSeconds(0.1f);
            }

        e.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        done[0] = true;
    }

    public static IEnumerator moveToInSecs(GameObject e, int x, int y, float sec, bool[] done)
    {
        float xDist = x - e.transform.position.x;
        float yDist = y - e.transform.position.y;
        float dx = xDist / sec;
        float dy = yDist / sec;

        e.GetComponent<Rigidbody>().velocity = new Vector3(dx, dy, 0);

        yield return new WaitForSeconds(sec);

        e.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        done[0] = true;
    }
}
