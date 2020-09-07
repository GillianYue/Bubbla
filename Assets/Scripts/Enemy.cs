using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	int life = 1000;
	int attack;

    public AudioStorage audioz;

    public float sizeScale; //same as paintball, base scale to be multiplied with global
    public float colliderScale; //multiplied to the original generated polygon 2d collider shape to resize

	public enum BuffMode { burn, freeze, none };
	public BuffMode debuff = BuffMode.none;
	IEnumerator currBuffProcess;

	// Use this for initialization
	void Start () {
		audioz = GameObject.FindWithTag ("AudioStorage").GetComponent<AudioStorage>();

        setSizeScale(sizeScale);

        setColliderScale(colliderScale);

    }
	
	// Update is called once per frame
	void Update () {
		if (life <= 0) {
			audioz.enemyDefeatedSE ();
			Destroy (gameObject);
		}
	}

	public int getLife(){
		return life;
	}

	public void setValues(int LIFE, int ATTACK){
		life = LIFE;
		attack = ATTACK;
	}

	public void damage(int damage, Color col){
		life -= damage;
		audioz.enemyDamagedSE ();
		StartCoroutine (damageVFXenemy(col));
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.GetComponent<Collider2D> ().tag == "Player") {
			other.gameObject.GetComponent<Player> ().damage (attack);
		}
	}

	IEnumerator damageVFXenemy(Color col){
			for (int i = 0; i < 2; i++) {
//				//flip visibility on and off
				GetComponent<SpriteRenderer> ().enabled = !GetComponent<SpriteRenderer> ().enabled;

				yield return new WaitForSeconds (0.1f);
			}
}

    public void setSizeScale(float sScale)
    {
        sizeScale = sScale;

        if (Global.scaleRatio != 0)
        {
            transform.localScale = new Vector3(sizeScale * Global.scaleRatio,
                sizeScale * Global.scaleRatio, sizeScale * Global.scaleRatio);
        }
    }

    public void setColliderScale(float cScale)
    {
        Vector2[] colliderPoints = GetComponent<PolygonCollider2D>().points;
        Vector2[] scaledPoints = new Vector2[colliderPoints.Length];
        for (int p = 0; p < colliderPoints.Length; p++)
        {
            Vector2 np = GetComponent<PolygonCollider2D>().points[p] * colliderScale;
            scaledPoints[p] = np;
        }
        GetComponent<PolygonCollider2D>().SetPath(0, scaledPoints);
    }

	public void triggerBuff(BuffMode b)
    {
		if (currBuffProcess != null) StopCoroutine(currBuffProcess);
		debuff = b;

        switch (b)
        {
			case BuffMode.burn:
				currBuffProcess = burnDuration();
				StartCoroutine(currBuffProcess);
				break;
			case BuffMode.freeze:
				currBuffProcess = freezeDuration();
				StartCoroutine(currBuffProcess);
				break;
			default:
				Debug.Log("buff case not recognized");
				break;
		}
    }

	IEnumerator burnDuration()
    {
		life -= 3; //TODO edit
				   //TODO sound effect
		//TODO instantiate vfx here
		yield return new WaitForSeconds(2);
    }

	IEnumerator freezeDuration()
    {
		//sfx, vfx
		yield return new WaitForSeconds(3);
		debuff = BuffMode.none; //cancels after duration
    }

	IEnumerator poisonDuration()
    {
		for(int count = 0; count < 3; count++) { 
		life -= 1;
			//TODO sound effect
		bool[] done = new bool[1];
		StartCoroutine(poisonDamageEffect(this.gameObject, done));
		yield return new WaitUntil(() => done[0]);

		yield return new WaitForSeconds(1.0f);
		}
	}


	public static IEnumerator poisonDamageEffect(GameObject go, bool[] done)
    {
		Vector3 origPos = go.transform.position;
		go.transform.position = origPos + new Vector3(10, 0, 0);
		yield return new WaitForSeconds(0.3f);
		go.transform.position = origPos - new Vector3(10, 0, 0);
		yield return new WaitForSeconds(0.3f);
		go.transform.position = origPos;

		done[0] = true;
	}



}
