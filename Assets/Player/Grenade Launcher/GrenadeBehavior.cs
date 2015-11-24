using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GrenadeBehavior : NetworkBehaviour {

	public GameObject shooter;
	public Rigidbody2D rb;
	public GrenadeLauncher Launcher;
	public GameObject NormalExplosion;
	public GameObject BiggerExplosion;

	public void Detonate () {
		RpcSpawnExplosion (transform.position);
		Launcher.GrenadeOut = null;
		//Destroy(gameObject);
	}

	[ClientRpc]
	void RpcSpawnExplosion (Vector3 pos) {
		GameObject expl;
		if (Random.Range (1, 4) == 1)
			expl = Instantiate (BiggerExplosion, pos, Quaternion.identity) as GameObject;
		else
			expl = Instantiate (NormalExplosion, pos, Quaternion.identity) as GameObject;
		if (isServer)
			expl.GetComponent<ExplosionBehavior> ().shooter = shooter;
		Destroy (gameObject);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (isServer && other.gameObject != shooter)
			Detonate ();
	}
}
