using UnityEngine;
using System.Collections;


public class PaintballSpawner : MonoBehaviour {

	private Renderer rend;
	private int size;
	private Color color;
	public float sizeScale;
	private int myNumInList;
	public GameObject explosion;
	public GameObject absorption;

	void Start () {
		rend = GetComponent<Renderer> ();

		Color rdmColor = new Color (Random.Range (0.0f, 1.0f),
			Random.Range (0.0f, 1.0f),
			Random.Range (0.0f, 1.0f),
			1);
		setColor (rdmColor);

		int rdmSize = (int) Random.Range (1.0f, 3.99f);
		setSize(rdmSize);

	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnCollisionEnter(Collision other){

		//if paintball hit player, it bursts
		if (other.collider.tag == "Player" || other.collider.tag == "Bullet") {
			if (explosion != null) {
				Instantiate (explosion, transform.position, transform.rotation);
				Destroy (gameObject);
			}
		}

	}
		
	public void getsAbsorbed(){
		Instantiate (absorption, transform.position, transform.rotation);
		if (size - 1 == 0) {
			Destroy (gameObject);
		} else {
			setSize (size - 1);
		}
	}

	public void setColor(Color c){
		rend.materials[0].color = c;
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
		rend.transform.localScale = vector;
	}

	public int getNumInList(){
		return myNumInList;
	}

	public void setNumInList(int n){
		myNumInList = n;
	}
}


