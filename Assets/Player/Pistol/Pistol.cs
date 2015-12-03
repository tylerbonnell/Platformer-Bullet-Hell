using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Pistol : NetworkBehaviour, Weapon {

	public Animator anim;
	public PixelRotation PixRotation;
	public GameObject ShellPrefab;
	public GameObject Bullet;
	private float TimeElapsed;
	private float TimeToShoot = .25f;
	private float TimeToShootOriginal = .25f;
	private float TimeToShootReloading = 1.5f;
	public int MagazineCapacity = 12;
	public int MaxAmmo;
	[SyncVar]
	public int Ammo;
	[SyncVar]
	private int Magazine = 12;

	public void Update () {
		TimeElapsed += Time.deltaTime;
	}

	public string GUI () {
		return "" + Magazine + "/" + Ammo;
	}

	public void Reload () {
		if (Magazine == MagazineCapacity || Ammo == 0)
			return;

		TimeElapsed = 0;
		TimeToShoot = TimeToShootReloading;
	}

	[Server]
	public float Shoot (bool isTriggerDown) {
		if (TimeToShoot == TimeToShootReloading && TimeElapsed > TimeToShoot) { // reloading
			int currentMagSize = Magazine;
			Magazine = Mathf.Min (Magazine + Ammo, MagazineCapacity);
			Ammo = Mathf.Max (0, Ammo - currentMagSize);
			TimeToShoot = TimeToShootOriginal;
		}
		if (isTriggerDown && TimeElapsed > TimeToShoot && Ammo > 0) { // shooting
			Magazine--;
			if (Magazine == 0) { // You are reloading this time
				TimeToShoot = TimeToShootReloading;
			}
			TimeElapsed = 0f;

			// Instantiate bullet
			Vector3 bulletPos = transform.position;
			float angle = PixRotation.Angle * Mathf.PI / 180 + Mathf.Atan (2f / 16f);
			float DistToTip = Mathf.Sqrt (300) / 16f;
			bulletPos.x += transform.parent.parent.localScale.x * Mathf.Cos (angle) * DistToTip;
			bulletPos.y += Mathf.Sin (angle) * DistToTip;
			GameObject nb = (GameObject)Instantiate (Bullet, bulletPos, Quaternion.identity);

			// Instantiate casing
			Vector3 casingPos = transform.position;
			casingPos.x += transform.parent.parent.localScale.x * Mathf.Cos (angle) * DistToTip * .75f;
			casingPos.y += Mathf.Sin (angle) * DistToTip * .75f;

			// Casing velocity
			float casingAngleRandom = 115f * Mathf.PI / 180 * Random.Range (.9f, 1.1f);
			float casingVelocity = 650f;
			float dir = transform.parent.parent.localScale.x;
			Vector2 force = (Vector3.up * Mathf.Sin (casingAngleRandom + angle) + Vector3.right * Mathf.Cos (casingAngleRandom + angle) * dir) * casingVelocity;

			RpcLocalEffects (casingPos, force);
			//LocalEffects (casingPos, force);
			NetworkServer.Spawn (nb); // spawn the bullet on the server
			PistolBulletMovement nbScript = nb.GetComponent<PistolBulletMovement> ();
			nbScript.shooter = transform.parent.parent.gameObject;
			nbScript.RpcSetup (PixRotation.Angle, transform.parent.parent.localScale.x); // spawn the bullet on all clients

			return 1500f;
		}
		return 0f;
	}

	public bool CanRotate () {
		return true;
	}

	// Spawns a casing, shakes the screen, plays the gun animation
	[ClientRpc]
	void RpcLocalEffects (Vector3 casingPos, Vector2 force) {
		LocalEffects (casingPos, force);
	}

	private WeaponAddToPlayer WeaponAdd;
	void LocalEffects (Vector3 casingPos, Vector2 force) {
		GameObject nc = (GameObject)Instantiate (ShellPrefab, casingPos, Quaternion.identity);
		nc.GetComponent<Rigidbody2D> ().AddForce (force);
		if (WeaponAdd == null)
			WeaponAdd = GetComponent<WeaponAddToPlayer> ();
		WeaponAdd.SetUpperArmTrigger ("Pistol_Shoot");
		anim.SetTrigger ("shoot");
		if (CameraShake.cam.camMove.player == transform.parent.parent)
			CameraShake.cam.Shake (.12f, 5);
	}
}
