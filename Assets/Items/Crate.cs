using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Crate : NetworkBehaviour, Damageable {

	// Use this for initialization
	void Start () {
		transform.eulerAngles = new Vector3 (transform.eulerAngles.x, transform.eulerAngles.y, Random.Range (0, 3) * 90);
	}

	// Only runs on the server
	public void Damage (int DamageAmount, GameObject shooter, Vector3 force = default(Vector3)) {
		if (!isServer)
			return;
		SpawnItem ();
		Destroy (gameObject);
	}

	// Only runs on the server
	void SpawnItem () {
		GameObject Item = GameManager.CurrentGameManager.GetPowerUp ();
		if (Item == null)
			return;
		GameObject go = (GameObject) Instantiate (Item, transform.position, Quaternion.identity);
		NetworkServer.Spawn (go);
	}
}