using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour {

	public Transform player;
	private PlayerControls pc;
	public bool TrackPlayer = true;
	public float CamDistance = 20f;
	public MainGUI GUI;
	private float lerpSpeed = .03f;
	private Vector3 lastPlayerPosition = new Vector3 (0, 5, -100);

	// Update is called once per frame
	public Vector3 CalculatePosition (Vector3 camPos) {
		if (player == null || !TrackPlayer)
			return new Vector3 (0, 5, -100);

		if (pc == null)
			pc = player.GetComponent<PlayerControls> ();

		if (pc.Alive)
			lastPlayerPosition = player.position;

		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		Vector3 size = Camera.main.ScreenToWorldPoint (new Vector3 (Camera.main.pixelWidth, Camera.main.pixelHeight, 0))
			- Camera.main.ScreenToWorldPoint (new Vector3 (0, 0, 0));
		float mousePercentageFromCenterX = Mathf.Abs (mousePos.x - transform.position.x) / size.x;
		float mousePercentageFromCenterY = Mathf.Abs (mousePos.y - transform.position.y) / size.y;

		/* Center camera between player and mouse*/
		Vector3 midPos = (mousePos - lastPlayerPosition).normalized;
		midPos.x *= mousePercentageFromCenterX * CamDistance;
		midPos.y *= mousePercentageFromCenterY * CamDistance;
		midPos += lastPlayerPosition;
		midPos.z = transform.position.z;

		return Vector3.Lerp (camPos, midPos, lerpSpeed);

		/* Center between player and mouse, not relative to mouse distance from player
		Vector3 midPos = (mousePos - player.position).normalized * 6 + player.position;
		midPos.z = transform.position.z;
		transform.position = Vector3.Lerp(transform.position, midPos, .02f); */

		/* Center to the left or right depending on direction 
		float dir = 1;
		if (player.localScale.x < 1)
			dir = -1;
		Vector3 camPos = player.position;
		camPos.z = transform.position.z;
		camPos.x = camPos.x + (dir * .15f * width);
		transform.position = Vector3.Lerp (transform.position, camPos, .03f);*/
	}
}
