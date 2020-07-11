using UnityEngine;
using System.Collections;


public class PaintballBehavior : MonoBehaviour {
	//instantiated for each paintball
	private int size;
	private Color color;
    public bool needGenCol = true, //that color isn't set externally, and that calling genColorWDist is needed if false
    needSetSize = true;

    public float sizeScale;
    public float colliderScale; //multiplied to the original generated polygon 2d collider shape to resize

    private int myNumInList;
	public GameObject explosion;
	public GameObject absorption;
	public Sprite[] pbSprites, highlights;
	private int num;

	public static float mxD; //distance on 3D RGBcube to the given "standard" color
	public static Color standard; //a set base color w given range (above) to determine color for this pb

    public AudioStorage audioz;
	public GameObject myVFX;
	public float VFXtimer;
	

	/**
	 * this static method takes the parameter of a float between 0 and 442, and sets the 
	 * local mxD variable accordingly 
	 */
	public static void setMaxD(float m){
		mxD = (m / 255.0f);
	}


	void Start () {

        //based on the 3D RGB cube, generate colors close to given color within a 
        //certain 3D distance based on distance formula between two 3D points

        //		mxD = (200.0f / 255.0f); 
        //		//max distance possible: full color ranges: 441.6729 (from black corner to white corner)
        //		standard = Color.cyan; //a set "base" color

        audioz = GameObject.FindWithTag("AudioStorage").GetComponent<AudioStorage>();

        if (needGenCol) setColor (genColorWDist(mxD, standard));
		randomizeSpriteKind ();

        setSizeScale(1.8f);
        if (needSetSize)
        {
            int rdmSize = (int)Random.Range(1.0f, 3.99f);
            setSize(rdmSize);
        }
        else
        {
            setSize(size);
        }


        //resizing 2d polygon collider
        Vector2[] colliderPoints = GetComponent<PolygonCollider2D>().points;
        Vector2[] scaledPoints = new Vector2[colliderPoints.Length];
        for (int p = 0; p < colliderPoints.Length; p++)
        {
            Vector2 np = GetComponent<PolygonCollider2D>().points[p] * colliderScale;
            scaledPoints[p] = np;
        }
        GetComponent<PolygonCollider2D>().SetPath(0, scaledPoints);

        randomizeRotateVelocity ();

	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter2D(Collider2D other){

		//if paintball hit player, it bursts
		if (other.GetComponent<Collider2D>().tag == "Player" ) {
			if (explosion != null) {
				//TODO: absorption vfx
		myVFX = Instantiate (explosion, transform.position, transform.rotation) as GameObject;
				myVFX.transform.localScale = new Vector3(getScale(), getScale(), getScale());
                audioz.paintballExplosionSE ();
				myVFX.GetComponent<SpriteRenderer> ().color = color;
				other.GetComponent<Player>().addPaint(color);
				Destroy (gameObject);
			}
		}else if(other.GetComponent<Collider2D>().tag == "Bullet")
        {
			if (!myVFX)
			{
				myVFX = Instantiate
				(explosion, transform.position, transform.rotation) as GameObject;
				myVFX.transform.localScale = new Vector3(getScale(), getScale(), getScale());
			}
		}

	}
		
	public void getsAbsorbed(){
		GameObject vfx = Instantiate 
			(absorption, transform.position, transform.rotation) as GameObject;
		vfx.GetComponent<SpriteRenderer> ().color = color;
		audioz.paintballAbsorptionSE ();
		vfx.transform.localScale = new Vector3 (getScale(), getScale(), getScale());
		if (size - 1 == 0) {
			Destroy (gameObject);
		} else {
			setSize (size - 1);
		}
	}

	public void setColor(Color c){
        needGenCol = false; //since it's already done
		GetComponent<SpriteRenderer> ().color = c;
		color = c;
	}

    public void setColor(float R, float G, float B)
    {
        if(R>1 || G>1 || B > 1) //converting from 255 to 1 color mode
        {
            R /= 255.0f;
            G /= 255.0f;
            B /= 255.0f;
        }
        Color c = new Color(R, G, B);
        setColor(c); //sets needGenCol to false
    }

    public Color getColor(){
		return color;
	}

	public Color genColorWDist(float maxD, Color standard){ 
		//parameter: distance allowed bt generated color and given set color on the 3d RGB cube
		//max distance possible: full color ranges: 441.6729f/255f(from black corner to white corner)
		//mxD =abt sqrt 3
		float R = Random.Range ((standard.r - maxD > 0)? (standard.r - maxD):0, 
			(standard.r + maxD < 1)? (standard.r + maxD):1); //limiting extremes to not go overbound
		float mxD2 = Mathf.Sqrt(Mathf.Pow(maxD, 2)- Mathf.Pow((standard.r - R),2));
		float G = Random.Range ((standard.g - mxD2 > 0)? (standard.g - mxD2):0, 
			(standard.g + mxD2 < 1)? (standard.g + mxD2):1);
		float mxD3 = Mathf.Sqrt(Mathf.Pow(maxD, 2)- 
			Mathf.Pow((standard.r - R),2)- Mathf.Pow((standard.g - G),2));
		float B = Random.Range ((standard.b - mxD3 > 0)? (standard.b - mxD3):0, 
			(standard.b + mxD3 < 1)? (standard.b + mxD3):1);

		Color rdmColor = new Color(R,G,B);
		return rdmColor;
	}


	public void randomizeSpriteKind(){
		num = (int)Random.Range (0, 3.99f);
		GetComponent<SpriteRenderer> ().sprite = pbSprites [num];
		transform.GetChild(0).gameObject.GetComponent<SpriteRenderer> ().sprite 
		= highlights [num];
	}

	public void randomizeRotateVelocity(){
	this.gameObject.GetComponent<Rigidbody2D> ().AddTorque(Random.Range(30f, 50f)); //TODO this is arbitrary
	}
		
	public void setSize(int s){
        needSetSize = false;
       size = s;
		changeScale (new Vector3 
			((size) * sizeScale,
				(size) * sizeScale,(size) * sizeScale));
	}

    /**
     * sizeScale is a basis common factor for all paintballs out there
     * say sizeScale is 2, diff paintballs could be 2*1.5, 2*2, 2*4 big    
     */
    public void setSizeScale(float s)
    {
        sizeScale = s;
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


