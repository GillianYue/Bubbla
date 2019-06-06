using UnityEngine;
using System.Collections;

public class PotionBehav : MonoBehaviour {

		private Color color;
		public GameObject explosion;
		public GameObject absorption;
		private int size;
		public float sizeScale;
	private int curingPotency;

		private AudioStorage audioz;

		void Start () {
		setColor (Color.white);
		size = 1;
			setSize(size);

		curingPotency = (int)Random.Range (3, 7.99f);
			audioz = GameObject.FindWithTag ("AudioStorage").GetComponent<AudioStorage>();
		}

		// Update is called once per frame
		void Update () {
		transform.Rotate (new Vector3 (0,0,1));
		}

		void OnTriggerEnter(Collider other){

			//if paintball hit player, it bursts
			if (other.GetComponent<Collider>().tag == "Player"
				|| other.GetComponent<Collider>().tag == "Bullet") {
				if (explosion != null) {
					GameObject vfx = Instantiate 
						(explosion, transform.position, transform.rotation) as GameObject;
					audioz.potionExplosionSE ();
					vfx.GetComponent<SpriteRenderer> ().color = color;
					Destroy (gameObject);
				}
			}

		}

	public int getCuringPotency(){
		return curingPotency;
	}

		public void getsAbsorbed(){
			GameObject vfx = Instantiate 
				(absorption, transform.position, transform.rotation) as GameObject;

			audioz.potionAbsorptionSE ();
			vfx.transform.localScale = new Vector3 (getScale(), getScale(), getScale());
			if (size - 1 == 0) {
				Destroy (gameObject);
			} else {
				setSize (size - 1);
			}
		}

		public void setColor(Color c){
			GetComponent<SpriteRenderer> ().color = c;
			color = c;
		}

		public Color getColor(){
			return color;
		}

		public void setSize(int s){
			size = s;
			changeScale (new Vector3 
				((size) * sizeScale,
					(size) * sizeScale,(size) * sizeScale));
		}

		public float getScale(){
			return size*sizeScale;
		}

		public void changeScale(Vector3 vector){
			transform.localScale = vector;
		}

	}


