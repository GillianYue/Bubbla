using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	public static List<Color> bulletGauge;
	public Text BGText;
	public double spaceBtPaintSprites;
	public GameObject PaintSpriteObj, BulletGaugeObj, BulletObj;
	public List<GameObject> PaintSprites;
	public int bulletGaugeCapacity, bulletSpeed;
	//bulletSpeed is the absolute distance travelled per sec

	// Use this for initialization
	void Start ()
	{
		bulletGauge = new List<Color> ();
		BGText.text = "BulletGauge";
		PaintSprites = new List<GameObject> ();
	}
	
	// Update is called once per frame
	void Update ()
	{

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
		
			Vector3 direction = mouse - 
				Camera.main.WorldToScreenPoint(transform.position);
			float tan = direction.x / direction.y;
			float angle = Mathf.Atan (tan);

			bullet.GetComponent<Rigidbody> ().
			velocity = new Vector3 (((direction.y>0)? 1:-1) * Mathf.Sin(angle)*bulletSpeed, 0,
				Mathf.Cos(angle)*bulletSpeed);

			print (bullet.GetComponent<Rigidbody> ().
				velocity+" "+direction+" sin"+Mathf.Sin(angle));
			bullet.GetComponent<Renderer> ().materials [0].color 
			= bulletGauge [bulletGauge.Count - 1];
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
				BGText.transform.position
			+ new Vector3 ((float)(1.5 //text space
			+ (bulletGauge.Count + 1) * spaceBtPaintSprites //mid space
			+ bulletGauge.Count * 1) //ball space
					, 0, 0), Quaternion.FromToRotation (Vector3.up,
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

		void printPlayer(){
		print (transform.position +
		Camera.main.WorldToScreenPoint
						(transform.position));
					
				}
}

