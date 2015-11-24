using UnityEngine;
using System.Collections;

public class DrawLogo : MonoBehaviour {

	private int xIndex;
	private int yIndex;
	private string LogoStr = "2211111111111111111111122-2210000110100000100000122-2210111011101110101111122-2210111010100000100000122-2210111010101101101111122-2210000110101100100000122-1111111111111111111111111-1000011000001000001000001-1011011011111011101011101-1000001000001000001000001-1011101011111011101011011-1000001000001011101011001-1111111111111111111111111";
	private char[] PixelChars;
	private int charIndex;
	public GameObject pixel;
	private GameObject[,] Pixels = new GameObject[25,13];
	public Rigidbody2D rocket;

	// Use this for initialization
	void Start () {
		PixelChars = LogoStr.ToCharArray ();

		for (int i = 0; i < PixelChars.Length; i++) {
			if (PixelChars[i] == '2') {
				xIndex++;
			} else if (PixelChars[i] == '-') {
				yIndex++;
				xIndex = 0;
			} else { //if (PixelChars[i] == '1' || PixelChars[i] == '0') {
				GameObject go = Instantiate (pixel, new Vector3 (xIndex, -yIndex, 0), Quaternion.identity) as GameObject;
				Pixels[xIndex, yIndex] = go;
				xIndex++;
				if (PixelChars[i] == '1') { // border pixel
					go.GetComponent<SpriteRenderer> ().color = new Color (.122f, .122f, .122f);
					//go.GetComponent<SpriteRenderer> ().color = Color.clear;
				} else { // inner pixel
					//go.GetComponent<SpriteRenderer> ().color = Color.white;
				}
			}
		}

		rocket.AddForce (new Vector2 (0, 500000));
	}
	
	// Update is called once per frame
	void Update () {
	//	char c = pixels[charIndex];
	}
}
