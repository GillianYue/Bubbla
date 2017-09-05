using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	public static List<Color> bulletGauge;
	public Text lifeText;
	public GameObject PaintSpriteObj, BulletGaugeObj, BulletObj, cannon;
	public List<GameObject> PaintSprites;
	public int bulletGaugeCapacity;
	public int maxLife;

	public float bulletSpeed;
	private int life;
	public GameControl gameControl;
	private AudioSource[] fire, ouch;

	//bulletSpeed is the absolute distance travelled per sec

	// Use this for initialization
	void Start ()
	{
		life = maxLife;
		bulletGauge = new List<Color> ();
		PaintSprites = new List<GameObject> ();

		fire = new AudioSource[5];
		ouch = new AudioSource[3];

		for (int i = 0; i < 5; i++) {
				fire.SetValue (GetComponents<AudioSource> ()[i], i);
			} 

		for(int i = 0; i<3; i++) {
				ouch.SetValue (GetComponents<AudioSource> ()[i+5], i);
			}



	}
	
	// Update is called once per frame
	void Update ()
	{
		lifeText.text = ("Life: " + life.ToString ());

		if (life < 1) {
			gameControl.gameOver();
		}

		gameControl.updateLife (life);

		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			transform.position += new Vector3 (1, 0, 0);
		}
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			transform.position += new Vector3 (-1, 0, 0); 
		}
		if (Input.GetKeyDown (KeyCode.UpArrow)) {
			transform.position += new Vector3 (0, 0, 1);
		}
		if (Input.GetKeyDown (KeyCode.DownArrow)) {
			transform.position += new Vector3 (0, 0, -1); 
		}



	}

	public bool addPaint(Color c){
		if (bulletGauge.Count < bulletGaugeCapacity) {
			bulletGauge.Add (c);
			addPaintSprite (c);
			return true;
		} else {
			print ("bulletGauge full");
			return false;
		}
	}

	//shoooot paint; this is called in GameControl's update
	public void launchBullet(Vector3 mouse){
		if (bulletGauge.Count > 0) {
			GameObject bullet = Instantiate (BulletObj, transform.position, 
				                   BulletObj.transform.rotation) as GameObject;
			fire[(int)(Random.Range(0, fire.Length-0.01f))].Play ();

			Vector3 direction = mouse - 
				Camera.main.WorldToScreenPoint(transform.position);
			float tan = direction.x / direction.y;
			float angle = Mathf.Atan (tan);

			bullet.GetComponent<Rigidbody> ().
			velocity = new Vector3 (((direction.y>0)? 1:-1) * Mathf.Sin(angle)*bulletSpeed, 0,
				Mathf.Cos(angle)*bulletSpeed);

			bullet.GetComponent<SpriteRenderer> ().color 
			= bulletGauge [bulletGauge.Count - 1];
			bullet.transform.Rotate (new Vector3(0,0,
				((direction.y>0)? -1:1) * Mathf.Rad2Deg*angle));
			removePaint ();
		}
	}

	public void damage(int damage){
		life -= damage;
		StartCoroutine (damageVFX ());
		ouch [(int)(Random.Range (0, ouch.Length - 0.01f))].Play ();
	}

	IEnumerator damageVFX(){
		for (int i = 0; i < 6; i++) {
			//flip visibility for three times
		GetComponent<SpriteRenderer> ().enabled = !GetComponent<SpriteRenderer> ().enabled;

			yield return new WaitForSeconds (0.1f);
		}
	}

	public void cure(int addition){
		life += addition;
	}

	public void respawn(){
		life = maxLife;
		clearPaint ();
	}

	private void clearPaint(){
		for (int c = 0; c < bulletGauge.Count; c++) {
			removePaint ();
		}
	}

	private void removePaint(){
			//removes both the color in list and the sprite
			bulletGauge.RemoveAt(bulletGauge.Count-1);
			removePaintSprite();

	}

	private void addPaintSprite(Color c){
		if (PaintSprites.Count < bulletGaugeCapacity) {
			
			GameObject ps = PaintSpriteObj;
			PaintSprites.Add (Instantiate (ps,
				cannon.transform.position
				+ new Vector3 (0.01f+
					0.03f*(bulletGauge.Count==1 ? 1:0), 
					-0.01f, 
					//to ensure pbSprite appears BELOW cannon img
					(bulletGauge.Count-2)*0.96f), 
				Quaternion.FromToRotation (Vector3.up,
						Vector3.forward)) as GameObject);
			
			PaintSprites[PaintSprites.Count-1].transform.parent 
			= BulletGaugeObj.transform;
			PaintSprites[PaintSprites.Count-1].GetComponent
			<Renderer> ().materials [0].color = c;

		} else {
			print ("bulletGauge sprite full");
		}
	}

	private void removePaintSprite(int whichOne){
		if (PaintSprites.Count > whichOne) {
			Destroy(PaintSprites[whichOne]);
			PaintSprites.RemoveAt (whichOne);
		} else {
			print ("removePaintSprite out of index");
		}
	}

	//default: removes the last one
	private void removePaintSprite(){
		Destroy(PaintSprites[PaintSprites.Count - 1]);
		PaintSprites.RemoveAt (PaintSprites.Count - 1);
	}

	public void printPlayer(){
		print (transform.position +
		Camera.main.WorldToScreenPoint
						(transform.position));
					
				}

	public int getMaxLife(){
		return maxLife;
	}
}

