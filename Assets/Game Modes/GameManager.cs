using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

// THIS GAME MANAGER RUNS SOLELY ON THE SERVER
public class GameManager : NetworkBehaviour {

	public enum MODE { FREE_FOR_ALL, LAST_MAN_STANDING };
	public MainGUI GUI;
	public GameObject[] GameModes;
	public static Dictionary<NetworkInstanceId, Dictionary<string, int>> PlayerStats;
	public static Dictionary<NetworkInstanceId, GameObject> PlayerObjects;

	public static GameMode CurrentGame;
	public static GameManager CurrentGameManager;
	
	void Start () {
		CurrentGameManager = this;
		MODE gamemode = MODE.FREE_FOR_ALL;

		GameObject GM_Object = Instantiate (GameModes[(int) gamemode]) as GameObject;
		CurrentGame = GM_Object.GetComponent<GameMode> ();
		//ClientScene.RegisterPrefab (GM_Object);
		NetworkServer.Spawn (GM_Object);
	}

	// Adds a player to the scoreboard, sets all values to zero by default
	public static void AddNewPlayerInfo (NetworkInstanceId id, GameObject playerObject) {
		if (PlayerStats == null)
			PlayerStats = new Dictionary<NetworkInstanceId, Dictionary<string, int>> ();
		if (PlayerObjects == null)
			PlayerObjects = new Dictionary<NetworkInstanceId, GameObject> ();

		PlayerStats.Add (id, new Dictionary<string, int> ());
		PlayerStats[id].Add ("kills", 0);
		PlayerStats[id].Add ("deaths", 0);

		PlayerObjects.Add (id, playerObject);

		//Debug.Log("kills for player with id #" + id.Value + " = " + players[id]["kills"]);
	}

	// Logs deaths in the scoreboard and kills where applicable
	public static void LogKill (NetworkInstanceId killerId, NetworkInstanceId victimId) {
		PlayerStats[victimId]["deaths"]++;

		// Not a suicide or environmental death, so log a kill
		if (!killerId.IsEmpty () && killerId != victimId) {
			PlayerStats[killerId]["kills"]++;
		}
	}



	// EVERY OBJECT IN THIS ARRAY ***MUST*** HAVE A SCRIPT IMPLEMENTING THE POWERUP INTERFACE
	public GameObject[] PowerUps = new GameObject[2];

	// Returns a powerup based on the percentages that have been set
	// Returns null if no powerup can be spawned
	public GameObject GetPowerUp () {

		int whichPowerUp = Random.Range (0, PowerUps.Length);
		if (PowerUps[whichPowerUp] == null)
			return null;
		return PowerUps[whichPowerUp];
	}
}
