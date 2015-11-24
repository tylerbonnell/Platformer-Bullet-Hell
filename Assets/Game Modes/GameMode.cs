using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public abstract class GameMode : NetworkBehaviour {
	public MainGUI GUI;

	public bool TimedMatch;
	public abstract void Kill (GameObject killedPlayed);
}
