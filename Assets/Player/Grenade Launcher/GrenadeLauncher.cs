using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GrenadeLauncher : NetworkBehaviour, Weapon {

	public GameObject GrenadePrefab;
	public PixelRotation PixRotation;
	public Animator anim;

	private bool TriggerHasBeenReleased = true;
	public GameObject GrenadeOut;
	private GrenadeBehavior GrenadeOutBehavior;
	[SyncVar]
	public int MaxAmmo = 20;
	[SyncVar]
	private int ammo;

	public string GUI () {
		return "" + ammo;
	}

	void Start () {
		ammo = MaxAmmo;
	}

	public void Reload () { }

	[Server]
	public float Shoot (bool isTriggerDown) {
		if (ammo == 0) {
			return 0f;
		}

		if (isTriggerDown && TriggerHasBeenReleased) {
			TriggerHasBeenReleased = false;
			if (GrenadeOut == null) {
				// There is no grenade out yet
				ammo--;
				Vector3 bulletPos = transform.position;
				float angle = PixRotation.Angle * Mathf.PI / 180 + Mathf.Atan (2f / 18f);
				float DistToTip = .35f;
				bulletPos.x += transform.parent.parent.localScale.x * Mathf.Cos (angle) * DistToTip;
				bulletPos.y += Mathf.Sin (angle) * DistToTip;
				GrenadeOut = Instantiate (GrenadePrefab, bulletPos, Quaternion.identity) as GameObject;
				
				// grenade velocity
				float GrenadeVelocity = 450f;
				float dir = transform.parent.parent.localScale.x;
				Vector2 force = (Vector3.up * Mathf.Sin (angle) + Vector3.right * Mathf.Cos (angle) * dir) * GrenadeVelocity;

				GrenadeOutBehavior = GrenadeOut.GetComponent<GrenadeBehavior> ();
				GrenadeOutBehavior.shooter = transform.parent.parent.gameObject;
				GrenadeOutBehavior.rb.AddForce (force);
				GrenadeOutBehavior.Launcher = this;
				RpcAnimation ();
				//anim.SetTrigger ("Shoot");
				if (CameraShake.cam.camMove.player == transform.parent)
					CameraShake.cam.Shake (.1f, 5);
				NetworkServer.Spawn (GrenadeOut); // spawn the bullet on the server
				return 2000f;
			} else {
				// There is a grenade out, click will detonate it
				GrenadeOutBehavior.Detonate ();
			}
		} else if (!isTriggerDown) {
			TriggerHasBeenReleased = true;
		}
		return 0f;
	}

	private WeaponAddToPlayer WeaponAdd;
	[ClientRpc]
	void RpcAnimation () {
		anim.SetTrigger("Shoot");
		if (WeaponAdd == null)
			WeaponAdd = GetComponent<WeaponAddToPlayer> ();
		WeaponAdd.SetUpperArmTrigger ("Pistol_Shoot");
		if (CameraShake.cam.camMove.player == transform.parent.parent)
			CameraShake.cam.Shake (.08f, 5);
	}

	public bool CanRotate () {
		return true;
	}
}
