using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	GameControl gameControl;
	int life = 1000;
	public int attack;

    public AudioStorage audioz;

    public float sizeScale; //same as paintball, base scale to be multiplied with global
    public float colliderScale; //multiplied to the original generated polygon 2d collider shape to resize

	public enum BuffMode { burn, freeze, none };
	public BuffMode debuff = BuffMode.none;
	IEnumerator currBuffProcess;

	private PrefabHolder prefabHolder;
	private GameObject myProjectilePrefab; //has projectile script attached
	int projectileType, projectileAttack;
	float projectileSpeed, projectileAccl, shootInterval;
	bool genProjectile;
	Sprite projectileSprite;
	IEnumerator projectileProcess;


	void Start () {
		audioz = GameObject.FindWithTag ("AudioStorage").GetComponent<AudioStorage>();

        setSizeScale(sizeScale);

        setColliderScale(colliderScale);

    }
	
	void Update () {
		if (life <= 0) {
			audioz.enemyDefeatedSE ();
			Destroy (gameObject);
		}
	}

	public void passReferences(PrefabHolder ph, GameControl gc)
    {
		prefabHolder = ph;
		gameControl = gc;
    }

	public void setProjectile(GameObject prefab, Sprite spr, int p_attk, int p_type, float p_spd, float p_accl, float p_shoot_interval)
	{
		if (projectileProcess != null) StopCoroutine(projectileProcess);

		myProjectilePrefab = prefab; //mold
		projectileSprite = spr;
		projectileAttack = p_attk; //for now set to enemy's own attack
		projectileType = p_type;
		projectileSpeed = p_spd;
		projectileAccl = p_accl;
		shootInterval = p_shoot_interval;

		genProjectile = true;
		projectileProcess = launchProjectiles();
		StartCoroutine(projectileProcess);
	}

	IEnumerator launchProjectiles()
    {
		while (true)
		{
			if (genProjectile && projectileSprite)
			{
				GameObject p = Instantiate(myProjectilePrefab, this.transform.position, myProjectilePrefab.transform.rotation);
				p.GetComponent<SpriteRenderer>().sprite = projectileSprite;
				Destroy(p.GetComponent<CircleCollider2D>());
				p.AddComponent<CircleCollider2D>(); //TODO check here; supposedly auto generates appropriately sized collider

				p.transform.parent = null;
				p.transform.localScale = transform.localScale;
				Debug.Log("curr projectile scale " + p.transform.localScale);
				p.SetActive(true);
				projectile proj = p.GetComponent<projectile>();
				proj.damage = projectileAttack;
				proj.setSpeed(projectileSpeed);
				proj.setAcceleration(projectileAccl);

				Vector3 direction = Vector3.down; float angle = 0;
				switch (projectileType)
				{
					case 0:
						break;
					case 1:
						direction = transform.position - gameControl.player.transform.position;
						float tan = direction.x / direction.y;
						angle = Mathf.Atan(tan);
						break;
				}

				proj.setDirection(direction, angle);

			}

			yield return new WaitForSeconds(shootInterval);
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
		debuff = b; GameObject explosionPrefab;
		bool parentToOther = false;

		switch (b)
        {
			case BuffMode.burn:
				explosionPrefab = prefabHolder.palletExplosionRed;
				parentToOther = true;

				currBuffProcess = burnDuration();
				StartCoroutine(currBuffProcess);
				break;
			case BuffMode.freeze:
				explosionPrefab = prefabHolder.palletExplosionBlue;
				parentToOther = true;

				currBuffProcess = freezeDuration();
				StartCoroutine(currBuffProcess);
				break;
			default:
				Debug.Log("buff case not recognized");
				explosionPrefab = null;
				break;

		}

		GameObject effect = (explosionPrefab != null) ? Instantiate(explosionPrefab, transform.position, transform.rotation) : null;
		if (parentToOther) effect.transform.parent = transform;
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
