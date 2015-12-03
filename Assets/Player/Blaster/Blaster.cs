using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Blaster : NetworkBehaviour, Weapon {

	public PixelRotation PixRotation;
	public Animator anim;

	// shooting vars
	float timeHeldDown;
	public float TimeToShoot;
	bool triggerDownPreviously = false;
	bool showingCharged = false;

	// bullet vars
	public GameObject bullet;
	public int AmountOfBullets;
	public float SpreadAngle;

	public string GUI () {
		return "" + ((int)(Mathf.Min (100f * guiTimer / TimeToShoot, 100))) + "%";
	}

	public void Reload () { }

	// This section is used to make the charge-up more smooth by simulating the charge amount on
	// the client. It should match up with the charge amount on the server, and prevents us from
	// having to send the value repeatedly just to display on the GUI.
	private bool TimerEnabled = false;
	private float guiTimer;
	[ClientRpc]
	void RpcStartTimer () {
		TimerEnabled = true;
	}
	[ClientRpc]
	void RpcStopTimer () {
		TimerEnabled = false;
		guiTimer = 0f;
	}

	void Update () {
		if (TimerEnabled) {
			guiTimer += Time.deltaTime;
		}
	}

	[Server]
	public float Shoot (bool isTriggerDown) {
		if (triggerDownPreviously && isTriggerDown) { // charging up
			if (timeHeldDown < TimeToShoot)
				timeHeldDown += Time.deltaTime;
			if (timeHeldDown > TimeToShoot && !showingCharged) {
				showingCharged = true;
				RpcChargedAnim ();
				//anim.SetTrigger ("Charged");
			}
			if (!TimerEnabled) {
				RpcStartTimer ();
				TimerEnabled = true;
			}
		} else if (triggerDownPreviously && !isTriggerDown && timeHeldDown >= TimeToShoot) { // shoot
			RpcStopTimer ();
			TimerEnabled = false;
			guiTimer = 0f;

			showingCharged = false;
			triggerDownPreviously = false;
			Vector3 bulletPos = transform.position;
			float angle = PixRotation.Angle * Mathf.Deg2Rad;
			float DistToTip = 20f / 16f;
			bulletPos.x += transform.parent.parent.localScale.x * Mathf.Cos (angle) * DistToTip;
			bulletPos.y += Mathf.Sin (angle) * DistToTip;
			RpcLocalEffects ();
			//LocalEffects ();
			for (float i = 0; i < AmountOfBullets; i++) {
				GameObject nb = (GameObject)Instantiate (bullet, bulletPos, Quaternion.identity);
				NetworkServer.Spawn (nb); // spawn the bullet on the server
				PistolBulletMovement nbScript = nb.GetComponent<PistolBulletMovement> ();
				nbScript.shooter = transform.parent.parent.gameObject;
				nbScript.RpcSetup ((int) (PixRotation.Angle - SpreadAngle / 2 + i / AmountOfBullets * SpreadAngle), transform.parent.parent.localScale.x); // spawn the bullet on all clients
			}
			return 3000f;
		}
		if (!isTriggerDown) {
			timeHeldDown = 0f;
			RpcStopTimer ();
			TimerEnabled = false;
			guiTimer = 0f;
			showingCharged = false;
		}
		triggerDownPreviously = isTriggerDown;
		return 0f;
	}

	public bool CanRotate () {
		return true;
	}

	[ClientRpc]
	void RpcChargedAnim () {
		anim.SetTrigger ("Charged");
	}

	private WeaponAddToPlayer WeaponAdd;
	[ClientRpc]
	void RpcLocalEffects () {
		LocalEffects ();
	}

	void LocalEffects () {
		anim.SetTrigger ("Released");
		if (WeaponAdd == null)
			WeaponAdd = GetComponent<WeaponAddToPlayer> ();
		WeaponAdd.SetUpperArmTrigger ("Pistol_Shoot");
		if (CameraShake.cam.camMove.player == transform.parent.parent)
			CameraShake.cam.Shake (.3f, 5);
	}
}
