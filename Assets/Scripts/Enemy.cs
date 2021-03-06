﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	GameControl gameControl;
	int life = 1;
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
	float projectileSpeed, projectileAccl, shootInterval, shootNoise, shootChanceIndividual;

	bool willShoot; //determined by shootChanceEnemy in csv 
	bool genProjectile;
	Sprite projectileSprite;
	IEnumerator projectileProcess;

	public EnemySteering steer;

	public bool sprite_on_child; //whether the visual representation of this enemy is on its child


	void Start () {
		if(!audioz) audioz = GameObject.FindWithTag ("AudioStorage").GetComponent<AudioStorage>();

        setSizeScale(sizeScale);

        setColliderScale(colliderScale);

		if (!steer) steer = GetComponent<EnemySteering>();
    }
	
	void Update () {
		if (life <= 0) {
			audioz.enemyDefeatedSE ();
			Destroy (gameObject);
		}


		//print("child sprite " + transform.GetChild(0).GetComponent<SpriteRenderer>().sprite.name);
	}

	public void passReferences(PrefabHolder ph, GameControl gc)
    {
		prefabHolder = ph;
		gameControl = gc;
    }

	public void setProjectile(GameObject prefab, Sprite spr, int p_attk, int p_type, float p_spd, float p_accl, float p_shoot_interval,
		float p_noise, float p_chance_ind, float p_chance_en)
	{
		if (projectileProcess != null) StopCoroutine(projectileProcess);

		myProjectilePrefab = prefab; //mold
		projectileSprite = spr;
		projectileAttack = p_attk; //for now set to enemy's own attack
		projectileType = p_type;
		projectileSpeed = p_spd;
		projectileAccl = p_accl;
		shootInterval = p_shoot_interval;
		shootNoise = p_noise;
		shootChanceIndividual = p_chance_ind;
		willShoot = Global.percentChance((int)(p_chance_en * 100));

		if (willShoot)
		{
			genProjectile = true;
			projectileProcess = launchProjectiles();
			StartCoroutine(projectileProcess);
		}
	}

	IEnumerator launchProjectiles()
    {
		//wait for 2 reasons: 1) enemy's position is set relatively late (might provide wrong source loc for projectile)
		//2) more fair to start launch projectile after enemy visually shows up in screen
		yield return new WaitUntil(() => Global.gameObjectInView(transform));
		while (true)
		{
			if (genProjectile && projectileSprite && Global.percentChance((int)(shootChanceIndividual * 100)))
			{
				GameObject p = Instantiate(myProjectilePrefab, this.transform.localPosition, myProjectilePrefab.transform.rotation);
				p.transform.parent = null;
				p.SetActive(true);
				p.GetComponent<SpriteRenderer>().sprite = projectileSprite;
				Destroy(p.GetComponent<CircleCollider2D>());
				CircleCollider2D circ = p.AddComponent<CircleCollider2D>(); //TODO check here; supposedly auto generates appropriately sized collider
				circ.isTrigger = true;

				p.transform.localScale = transform.lossyScale;

				projectile proj = p.GetComponent<projectile>();
				proj.damage = projectileAttack;
				proj.setSpeed(projectileSpeed);
				proj.setAcceleration(projectileAccl);

				Vector3 direction = Vector3.down; float angle = 0; bool rotateProjectile = false;
				switch (projectileType)
				{
					case 0:
						break;
					case 1:
						direction = gameControl.player.transform.position - transform.position;
						float tan = direction.x / direction.y;
						angle = Mathf.Atan(tan);
						if (direction.y > 0) Destroy(proj); //if player is above, this type of projectile will not be launched
						rotateProjectile = true;
						break;
				}

				if(proj) proj.setDirection(direction, angle, rotateProjectile);

			}

			//effectively, noise = 0.2 will make shoot time range from 0.8x to 1.2x
			yield return new WaitForSeconds(shootInterval + Random.Range(-shootNoise, shootNoise) * shootInterval);
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
			other.GetComponent<Player> ().damage (attack);
		}
	}

	IEnumerator damageVFXenemy(Color col){
			for (int i = 0; i < 2; i++) {
//				//flip visibility on and off
				GetComponent<SpriteRenderer> ().enabled = !GetComponent<SpriteRenderer> ().enabled; //TODO sprite might be on child

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

		if (!prefabHolder) prefabHolder = FindObjectOfType<PrefabHolder>();

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
		life -= 1; //TODO edit
				   //TODO sound effect
		audioz.burnSFX.Play();
		//TODO instantiate vfx here
		yield return new WaitForSeconds(2);
    }

	IEnumerator freezeDuration()
    {
		//sfx, vfx
		audioz.freezeSFX.Play();
		if(steer) steer.velocity *= 0.5f;
		yield return new WaitForSeconds(3);
		if (steer) steer.velocity /= 0.5f;
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
