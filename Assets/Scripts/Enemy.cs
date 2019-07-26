using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	int life = 1000;
	int attack;
	public AudioStorage audioz;
    public float sizeScale; //same as paintball, base scale to be multiplied with global
    public float colliderScale; //multiplied to the original generated polygon 2d collider shape to resize

	// Use this for initialization
	void Start () {
		audioz = GameObject.FindWithTag ("AudioStorage").GetComponent<AudioStorage>();
        if (Global.scaleRatio != 0)
        {
            transform.localScale = new Vector3(sizeScale * Global.scaleRatio,
                sizeScale * Global.scaleRatio, sizeScale * Global.scaleRatio);
        }

        Vector2[] colliderPoints = GetComponent<PolygonCollider2D>().points;
        Vector2[] scaledPoints = new Vector2[colliderPoints.Length];
        for(int p = 0; p < colliderPoints.Length; p++)
        {
          //  Debug.Log("point: " + GetComponent<PolygonCollider2D>().points[p]+" to scale "+colliderScale);
            Vector2 np = GetComponent<PolygonCollider2D>().points[p] * colliderScale;
            scaledPoints[p] = np;
          //  Debug.Log("point after scale: " + GetComponent<PolygonCollider2D>().points[p]);
        }
        GetComponent<PolygonCollider2D>().SetPath(0, scaledPoints);

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

	void OnTriggerEnter(Collider other){
		if (other.GetComponent<Collider> ().tag == "Player") {
			other.gameObject.GetComponent<Player> ().damage (attack);
		}
	}

	IEnumerator damageVFXenemy(Color col){
//		if ( GetComponent<Animator> ().GetBool ("Whitened") == false) {
//			Color orig = GetComponent<SpriteRenderer> ().color;
			for (int i = 0; i < 2; i++) {
//				//flip visibility on and off
				GetComponent<SpriteRenderer> ().enabled = !GetComponent<SpriteRenderer> ().enabled;
//
//				if (i == 0) {
//					GetComponent<Animator> ().SetBool ("Whitened", true);
//					GetComponent<SpriteRenderer> ().color = col;
//				} else {
//					GetComponent<Animator> ().SetBool ("Whitened", false);
//					GetComponent<SpriteRenderer> ().color = orig;
//				}
				yield return new WaitForSeconds (0.1f);
			}
//		} else {
//			for (int i = 0; i < 2; i++) {
//				//flip visibility on and off
//				GetComponent<SpriteRenderer> ().enabled = !GetComponent<SpriteRenderer> ().enabled;
//		}
//	}
}
}
