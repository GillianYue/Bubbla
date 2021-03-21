using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// animates starting scene UI
/// </summary>
public class TitleController : MonoBehaviour {

    [Inject(InjectFrom.Anywhere)]
    public GameFlow gameFlow;
    [Inject(InjectFrom.Anywhere)]
    public LoadScene loadScene;

    //manually assign
    public Dialogue dialogue;

    public Sprite[] tt0To14;
	//RGB of this background: r68 g135 b152
	public GameObject UIBar1, UIBar2;
    public GameObject spritesParent; //supposed to be the "Background" GO in title scene
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
        StartCoroutine(initializeCoroutine());
    }

    IEnumerator initializeCoroutine()
    {
        //first set up stuff needed for title animation
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        //total sum of heights
        var sh = canvas.GetComponent<RectTransform>().rect.height;
        print("sh: " + sh);

        // set up the first row (exists already in prefab)
        var sr_0 = spritesParent.transform.GetChild(0).GetComponent<SpriteRenderer>();
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

        var singleHeight = sr_0.GetComponent<RectTransform>().rect.height * sr_0.transform.localScale.y;
        sh -= singleHeight;

        numRows = (int)(sh / (singleHeight)) + 1;

        // set up rest of the rows (including duplicating the first row)
        for (int r = 1; r <= numRows; r++)
        {
            GameObject n = Instantiate(spritesParent.transform.GetChild(0).gameObject);
            n.transform.SetParent(spritesParent.transform);
            var sr = spritesParent.transform.GetChild(r).GetComponent<SpriteRenderer>();
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
            sh -= singleHeight;
        }

        loadDone = new bool[1];
        bool[] parseDone = new bool[1];
        yield return StartCoroutine(LoadScene.processCSV(loadDone, DlgCsv, setData, parseDone, false)); //TODO this script is not a loader
        yield return new WaitUntil(() => loadScene.isAllLoadDone());

        yield return StartCoroutine(startTitleScreenAnim());
        yield return StartCoroutine(moveGameFlowPointer());
    }
	
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
		dialogue.displayTitleDLG(data); 

        //UI Bars
		Vector3 v1 = UIBar1.GetComponent<RectTransform> ().localScale;
		Vector3 v2 = UIBar2.GetComponent<RectTransform> ().localScale;
		v1.x = 0f; v2.x = 0f;

        //UI Texts
		for (int i = 0; i < texts.Length; i++) {
			StartCoroutine (showText (texts[i]));
		}
		StartCoroutine (showClockText(clock, clock.GetComponent<Text>()));

        // UI Bar + bg animation; 21 rows in total
		for (int r = numRows; r>=0; r--) {
            //slowly extend the 2 UI bars around dialogue
			UIBar1.GetComponent<RectTransform> ().localScale = v1;
			UIBar2.GetComponent<RectTransform> ().localScale = v2;
			v1.x += 0.008f;
			v2.x += 0.008f;
			StartCoroutine
			(singleRowAnim (spritesParent.transform.GetChild (r).GetComponent<SpriteRenderer> ()));
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

    IEnumerator moveGameFlowPointer()
    {
        yield return new WaitUntil(() => loadScene.isAllLoadDone());


            int tempPT = -1; //temp pointer, is passive, updates as gFlow pointer updates
            int gfP; //game flow pointer
            do
            {//once code gets here, should be ready to start gameFlow
                if (tempPT != (gfP = gameFlow.getCurrentLineNumber()))
                { //avoid redundant work; only rerender if changed
                    tempPT = gfP;
                    gameFlow.processCurrentLine();
                }
                yield return new WaitForSeconds(0.1f); //essentially check dialogue status every one s
            } while (!gameFlow.checkIfEnded()); //as long as there's still something to be done

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
