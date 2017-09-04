using UnityEngine;
using System.Collections;


public class PaintballSpawner : MonoBehaviour {

	private int size;
	private Color color;
	public float sizeScale;
	private int myNumInList;
	public GameObject explosion;
	public GameObject absorption;
	public Sprite[] pbSprites, highlights;
	private int num;

	void Start () {
		Color rdmColor = new Color (Random.Range (0.0f, 1.0f),
			Random.Range (0.0f, 1.0f),
			Random.Range (0.0f, 1.0f),
			1);
		setColor (rdmColor);
		randomizeSpriteKind ();

		int rdmSize = (int) Random.Range (1.0f, 3.99f);
		setSize(rdmSize);

	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider other){

		//if paintball hit player, it bursts
		if (other.GetComponent<Collider>().tag == "Player"
			|| other.GetComponent<Collider>().tag == "Bullet") {
			if (explosion != null) {
		GameObject vfx = Instantiate 
					(explosion, transform.position, transform.rotation) as GameObject;
				vfx.GetComponent<SpriteRenderer> ().color = color;
				Destroy (gameObject);
			}
		}

	}
		
	public void getsAbsorbed(){
		GameObject vfx = Instantiate 
			(absorption, transform.position, transform.rotation) as GameObject;
		vfx.GetComponent<SpriteRenderer> ().color = color;
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

	public void randomizeSpriteKind(){
		num = (int)Random.Range (0, 3.99f);
		GetComponent<SpriteRenderer> ().sprite = pbSprites [num];
		transform.GetChild(0).gameObject.GetComponent<SpriteRenderer> ().sprite 
		= highlights [num];
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

	public int getNumInList(){
		return myNumInList;
	}

	public void setNumInList(int n){
		myNumInList = n;
	}
}


