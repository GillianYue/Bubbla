using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

	//Mode in which player's position is updated. ACCL refers to in-game movement, while touch refers to clicking on screen and play would move towards that direction in some velocity; freeze is self explanatory
    public enum Mode { ACCL, TOUCH, FREEZE };
    public Mode navigationMode;

    public static List<Color> bulletGauge;
	public Text lifeText;
	public GameObject PaintSpriteObj, BulletGaugeObj, BulletCont, BulletContCenter, BulletContBase; /* bullet container; base marks the top left corner of container for touch checking*/
	public GameObject[] BulletObj;
	public List<GameObject> PaintSprites;
	public int bulletGaugeCapacity; // *number* of pbs allowed in the container
	public int bulletGaugeSelected = -1; //-1 == unselected; 0-2 corresponding to the slots
	public int[] bulletGaugeLimits, bulletGaugeContent; //upper bounds for gauge, and their current holding count

	public int maxLife;
    public Rigidbody2D playerRB;
	public bool canShoot = true, canSelect = true; //bool for firing at a rate
	public GameObject palletSelectedVFXPrefab;
	private GameObject palletSelectedVFX;

	public float bulletSpeed;
    private float bulletWeaponDist = 2;
	private int life;

    [Inject(InjectFrom.Anywhere)]
    public GameControl gameControl;
	private AudioSource[] fire, ouch;

	private Animator anim;

	public bool checkForUpdates = true, invincible; //updates life UI and checks for life if true


	//those are relative to player, since Cannon(player's cannon) is a child of player
	private Vector3 CannNormStart = new Vector3(1.36f, 14.89f, 3.33f), 
	CannLShoot = new Vector3(-3.82f, 6.45f, 3.33f), 
	CannRShoot = new Vector3(2.68f,2.75f,3.33f);

	//bulletSpeed is the absolute distance travelled per sec

	// Use this for initialization
	void Start ()
	{
		life = maxLife;
		bulletGauge = new List<Color> ();
		PaintSprites = new List<GameObject> ();

		bulletGaugeLimits = new int[bulletGaugeCapacity];
		bulletGaugeContent = new int[bulletGaugeCapacity];

		fire = new AudioSource[5];
		ouch = new AudioSource[3];

		for (int i = 0; i < 5; i++) {
				fire.SetValue (GetComponents<AudioSource> ()[i], i);
			} 

		for(int i = 0; i<3; i++) {
				ouch.SetValue (GetComponents<AudioSource> ()[i+5], i);
			}

        playerRB = GetComponent<Rigidbody2D>();

		anim = GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update ()
	{

		lifeText.text = ("Life: " + life.ToString ());

        if (checkForUpdates) {
            if (life < 1)
            {
                gameControl.gameOver();
            }
            else
            {
                if(navigationMode == Mode.ACCL)
                {
                    transform.Translate(Input.acceleration.x, 0, 0);
                }
                else if(navigationMode == Mode.TOUCH)
                {
                    //GameControl will send touch along if it's valid, nudge() will be called
                }
                if(gameControl.sceneType == GameControl.Mode.GAME) //TODO iffy way of dealing with this here
                gameControl.updateLife(life);


                ////////// just for testing purposes
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    transform.position += new Vector3(20, 0, 0);
                }
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    transform.position += new Vector3(-20, 0, 0);
                }
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    transform.position += new Vector3(0, 20, 0);
                }
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    transform.position += new Vector3(0, -20, 0);
                }
                //////////

            }
        }

	}

    public void fireAtRate(Vector3 pos)
    {
		if (canShoot) StartCoroutine(FireRate());
    }


	IEnumerator FireRate()
	{
		anim.SetBool("aiming", true);
		canShoot = false;
		launchBullet(new Vector3(0, 1), 0, 0, true);
		yield return new WaitForSeconds(0.1f);
		canShoot = true;
		anim.SetBool("aiming", false);
	}

	/*
     * Instantaneously move to location on screen, used by gameControl in game mode to move player wherever pressed
     */
	public void moveTo(float mouseX, float mouseY)
    {
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		Vector3 to = Global.mainCamera.ScreenToWorldPoint(new Vector3(mouseX, mouseY, 5)); //TODO that 5
		rb.MovePosition(to);
	}

    public void nudge()
    {
        if(navigationMode == Mode.TOUCH)
        StartCoroutine(nudgeWhilePressed()); //once this process starts, it checks for complete (mouse up) on it own
    }

    private IEnumerator nudgeWhilePressed()
    {
        yield return new WaitUntil(() => //delegate called after each Update()
        {
            Vector2 mouseInWorld = Global.ScreenToWorld(Input.mousePosition);
            Global.nudgeTowards(gameObject, (int)mouseInWorld.x, (int)mouseInWorld.y, 5);
            if (Input.GetMouseButtonUp(0))
            {
                return true;
            }
            else
                return false;
        });
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

	public bool selectGauge(Vector2 touchPos) //touchPos is screen
    {


			RectTransform r = BulletCont.GetComponent<RectTransform>();
			float sc_x = r.localScale.x, sc_y = r.localScale.y;
			float w = r.rect.width * sc_x;
			float h = r.rect.height * sc_y;
			Vector2 p = Global.WorldToScreen(BulletContCenter.transform.position);
			Vector2 b = Global.WorldToScreen(BulletContBase.transform.position);
			int index = -1;

			bool hit = Global.touching(touchPos, p, w, h);

		if (canSelect)
		{
			canSelect = false;

			if (hit)
			{

				if (touchPos.y >= b.y + h / 3 * 2)
				{
					index = 2;
				}
				else if (touchPos.y >= b.y + h / 3)
				{
					index = 1;
				}
				else
				{
					index = 0;
				}

				selectBulletSlot(index);
			}

			StartCoroutine(Global.Chain(this, Global.WaitForSeconds(0.2f), Global.Do(() =>
			{
				canSelect = true;
			}))
			);

        }
		return hit;

	}
	private void selectBulletSlot(int index)
    {
		if (bulletGaugeSelected == index) { 
			bulletGaugeSelected = -1;
			if (palletSelectedVFX) Destroy(palletSelectedVFX);
		}
		else
		{
			if (bulletGauge.Count - 1 >= index)
			{
				bulletGaugeSelected = index;
				if (palletSelectedVFX) Destroy(palletSelectedVFX);
				palletSelectedVFX = Instantiate
			(palletSelectedVFXPrefab, PaintSprites[index].transform.position + new Vector3(0,2), PaintSprites[index].transform.rotation) as GameObject;
			}

		}
    }

	public bool addPaint(Color c, int capacity){
		if (bulletGauge.Count < bulletGaugeCapacity) {
			bulletGauge.Add (c);
			int index = bulletGauge.Count - 1;

/*			if (bulletGaugeContent[index]+capacity <= bulletGaugeLimits[index])
            {*/
				bulletGaugeContent[index] = capacity;
			Debug.Log("gauge " + index + " capacity is " + bulletGaugeContent[index]);
 //           }
			
			addPaintSprite (c);
			return true;
		} else {
			//TODO: check if one of the bullets is the color of pb, and add to remainder of gauge space

			print ("bulletGauge full");
			return false;
		}
	}

	//shoot paint; this is called in GameControl's update; just one single shot
	public void launchBullet(Vector3 direction, float angle, int bulletType, bool infinite){
		if (!infinite)
		{
			//play the animation regardless of if a bullet is actually shot
			anim.SetTrigger("shoot");
			if (anim.GetBool("Shoot") == true)
			{
				//reset the animation clip if multiple shots are happening
				anim.Play("B_ShootLeft", -1, 0f);
			}
			anim.SetBool("Shoot", true);
		}

		//actual shooting
		if (infinite) {
			if(bulletGaugeSelected != -1)
            {
				//apply special effects to attack; subtracting from the gauge of that pb

/*				if (bulletGauge.Count > 0)   //if gauge runs out, remove this pb
				{
                //    bullet.GetComponent<SpriteRenderer>().color = bulletGauge[bulletGauge.Count - 1]; 
                    removePaint();
				}*/
			}

			//normal attack
			Vector3 pos = transform.GetComponent<RectTransform>().position;
			pos.x += (direction.y > 0 ? bulletWeaponDist : -bulletWeaponDist) * //TODO the 32 looks fishy here
				Mathf.Sin(angle) * (32 * Global.STWfactor.x);
			pos.y += (direction.y > 0 ? bulletWeaponDist : -bulletWeaponDist) *
				Mathf.Cos(angle) * (32 * Global.STWfactor.y);
			pos.z = 5;
			//from cannon's position plus a little bit of delta x and y to find the firing pos

			GameObject bullet = Instantiate(BulletObj[bulletType], pos,
				                   BulletObj[bulletType].transform.rotation) as GameObject;

			//Debug.Log("shooting one bullet up: dir" + direction + " angle " + angle + " bulletType " + bulletType);
			if(!infinite) fire[(int)(Random.Range(0, fire.Length-0.01f))].Play (); //sound

			bullet.GetComponent<Rigidbody2D> ().
			velocity = new Vector2 (((direction.y>0)? 10:-10) * Mathf.Sin(angle)*bulletSpeed,
				((direction.y>0)? 10:-10) * Mathf.Cos(angle)*bulletSpeed);


			bullet.transform.Rotate (new Vector3(0,0,
				((direction.y>0)? -1:1) * Mathf.Rad2Deg*angle));

		}
		//is interrupted, aiming animation can still transition back to normal
	}

	//this happens when pressed for extended amount of time; prereq is that bullets have sim color
	public void launch2Bullets(Vector3 direction, float angle, int bulletType, bool infinite){
		if (bulletGauge.Count <= 1) {
			launchBullet (direction, angle, 0, infinite);
		}else{
		//FIRST, play the animation for shooting two at the same time, NO MATTER
		Animator anim = GetComponent<Animator> ();
		anim.SetTrigger ("shoot");
		if (anim.GetBool ("Shoot") == true) {
			//reset the animation clip if multiple shots are happening
			anim.Play ("B_ShootLeft", -1, 0f);
		}
		anim.SetBool ("Shoot", true);

			float d = Global.find2ColorDist (bulletGauge [bulletGauge.Count - 1],
				          bulletGauge [bulletGauge.Count - 2]);
		//actual shooting
			if (d<3) { //if colors r actually close enough
			Vector3 pos = transform.position;
			pos.x += (direction.y>0 ? 1:-1) *  //TODO fishy 32
				Mathf.Sin (angle) * (32 * Global.STWfactor.x);
			pos.z += (direction.y>0 ? 1:-1) *
				Mathf.Cos (angle) * (32 * Global.STWfactor.y);
			//from cannon's position plus a little bit of delta x and y to find the firing pos

			GameObject bullet = Instantiate (BulletObj[bulletType], pos,
				BulletObj[bulletType].transform.rotation) as GameObject;
			fire[(int)(Random.Range(0, fire.Length-0.01f))].Play (); //sound

			bullet.GetComponent<Rigidbody2D> ().
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
        if (!invincible)
        {
            life -= damage;
        }
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
			new Vector3(0,0,0),
new Quaternion(0,0,0,0)) as GameObject);
			
			PaintSprites[PaintSprites.Count-1].transform.parent 
			= BulletCont.transform; //bulletContainer
            PaintSprites[PaintSprites.Count - 1].transform.localPosition =
                new Vector3(12.8f,
                    //to ensure pbSprite appears BELOW cannon img
                    (bulletGauge.Count == 3 || bulletGauge.Count == 2) ?
                    ((bulletGauge.Count == 3) ? 39.0f : 25.0f) : 10.5f,
                   0.01f);

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

    public void setNavigationMode(Mode mode)
    {
            navigationMode = mode;
    }


}

