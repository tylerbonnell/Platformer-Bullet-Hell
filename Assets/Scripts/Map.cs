using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

	public static Map singleton;
	public Transform[] SpawnLocations;

	// Use this for initialization
	void Start () {
		if (singleton == null) {
			singleton = this;
		} else {
			Destroy (this);
		}
	}

	public Transform getRandomLocation () {
		return SpawnLocations [Random.Range (0, SpawnLocations.Length)];
	}
	
	// Update is called once per frame
	//void Update () {}
}
