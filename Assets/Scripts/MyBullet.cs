using UnityEngine;
using System.Collections;

public class MyBullet : MonoBehaviour {
	
	public float accl;
    public GameObject trailPrefab, trail;
    public int damage = 1;
    public float bulletSpeed;
    public PaintballBehavior.ColorMode myColor;

    public PrefabHolder prefabHolder;

    public void Start () {

	}

	public void Update () {

	}

    public void passPrefabHolder(PrefabHolder ph)
    {
        prefabHolder = ph;
    }

    public void setVelocity(Vector3 direction, float angle)
    {
        GetComponent<Rigidbody2D>().
            velocity = new Vector2(((direction.y > 0) ? 10 : -10) * Mathf.Sin(angle) * bulletSpeed,
                ((direction.y > 0) ? 10 : -10) * Mathf.Cos(angle) * bulletSpeed);

        transform.Rotate(new Vector3(0, 0,
                ((direction.y > 0) ? -1 : 1) * Mathf.Rad2Deg * angle));
    }

    public void FixedUpdate()
    {
        //acceleration
        GetComponent<Rigidbody2D>().velocity += new Vector2
            (GetComponent<Rigidbody2D>().velocity.x * accl,
                GetComponent<Rigidbody2D>().velocity.y * accl);
    }

    void OnTriggerEnter2D(Collider2D other){
        string t = other.GetComponent<Collider2D>().tag;
        //if bullet hits enemy, it bursts and damages enemy
        if (t == "Enemy") {

                Enemy e = other.GetComponent<Enemy>();

                switch (myColor)
                {
                    case PaintballBehavior.ColorMode.RED:
                        if(Global.percentChance(10)) e.triggerBuff(Enemy.BuffMode.burn);
                        break;
                    case PaintballBehavior.ColorMode.BLUE:
                    if (Global.percentChance(10)) e.triggerBuff(Enemy.BuffMode.freeze);
                        break;
                    case PaintballBehavior.ColorMode.YELLOW:
                        break;
                    default:
                        break;

                }

				e.damage (damage, gameObject.GetComponent<SpriteRenderer>().color);
				Destroy (gameObject);

		}else if(t == "Boss")
        {
            other.transform.parent.GetComponent<BossBehavior>().damage(damage,
        gameObject.GetComponent<SpriteRenderer>().color);
            Destroy(gameObject);
        }

	}


    //also instantiates trail of the corresponding color here
    public void setBulletColor(PaintballBehavior.ColorMode col)
    {
        Color c = PaintballBehavior.colorDict[col];
        GetComponent<SpriteRenderer>().color = c;
        myColor = col;

        switch (myColor)
        {
            case PaintballBehavior.ColorMode.RED:
                trailPrefab = prefabHolder.palletTrailRed;
                break;
            case PaintballBehavior.ColorMode.BLUE:
                trailPrefab = prefabHolder.palletTrailBlue;
                break;
            case PaintballBehavior.ColorMode.YELLOW:
                break;
            default:
                trailPrefab = prefabHolder.palletTrail;
                break;

        }

        trail = Instantiate(trailPrefab, gameObject.transform.position,
            trailPrefab.transform.rotation) as GameObject;
        /*        trail.transform.parent = gameObject.transform;
                trail.transform.localScale = Vector3.one;*/
        trail.GetComponent<TrailFollowBall>().setMyBullet(gameObject);
    }

    void OnDestroy()
    {
      //  if (transform.childCount != 0) transform.GetChild(0).parent = null; //detach child trail GO
    }
}