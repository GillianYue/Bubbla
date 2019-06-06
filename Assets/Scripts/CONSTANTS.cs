using UnityEngine;
using System.Collections;

public class CONSTANTS : MonoBehaviour {

	private static float orthographicSize = 6.67f; //entered manually by ME

	private static float worldY = orthographicSize * 2;
	private static float worldX = worldY * Screen.width / Screen.height;
	
	public static Vector2 WorldUnitsInCamera = new Vector2(worldX, worldY);
	public static Vector2 WorldToPixelFactor = new Vector2(Screen.width/worldX, 
		Screen.height/worldY);
	public static Vector2 PixelToWorldFactor = new Vector2(worldX/Screen.width, 
		worldY/Screen.height);



}
