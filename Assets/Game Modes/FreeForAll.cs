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
		RespawnQueue = new Queue<PlayerControls> ();
	}

	void Update () {
		if (TimedMatch)
			GUI.Timer.DisplayTime (Seconds);
		if (isServer && Seconds > 0) {
			Seconds_ -= Time.deltaTime;
			Seconds = (int) Seconds_;
		}
	}

	// Called on server
	public override void Kill (PlayerControls killedPlayer) {
		killedPlayer.AliveAndVisible (false);
		RespawnQueue.Enqueue (killedPlayer);
		Invoke ("RespawnNext", 10f);
	}

	private Queue<PlayerControls> RespawnQueue;
	private void RespawnNext () {
		PlayerControls RevivedPlayer = RespawnQueue.Dequeue ();
		RevivedPlayer.AliveAndVisible (true);
	}
}