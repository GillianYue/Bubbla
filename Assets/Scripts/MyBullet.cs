using UnityEngine;
using System.Collections;

public class MyBullet : MonoBehaviour {
	
	public float accl;
	public GameObject trail;
    public int damage = 1;
    public float bulletSpeed;
    public PaintballBehavior.ColorMode myColor;

    [Inject(InjectFrom.Anywhere)]
    public PrefabHolder prefabHolder;

    public void Start () {
        if (trail != null)
        {
            GameObject t = trail;
            t = Instantiate(trail, gameObject.transform.position,
                trail.transform.rotation) as GameObject;
            t.GetComponent<TrailFollowBall>().setMyBullet(gameObject);
        }
	}

	public void Update () {

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

                GameObject explosionPrefab;
                Enemy e = other.GetComponent<Enemy>();

                switch (myColor)
                {
                    case PaintballBehavior.ColorMode.RED:
                        explosionPrefab = prefabHolder.palletExplosionRed;
                        e.triggerBuff(Enemy.BuffMode.burn);
                        break;
                    case PaintballBehavior.ColorMode.BLUE:
                        explosionPrefab = prefabHolder.palletExplosionBlue;
                        e.triggerBuff(Enemy.BuffMode.freeze);
                        break;
                    case PaintballBehavior.ColorMode.YELLOW:
                    explosionPrefab = prefabHolder.palletExplosionYellow;
                        break;
                    default:
                        explosionPrefab = prefabHolder.palletExplosion;
                        break;

                }
				Instantiate (explosionPrefab, transform.position, transform.rotation);
				e.damage (damage, 
					gameObject.GetComponent<SpriteRenderer>().color);
				Destroy (gameObject);
		}else if(t == "Boss")
        {
            other.transform.parent.GetComponent<BossBehavior>().damage(damage,
        gameObject.GetComponent<SpriteRenderer>().color);
            Destroy(gameObject);
        }

	}

    public void setBulletColor(PaintballBehavior.ColorMode col)
    {
        GetComponent<SpriteRenderer>().color = PaintballBehavior.colorDict[col];
        myColor = col;
    }
}
