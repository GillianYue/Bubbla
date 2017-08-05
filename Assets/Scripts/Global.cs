using UnityEngine;
using System.Collections;

public class Global : MonoBehaviour
{

	//needTest
	public static bool touching(Vector2 touch, Vector2 itemPos,
		double itemwidth, double itemheight){
		if (touch.x > (itemPos.x - itemwidth / 2) &&
		    touch.x < (itemPos.x + itemwidth / 2)
			&& touch.y > (itemPos.y - itemheight / 2)
			&& touch.y < (itemPos.y + itemheight / 2)) {
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
}

