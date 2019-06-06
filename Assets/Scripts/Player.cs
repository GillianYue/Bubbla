using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
	public static List<Color> bulletGauge;
	public Text lifeText;
	public GameObject PaintSpriteObj, BulletGaugeObj, BulletObj, BulletCont; /* bullet container*/
	public List<GameObject> PaintSprites;
	public int bulletGaugeCapacity;
	public int maxLife;

	public float bulletSpeed;
	private int life;
	public GameControl gameControl;
	private AudioSource[] fire, ouch;

	//those are relative to player, since Cannon(player's cannon) is a child of player
	private Vector3 CannNormStart = new Vector3(0.01f, -0.27f, 3.33f), 
	CannLShoot = new Vector3(-0.05f, 0.06f, 3.33f), 
	CannRShoot = new Vector3(0.07f,0.09f,3.33f);

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


	public void endShootStatus(){
		GetComponent<Animator> ().SetBool ("Shoot", false);

	}


	public void cannonSpriteDeltaY(float deltaY){
		Vector3 temp = this.gameObject.transform.Find ("Cannon").localPosition;
		temp.y += deltaY;
		this.gameObject.transform.Find ("Cannon").localPosition = temp;
	}

	public void cannonSpriteDeltaX(float deltaX){
		Vector3 temp = this.gameObject.transform.Find ("Cannon").localPosition;
		temp.x += deltaX;
		this.gameObject.transform.Find ("Cannon").localPosition = temp;
	}

	public void setCannonSpritePos(int state){
		Transform cann = this.transform.Find ("Cannon");
		Vector3 temp;
		switch (state) {
		case 0:
			temp = CannNormStart;
			Quaternion rot = new Quaternion (0, 0, 0, 0);
			cann.localRotation = rot;
			break;
		case 1:
			temp = CannLShoot;
			break;
		case 2:
			temp = CannRShoot;
			break;
		default:
			temp = CannNormStart;
			break;
		}
		cann.localPosition = temp;
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

	//shoooot paint; this is called in GameControl's update; just one single shot
	public void launchBullet(Vector3 direction, float angle){
		//FIRST, play the animation, NO MATTER if a bullet is actually shot
		Animator anim = GetComponent<Animator> ();
		anim.SetTrigger ("shoot");
		if (anim.GetBool ("Shoot") == true) {
			//reset the animation clip if multiple shots are happening
			anim.Play ("B_ShootLeft", -1, 0f);
		}
		anim.SetBool ("Shoot", true);

		//actual shooting
		if (bulletGauge.Count > 0) {
			Vector3 pos = transform.position;
			pos.x += (direction.y>0 ? 1:-1) * 
				Mathf.Sin (angle) * (32 * CONSTANTS.PixelToWorldFactor.x);
			pos.z += (direction.y>0 ? 1:-1) *
				Mathf.Cos (angle) * (32 * CONSTANTS.PixelToWorldFactor.y);
			//from cannon's position plus a little bit of delta x and y to find the firing pos

			GameObject bullet = Instantiate (BulletObj, pos,
				                   BulletObj.transform.rotation) as GameObject;
			fire[(int)(Random.Range(0, fire.Length-0.01f))].Play (); //sound

			bullet.GetComponent<Rigidbody> ().
			velocity = new Vector3 (((direction.y>0)? 1:-1) * Mathf.Sin(angle)*bulletSpeed, 0,
				((direction.y>0)? 1:-1) * Mathf.Cos(angle)*bulletSpeed);

			bullet.GetComponent<SpriteRenderer> ().color 
			= bulletGauge [bulletGauge.Count - 1];
			bullet.transform.Rotate (new Vector3(0,0,
				((direction.y>0)? -1:1) * Mathf.Rad2Deg*angle));
			removePaint ();
		}
		//is interrupted, aiming animation can still transition back to normal
	}

	//this happens when pressed for extended amount of time; prereq is that bullets have sim color
	public void launch2Bullets(Vector3 direction, float angle){
		if (bulletGauge.Count <= 1) {
			launchBullet (direction, angle);
		}else{
		//FIRST, play the animation for shooting two at the same time, NO MATTER
		Animator anim = GetComponent<Animator> ();
		anim.SetTrigger ("shoot");
		if (anim.GetBool ("Shoot") == true) {
			//reset the animation clip if multiple shots are happening
			anim.Play ("B_ShootLeft", -1, 0f);
		}
		anim.SetBool ("Shoot", true);

			float d = PaintballSpawner.find2ColorDist (bulletGauge [bulletGauge.Count - 1],
				          bulletGauge [bulletGauge.Count - 2]);
			print ("dist bt 2:"+d);
		//actual shooting
			if (d<3) { //if colors r actually close enough
			Vector3 pos = transform.position;
			pos.x += (direction.y>0 ? 1:-1) * 
				Mathf.Sin (angle) * (32 * CONSTANTS.PixelToWorldFactor.x);
			pos.z += (direction.y>0 ? 1:-1) *
				Mathf.Cos (angle) * (32 * CONSTANTS.PixelToWorldFactor.y);
			//from cannon's position plus a little bit of delta x and y to find the firing pos

			GameObject bullet = Instantiate (BulletObj, pos,
				BulletObj.transform.rotation) as GameObject;
			fire[(int)(Random.Range(0, fire.Length-0.01f))].Play (); //sound

			bullet.GetComponent<Rigidbody> ().
			velocity = new Vector3 (((direction.y>0)? 1:-1) * Mathf.Sin(angle)*bulletSpeed, 0,
				((direction.y>0)? 1:-1) * Mathf.Cos(angle)*bulletSpeed);

				Color c1 = bulletGauge [bulletGauge.Count - 1];
				Color c2 = bulletGauge [bulletGauge.Count - 2];
			bullet.GetComponent<SpriteRenderer> ().color 
				= new Color ((c1.r+c2.r)/2, (c1.g+c2.g)/2, (c1.b+c2.b)/2);
			bullet.transform.Rotate (new Vector3(0,0,
				((direction.y>0)? -1:1) * Mathf.Rad2Deg*angle));
			removePaint ();
			removePaint ();
		}
		//is interrupted, aiming animation can still transition back to normal
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
		if (life + addition <= maxLife) {
			life += addition;
		} else {
			life = maxLife;
		}
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
				BulletCont.transform.position
				+ new Vector3 (0.01f
					+0.01f*((bulletGauge.Count==2||bulletGauge.Count==1) ? 1f:0), 
					-0.01f, 
					//to ensure pbSprite appears BELOW cannon img
					(bulletGauge.Count-2)*0.85f+0.0057f), 
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

	public int getMaxLife(){
		return maxLife;
	}


}

