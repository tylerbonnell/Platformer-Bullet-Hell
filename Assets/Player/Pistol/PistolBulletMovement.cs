using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PistolBulletMovement : NetworkBehaviour {

	public GameObject shooter;
	public PixelRotation PixRotation;
	public SpriteRenderer SpriteRender;
	public GameObject NormalExplosion;
	public GameObject BiggerExplosion;
	public float speed;
	private float TimeElapsed = 0f;
	public int DamageAmount;

	// Update is called once per frame
	void Update () {
		if (SpriteRender.enabled) {
			transform.Translate (Mathf.Cos (PixRotation.Angle * Mathf.PI / 180) * speed * transform.localScale.x * Time.deltaTime,
							Mathf.Sin (PixRotation.Angle * Mathf.PI / 180) * speed * Time.deltaTime, 0);
			TimeElapsed += Time.deltaTime;

			if (TimeElapsed > 4f)
				Destroy (gameObject);
		}
	}

	// Called when the bullet is created. Ensures that the sprite is angled correctly
	[ClientRpc]
	public void RpcSetup (int angle, float direction) {
		PixRotation.Angle = angle;
		transform.localScale = new Vector3 (direction, 1, 1);
		SpriteRender.enabled = true;
	}

	// Spawns an explosion at the site of impact
	[ClientRpc]
	void RpcSpawnExplosion () {
		GameObject explosion;
		if (Random.Range (1, 4) == 1)
			explosion = Instantiate (BiggerExplosion, transform.position, Quaternion.identity) as GameObject;
		else
			explosion = Instantiate (NormalExplosion, transform.position, Quaternion.identity) as GameObject;
		if (isServer)
			explosion.GetComponent<ExplosionBehavior> ().shooter = shooter;
		Destroy (gameObject);
	}

	// Damages a player if it hits, creates a small explosion when hitting a wall
	void OnTriggerEnter2D (Collider2D other) {
		if (!isServer)
			return;

		Damageable dmg = other.gameObject.GetComponent<Damageable> ();
		if (dmg != null && other.gameObject != shooter) {
			Vector3 DmgAngle = new Vector3 (other.transform.position.x - shooter.transform.position.x, other.transform.position.y - shooter.transform.position.y, 0).normalized;
			dmg.Damage (DamageAmount, shooter, DmgAngle * 1000);
			if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
				RpcSpawnExplosion ();
			} else {
				Destroy (gameObject);
			}
		} else if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			RpcSpawnExplosion ();
		}
	}
}
