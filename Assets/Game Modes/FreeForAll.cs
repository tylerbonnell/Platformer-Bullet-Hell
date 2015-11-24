using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class FreeForAll : GameMode {

	// This is running on all clients at once

	[SyncVar]
	public int Seconds;
	private float Seconds_;

	void Start () {
		GUI = GameObject.FindGameObjectWithTag ("GUI").GetComponent<MainGUI> ();
		Seconds_ = Seconds;
		RespawnStack = new Stack<GameObject> ();
	}

	void Update () {
		if (TimedMatch)
			GUI.Timer.DisplayTime (Seconds);
		if (isServer && Seconds > 0) {
			Seconds_ -= Time.deltaTime;
			Seconds = (int) Seconds_;
		}
	}

	public override void Kill (GameObject killedPlayer) {
		killedPlayer.SetActive (false);
		RespawnStack.Push (killedPlayer);
		Invoke ("RespawnNext", 5f);
	}

	private Stack<GameObject> RespawnStack;
	private void RespawnNext () {
		GameObject RevivedPlayer = RespawnStack.Pop ();
		RevivedPlayer.SetActive (true);
	}
}