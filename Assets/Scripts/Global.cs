using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{
	public static int Scene_To_Load;

	public static bool touching(Vector2 touch, Vector2 itemPos,
		double itemwidth, double itemheight){
		if (touch.x > (itemPos.x) &&
		    touch.x < (itemPos.x + itemwidth)
			&& touch.y > (itemPos.y)
			&& touch.y < (itemPos.y + itemheight)) {
			//NOTE: the y here is only the relative y for the 
			//Vector2s in the parameter. Pass z when calling function
			return true;
		} else {
			return false;
		}
	}

	public static bool touching(Vector2 touch, Vector2 itemPos,
		double radius){
		if (touch.x > (itemPos.x - radius / 2) &&
			touch.x < (itemPos.x + radius / 2)
			&& touch.y > (itemPos.y - radius / 2)
			&& touch.y < (itemPos.y + radius / 2)) {
			return true;
		} else {
			return false;
		}
	}

	public static IEnumerator LoadAsyncScene(int sceneNO)
	{
		// The Application loads the Scene in the background as the current Scene runs.
		// This is particularly good for creating loading screens.
		// You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
		// a sceneBuildIndex of 1 as shown in Build Settings.


		AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNO);

		// Wait until the asynchronous scene fully loads
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
	}
		

}

