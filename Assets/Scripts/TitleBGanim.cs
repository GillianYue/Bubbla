using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleBGanim : MonoBehaviour {

    [Inject(InjectFrom.Anywhere)]
    public GameFlow gameFlow;

    [Inject(InjectFrom.Anywhere)]
    public Dialogue dialogue;

    public Sprite[] tt0To14;
	//RGB of this background: r68 g135 b152
	public GameObject UIBar1, UIBar2;
	public Text[] texts;

    public GameObject clock;
	public float startAnimWait = 0.08f; //speed of how fast it loads
    private GameObject canvas;
    private int numRows;

    private bool[] loadDone;  //for title csv load
    public TextAsset DlgCsv; //dialogue file for a specific level
    private string[,] data; //double array that stores all info of this level

    public ArrayList specialDLGstarts, specialDLGends; //arraylist of ints

    void Start () {

        loadDone = new bool[1];
        bool[] parseDone = new bool[1];
        StartCoroutine(LoadScene.processCSV(loadDone, DlgCsv, setData, parseDone, false));

        canvas = GameObject.FindGameObjectWithTag("Canvas");
        //total sum of heights
        var sh = canvas.GetComponent<RectTransform>().rect.height * 
            canvas.transform.localScale.y;

        // set up the first row (exists already in prefab)
        var sr_0 = transform.GetChild(0).GetComponent<SpriteRenderer>();
        var width = sr_0.sprite.bounds.size.x;
        var screenWidth = canvas.GetComponent<RectTransform>().rect.width;
        // var screenHeight = canvas.GetComponent<RectTransform>().rect.height;
        var tf_0 = sr_0.gameObject.GetComponent<RectTransform>();

        Vector3 scale = new Vector3(1, 1, 1);
        scale.x = screenWidth / width;
        scale.y = scale.x;
        tf_0.localScale = scale;

        Vector3 pos = new Vector3(0, sh, 1);
        tf_0.localPosition = pos;

        sh -= sr_0.bounds.size.y;

        numRows = (int)(sh / sr_0.bounds.size.y)+1;

        // set up rest of the rows (including duplicating the first row)
        for (int r = 1; r <= numRows; r++)
        {
            GameObject n = Instantiate(transform.GetChild(0).gameObject);
            n.transform.SetParent(transform);
            var sr = transform.GetChild(r).GetComponent<SpriteRenderer>();
            width = sr.sprite.bounds.size.x;
            screenWidth = canvas.GetComponent<RectTransform>().rect.width;
            // var screenHeight = canvas.GetComponent<RectTransform>().rect.height;
            var tf = sr.gameObject.transform;

           scale = new Vector3(1, 1, 1);
            scale.x = screenWidth / width;
            scale.y = scale.x;
            tf.localScale = scale;

            pos = new Vector3(0, sh, 1);
            tf.localPosition = pos;
            sh -= sr.bounds.size.y;
        }
        StartCoroutine (startTitleScreenAnim ());
        }
	
	// Update is called once per frame
	void Update () {
	
	}

    public bool checkTitleLoadDone()
    { //check if title loaders are ready for game
        return (loadDone[0]);
    }

    // set data for title screen
    public void setData(string[,] d, bool[] parseDone)
    {
        data = d;
        StartCoroutine(parseDLGcsvData(parseDone));
    }

    IEnumerator parseDLGcsvData(bool[] parseDone)
    {
        yield return new WaitUntil(() => (data != null));
        int nRows = data.GetLength(1); bool toggle = false;
        for (int r = 1; r < nRows; r++) //-1 because title row doesn't count
        {
            if (data[0, r].Equals("SPECIAL"))
            {
                if (!toggle)
                {
                    specialDLGstarts.Add(r + 1); //line after starting "SPECIAL"
                }
                else
                {
                    specialDLGends.Add(r - 1);
                }
                toggle = !toggle;
            }
        }

        parseDone[0] = true;
    }

    IEnumerator startTitleScreenAnim(){
      
		while (!checkTitleLoadDone()) {//wait till csv's loaded
            yield return null;
		}
        //gameFlow dlg has access to the csv containing scripts of dialogue
		StartCoroutine (dialogue.displayTitleDLG(data)); 

		Vector3 v1 = UIBar1.GetComponent<RectTransform> ().localScale;
		Vector3 v2 = UIBar2.GetComponent<RectTransform> ().localScale;
		v1.x = 0f; v2.x = 0f;

		for (int i = 0; i < texts.Length; i++) {
			StartCoroutine (showText (texts[i]));
		}
		StartCoroutine (showClockText(clock, clock.GetComponent<Text>()));

        // There is 21 rows in total
		for (int r = numRows; r>=0; r--) {
            //slowly extend the 2 UI bars around dialogue
			UIBar1.GetComponent<RectTransform> ().localScale = v1;
			UIBar2.GetComponent<RectTransform> ().localScale = v2;
			v1.x += 0.008f;
			v2.x += 0.008f;
			StartCoroutine
			(singleRowAnim (transform.GetChild (r).GetComponent<SpriteRenderer> ()));
			yield return new WaitForSeconds (startAnimWait);
		}
		for (int r = 10; r>=0; r--) {
            //continue doing so even after background is loaded to create discontinuity
			UIBar1.GetComponent<RectTransform> ().localScale = v1;
			UIBar2.GetComponent<RectTransform> ().localScale = v2;
			v1.x += 0.006f;
			v2.x += 0.006f;
			yield return new WaitForSeconds (startAnimWait);
		}


	}

	private IEnumerator singleRowAnim(SpriteRenderer sr){

        for (int n = 0; n < 15; n++) {
			sr.sprite = tt0To14[n];
            yield return new WaitForSeconds (startAnimWait);
		}
	}

	private IEnumerator showText(Text t){
		string store = t.text;
		t.text = "";
		for (int n = 0; n < store.Length; n++) {
			t.text += store [n];
			yield return new WaitForSeconds (startAnimWait+0.05f);
		}
	}

	private IEnumerator showClockText(GameObject clk, Text clkT){
		string store = clkT.text;
		clkT.text = "";
		for (int n = 0; n < store.Length; n++) {
			clkT.text += store [n];
			yield return new WaitForSeconds (startAnimWait+0.05f);
		}
		clk.GetComponent<TimeUpdate> ().updt = true;
	}
}
