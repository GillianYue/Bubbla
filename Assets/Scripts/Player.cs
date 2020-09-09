using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class Player : MonoBehaviour
{

	//Mode in which player's position is updated. ACCL refers to in-game movement, while touch refers to clicking on screen and play would move towards that direction in some velocity; freeze is self explanatory
    public enum Mode { ACCL, TOUCH, FREEZE };
    public Mode navigationMode;

    public static List<PaintballBehavior.ColorMode> bulletGauge;
	public Text lifeText;
	public GameObject BulletGaugeObj, BulletCont, BulletContCenter, BulletContBase; /* bullet container; base marks the top left corner of container for touch checking*/
	public GameObject[] bulletGaugeMasks; //pivots are children of obj "Pivots" in scene to help locate the top center of masks
	public RectTransform borderRect; //used to check for clicked bullet index
	public float bulletGaugeMaskMaxScaleY;

	//prefabs
	private GameObject PaintSpritePrefab, palletSelectedVFXPrefab;
	private GameObject[] BulletPrefabs;

	public List<GameObject> PaintSprites;
	public int bulletGaugeCapacity; // *number* of pbs allowed in the container
	public int bulletGaugeSelected = -1; //-1 == unselected; 0-2 corresponding to the slots
	public int[] bulletGaugeLimits, bulletGaugeContent; //upper bounds for gauge, and their current holding count
	//NOTE: for now, bulletGaugeLimits is an array of 60s, meaning that all slots have the same capacity

	public int maxLife;
    public Rigidbody2D playerRB;
	public bool canShoot = true, //bool for firing at a rate
		canSelect = true, 
		canMove = true; //toggled by UI bulletGauge select

	private GameObject palletSelectedVFX;
	private Vector3[] slotPositions; //world locations to place the paint sprites; initialized on start

    private float bulletWeaponDist = 2;
	private int life;

    [Inject(InjectFrom.Anywhere)]
    public GameControl gameControl;
	private AudioSource[] fire, ouch;

	[Inject(InjectFrom.Anywhere)]
	public PrefabHolder prefabHolder;

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
		bulletGauge = new List<PaintballBehavior.ColorMode> ();
		PaintSprites = new List<GameObject> ();

		slotPositions = new Vector3[bulletGaugeCapacity];
		bulletGaugeLimits = new int[bulletGaugeCapacity];



		for(int i = 0; i < bulletGaugeCapacity; i++)
        {
			bulletGaugeLimits[i] = 60;
			slotPositions[i] = new Vector3(12.8f,
					//to ensure pbSprite appears BELOW cannon img
					(i == 2 || i == 1) ? ((i == 2) ? 39.0f : 25.0f) : 10.5f, 0.01f);
		}
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

		bulletGaugeMaskMaxScaleY = bulletGaugeMasks[0].transform.localScale.y;

		foreach(GameObject m in bulletGaugeMasks)
        {
			m.transform.localScale = new Vector3(m.transform.localScale.x, 0.0001f, m.transform.localScale.z);
        }


		//locate prefabs
		PaintSpritePrefab = prefabHolder.gaugePaintSprite;
		palletSelectedVFXPrefab = prefabHolder.gaugeSelectedHalo;
		BulletPrefabs = prefabHolder.pallets;


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
		if (canShoot && canMove)
		{
			StartCoroutine(FireRate());
		}
    }

	//handles animation and shoots bullets at proper rate
	IEnumerator FireRate()
	{
		setShootAnimation(true);

		canShoot = false; 

		launchBullet(new Vector3(0, 1), 0, 0, true);

		yield return new WaitForSeconds(0.1f);
		canShoot = true;

		setShootAnimation(false);
	}

	/*
     * Instantaneously move to location on screen, used by gameControl in game mode to move player wherever pressed
     */
	public void moveTo(float mouseX, float mouseY)
	{
		if (canMove) { 
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		Vector3 to = Global.mainCamera.ScreenToWorldPoint(new Vector3(mouseX, mouseY, 5)); //TODO that 5
		rb.MovePosition(to);
		}
	}

	public void freezeMoveToBriefly()
    {
		StartCoroutine(shortPauseOnMoveTo());
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

			RectTransform r = borderRect;
			float sc_x = r.localScale.x, sc_y = r.localScale.y;
			float w = r.rect.width * sc_x;
			float h = r.rect.height * sc_y;
			Vector2 p = Global.WorldToScreen(BulletContCenter.transform.position);

			int index = -1;

			bool hit = Global.touching(touchPos, p, w, h);

		if (canSelect)
		{
			canSelect = false;

			if (hit)
			{

				index = whichGauge(touchPos);

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

	public int whichGauge(Vector2 touchPos)
    {
		Vector2 b = Global.WorldToScreen(BulletContBase.transform.position);
		float h = borderRect.rect.height * borderRect.localScale.y;

		int index = -1;

		if (touchPos.y >= b.y + h / bulletGaugeCapacity * 2)
		{
			index = 2;
		}
		else if (touchPos.y >= b.y + h / bulletGaugeCapacity)
		{
			index = 1;
		}
		else
		{
			index = 0;
		}


		return index;
	}

	private void selectBulletSlot(int index)
    {
		if (bulletGaugeSelected == index) {
			deselectBulletSlot();
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

	private void deselectBulletSlot()
    {
		bulletGaugeSelected = -1;
		if (palletSelectedVFX) Destroy(palletSelectedVFX);
	}

	public bool addPaint(PaintballBehavior.ColorMode c, int capacity){
		//first check for space within slots that are already occupied
		int remainder = capacity; //indicates extra amount that might/not be added as a new slot of the same color

		if(bulletGauge.Count != 0)
        {
			for(int index=0; index<bulletGauge.Count; index++)
            {
                if (c.Equals(bulletGauge[index]))
                {
					remainder = incrementGaugeCapacity(index, remainder);
					if(remainder == 0) break;
                }
            }
        }

		if (bulletGauge.Count < bulletGaugeCapacity) {
			if (remainder != 0)
			{
				bulletGauge.Add(c); //means no same color found/no space left --> definitely needs a new slot --> and there is a new slot
				int index = bulletGauge.Count - 1;

				setGaugeContent(index, remainder);
				addPaintSprite(PaintballBehavior.colorDict[c]);
			}
			return true;
		} else {
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

		//actual shooting of the pellets
		if (infinite) {

			//normal attack
			Vector3 pos = transform.GetComponent<RectTransform>().position;
			pos.x += (direction.y > 0 ? bulletWeaponDist : -bulletWeaponDist) * //TODO the 32 looks fishy here
				Mathf.Sin(angle) * (32 * Global.STWfactor.x);
			pos.y += (direction.y > 0 ? bulletWeaponDist : -bulletWeaponDist) *
				Mathf.Cos(angle) * (32 * Global.STWfactor.y);
			pos.z = 5;
			//from cannon's position plus a little bit of delta x and y to find the firing pos

			GameObject bullet = Instantiate(BulletPrefabs[bulletType], pos,
								   BulletPrefabs[bulletType].transform.rotation) as GameObject;

			//Debug.Log("shooting one bullet up: dir" + direction + " angle " + angle + " bulletType " + bulletType);
			if(!infinite) fire[(int)(Random.Range(0, fire.Length-0.01f))].Play (); //sound

			MyBullet bulletScript = bullet.GetComponent<MyBullet>();
			bulletScript.setVelocity(direction, angle);


			//apply additional effects
			if (bulletGaugeSelected != -1)
			{
				//applying special effects to attack; subtracting from the gauge of that pb

				bulletScript.setBulletColor(bulletGauge[bulletGaugeSelected]);

				decrementGaugeCapacity(bulletGaugeSelected, 1);

				if(bulletGaugeContent[bulletGaugeSelected] <= 0) //exhaust
                {
					removePaint(bulletGaugeSelected);
				}

            }
            else
            {
				bulletScript.setBulletColor(PaintballBehavior.ColorMode.NON);
            }

		}
		//is interrupted, aiming animation can still transition back to normal
	}

	//this happens when pressed for extended amount of time; prereq is that bullets have sim color
	//TODO revise; esp the removePaint() part without parameter
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

			float d = Global.find2ColorDist (PaintballBehavior.colorDict[bulletGauge[bulletGauge.Count - 1]],
						  PaintballBehavior.colorDict[bulletGauge [bulletGauge.Count - 2]]);
		//actual shooting
			if (d<3) { //if colors r actually close enough
			Vector3 pos = transform.position;
			pos.x += (direction.y>0 ? 1:-1) *  //TODO fishy 32
				Mathf.Sin (angle) * (32 * Global.STWfactor.x);
			pos.z += (direction.y>0 ? 1:-1) *
				Mathf.Cos (angle) * (32 * Global.STWfactor.y);
			//from cannon's position plus a little bit of delta x and y to find the firing pos

			GameObject bullet = Instantiate (BulletPrefabs[bulletType], pos,
				BulletPrefabs[bulletType].transform.rotation) as GameObject;
			fire[(int)(Random.Range(0, fire.Length-0.01f))].Play (); //sound

				bullet.GetComponent<MyBullet>().setVelocity(direction, angle);

				Color c1 = PaintballBehavior.colorDict[bulletGauge [bulletGauge.Count - 1]];
				Color c2 = PaintballBehavior.colorDict[bulletGauge [bulletGauge.Count - 2]];
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

	//removes topmost pb
	private void removePaint(){
			//removes both the color in list and the sprite
			bulletGauge.RemoveAt(bulletGauge.Count-1);
			removePaintSprite();
		deselectBulletSlot();
	}

	public void removePaint(int index)
	{
		//removes both the color in list and the sprite
		bulletGauge.RemoveAt(index);
		removePaintSprite(index);
		deselectBulletSlot();

		if(index != bulletGaugeCapacity - 1) //means not removing the topmost pb
        {
			//repositioning
			for(int i=index; i<bulletGauge.Count; i++)
            {
				PaintSprites[i].transform.localPosition = slotPositions[i];
				setGaugeContent(i, bulletGaugeContent[i + 1]);
				setGaugeContent(i + 1, 0);
			}
        }
	}



	public int getMaxLife(){
		return maxLife;
	}

    public void setNavigationMode(Mode mode)
    {
            navigationMode = mode;
    }

	//always call this method to update gauge content, as it updates the mask to show the right amount of paint accordingly
	public void setGaugeContent(int index, int capacity)
    {
		bulletGaugeContent[index] = capacity;
		refreshGaugeMask(index);

	}

	public void decrementGaugeCapacity(int index, int amount)
    {
		if (bulletGaugeContent[index] >= amount) bulletGaugeContent[index] -= amount;
		else
		{
			bulletGaugeContent[index] = 0;
		}

		refreshGaugeMask(index);
	}

	/// <summary>
	///  increments content of a slot with given index; will satisfy maximum constraints. 
	/// </summary>
	/// <param name="index"></param>
	/// <param name="amount"></param>
	/// <returns>the extra amount that was not added to the slot content due to maximum constraint </returns>
	public int incrementGaugeCapacity(int index, int amount)
	{
		int remainder = 0;
		if (bulletGaugeContent[index] + amount <= bulletGaugeLimits[index]) bulletGaugeContent[index] += amount;
		else
		{
			remainder = bulletGaugeContent[index] + amount - bulletGaugeLimits[index];
			bulletGaugeContent[index] = bulletGaugeLimits[index];
		}

		refreshGaugeMask(index);
		return remainder;
	}


	public void setShootAnimation(bool to)
    {
		anim.SetBool("aiming", to);
		Vector3 temp = transform.Find("Cannon").localEulerAngles;
		temp.z = to? 45 : 0;
		transform.Find("Cannon").localEulerAngles = temp;
	}


	// -----------------------------------------------------HELPER FUNCTIONS--------------------------------------------------


	//makes sure that the mask height is in sync with the actual amount of gauge left
	private void refreshGaugeMask(int index)
    {
		Vector3 s = bulletGaugeMasks[index].transform.localScale;
		//the float conversion of content and limit below is important; else it will get 0 results for anything other than 1
		Vector3 newScale = new Vector3(s.x, (1 - (float)bulletGaugeContent[index] / (float)bulletGaugeLimits[index]) * bulletGaugeMaskMaxScaleY, s.z);

        bulletGaugeMasks[index].transform.localScale = newScale;

	}

	private void addPaintSprite(Color c)
	{
		if (PaintSprites.Count < bulletGaugeCapacity)
		{

			GameObject ps = Instantiate(PaintSpritePrefab, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
			ps.transform.parent = BulletCont.transform; //bulletContainer

			PaintSprites.Add(ps);

			int index = bulletGauge.Count - 1; //spot to add to

			PaintSprites[PaintSprites.Count - 1].transform.localPosition = slotPositions[index];

			PaintSprites[PaintSprites.Count - 1].GetComponent<Renderer>().materials[0].color = c;

		}
		else
		{
			print("bulletGauge sprite full");
		}
	}

	private void removePaintSprite(int whichOne)
	{
		if (PaintSprites.Count > whichOne)
		{
			Destroy(PaintSprites[whichOne]);
			PaintSprites.RemoveAt(whichOne);
		}
		else
		{
			print("removePaintSprite out of index");
		}
	}

	//default: removes the last one
	private void removePaintSprite()
	{
		Destroy(PaintSprites[PaintSprites.Count - 1]);
		PaintSprites.RemoveAt(PaintSprites.Count - 1);
	}

	private IEnumerator shortPauseOnMoveTo()
    {
		canMove = false;
		yield return new WaitForSeconds(0.1f);
		canMove = true;
    }


	// -----------------------------------------------------END HELPER FUNCTIONS--------------------------------------------------
}

