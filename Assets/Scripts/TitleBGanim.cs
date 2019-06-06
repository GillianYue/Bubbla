using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleBGanim : MonoBehaviour {

	public Sprite[] tt0To14;
	//RGB of this background: r68 g135 b152
	public GameObject UIBar1, UIBar2;
	public Text[] texts;
	public GameFlow dlg;
	public GameObject clock;
	public float startAnimWait = 0.08f; //speed of how fast it loads

	void Start () {
		StartCoroutine (startTitleScreenAnim ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator startTitleScreenAnim(){
		while (!dlg.checkLoadDone()) {//wait till csv's loaded
			yield return null;
		}
		StartCoroutine (dlg.displayTitleDLG());

		Vector3 v1 = UIBar1.GetComponent<RectTransform> ().localScale;
		Vector3 v2 = UIBar2.GetComponent<RectTransform> ().localScale;
		v1.x = 0f; v2.x = 0f;

		for (int i = 0; i < texts.Length; i++) {
			StartCoroutine (showText (texts[i]));
		}
		StartCoroutine (showClockText(clock, clock.GetComponent<Text>()));

		for (int r = 21; r>=0; r--) {
			UIBar1.GetComponent<RectTransform> ().localScale = v1;
			UIBar2.GetComponent<RectTransform> ().localScale = v2;
			v1.x += 0.008f;
			v2.x += 0.008f;
			StartCoroutine
			(singleRowAnim (transform.GetChild (r).GetComponent<SpriteRenderer> ()));
			yield return new WaitForSeconds (startAnimWait);
		}
		for (int r = 12; r>=0; r--) {
			UIBar1.GetComponent<RectTransform> ().localScale = v1;
			UIBar2.GetComponent<RectTransform> ().localScale = v2;
			v1.x += 0.008f;
			v2.x += 0.008f;
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
