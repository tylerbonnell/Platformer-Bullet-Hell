﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class PlayerControls : NetworkBehaviour, Damageable {

	private bool facingRight = true;
	private Animator LegAnimator;
	private Rigidbody2D rb;

	/*public Color HeadMainColor;
	public Color HeadSecondaryColor;
	public Color BodyMainColor;
	public Color BodySecondaryColor;*/

	public int MaxHealth;
	[SyncVar]
	private int Health;
	private float BaseGravity;

	public Transform LeftArmPrefab;
	public Transform LeftArm;
	public Transform LeftArmLocation;
	private PixelRotation LeftArmRotation;
	private SpriteRenderer LeftArmSprite;
	public Transform LeftUpperArm;
	private PixelRotation LeftUpperArmRotation;
	private SpriteRenderer LeftUpperArmSprite;
	private Animator LeftUpperArmAnim;
	private Weapon LeftArmWeapon;
	private bool LeftArmCanRotate;

	public Transform RightArmPrefab;
	public Transform RightArm;
	public Transform RightArmLocation;
	private PixelRotation RightArmRotation;
	private SpriteRenderer RightArmSprite;
	public Transform RightUpperArm;
	private PixelRotation RightUpperArmRotation;
	private SpriteRenderer RightUpperArmSprite;
	private Animator RightUpperArmAnim;
	private Weapon RightArmWeapon;
	private bool RightArmCanRotate;

	public SpriteRenderer BodySprite;
	public SpriteRenderer HeadSprite;
	public SpriteRenderer LegSprite;
	public GameObject Legs;
	public float WalkSpeed;
	public float JumpPower;

	// Powerups
	public GameObject JetPack;
	public bool JetPackEnabled;
	public Animator JetPackAnimator;
	public GameObject ForceField;

	// Other Stuff
	public ParticleSystem RunningParticles;

	// Use this for initialization
	void Start () {
		Health = MaxHealth;
		LegAnimator = Legs.GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
		if (isLocalPlayer) {
			CameraMovement CM = Camera.main.GetComponent<CameraMovement> ();
			CM.player = transform;
			GUI = CM.GUI;
		}
		BaseGravity = rb.gravityScale;
	}

	public override void OnStartLocalPlayer () {
		//Debug.Log("local player started");
		CmdInstantiateArms ();
	}

	[Server]
	public override void OnStartServer () {
		GameManager.AddNewPlayerInfo (netId, gameObject);
	}

	// Update is called once per frame
	void Update () {
		if (LeftArmWeapon == null || RightArmWeapon == null)
			return;

		if (isLocalPlayer) {
			UpdateGUI ();
			if (LeftArmCanRotate)
				GunAngle (LeftArm, LeftArmRotation);
			if (RightArmCanRotate)
				GunAngle (RightArm, RightArmRotation);
			GetTriggersDown ();
			PowerUpFunction ();
		}
		if (isServer) {
			ShootRight (IsRightTriggerDown);
			ShootLeft (IsLeftTriggerDown);
		}
		Animation ();
	}

	void FixedUpdate () {
		if (isLocalPlayer)
			Movement ();
	}

	// Spawn arms, set up all the arm-related variables
	[Command]
	void CmdInstantiateArms () {
		// Left arm components
		LeftArm = Instantiate (LeftArmPrefab, LeftArmLocation.position, Quaternion.identity) as Transform;
		WeaponAddToPlayer ScriptL = LeftArm.GetComponent<WeaponAddToPlayer> ();
		ScriptL.PlayerNetId = netId;
		ScriptL.isRightArm = false;
		NetworkServer.Spawn (LeftArm.gameObject);

		// Right arm components
		RightArm = Instantiate (RightArmPrefab, RightArmLocation.position, Quaternion.identity) as Transform;
		WeaponAddToPlayer ScriptR = RightArm.GetComponent<WeaponAddToPlayer> ();
		ScriptR.PlayerNetId = netId;
		ScriptR.isRightArm = true;
		NetworkServer.Spawn (RightArm.gameObject);
	}
	public void ConfigureRightArm (Transform newRightArm) {
		RightArm = newRightArm;
		RightArm.parent = BodySprite.transform;
		RightArmSprite = RightArm.GetComponent<SpriteRenderer> ();
		RightArmRotation = RightArm.GetComponent<PixelRotation> ();
		RightArmWeapon = RightArm.GetComponent<Weapon> ();
		RightArmCanRotate = RightArmWeapon.CanRotate ();
		RightUpperArmRotation = RightUpperArm.GetComponent<PixelRotation> ();
		RightUpperArmSprite = RightUpperArm.GetComponent<SpriteRenderer> ();
		RightUpperArmAnim = RightUpperArm.GetComponent<Animator> ();
	}
	void PositionRightArm () {
		if (facingRight) {
			RightArm.position = RightArmLocation.position;
			RightArmSprite.sortingOrder = 4;
			RightUpperArm.position = RightArmLocation.position;
			RightUpperArmSprite.sortingOrder = 3;
		} else {
			RightArm.position = LeftArmLocation.position;
			RightArmSprite.sortingOrder = -2;
			RightUpperArm.position = LeftArmLocation.position;
			RightUpperArmSprite.sortingOrder = -3;
		}
	}
	public void ConfigureLeftArm (Transform newLeftArm) {
		LeftArm = newLeftArm;
		LeftArm.parent = BodySprite.transform;
		LeftArmSprite = LeftArm.GetComponent<SpriteRenderer> ();
		LeftArmRotation = LeftArm.GetComponent<PixelRotation> ();
		LeftArmWeapon = LeftArm.GetComponent<Weapon> ();
		LeftArmCanRotate = LeftArmWeapon.CanRotate ();
		LeftUpperArmRotation = LeftUpperArm.GetComponent<PixelRotation> ();
		LeftUpperArmSprite = LeftUpperArm.GetComponent<SpriteRenderer> ();
		LeftUpperArmAnim = LeftUpperArm.GetComponent<Animator> ();
	}
	void PositionLeftArm () {
		if (facingRight) {
			LeftArm.position = LeftArmLocation.position;
			LeftArmSprite.sortingOrder = -2;
			LeftUpperArm.position = LeftArmLocation.position;
			LeftUpperArmSprite.sortingOrder = -3;
		} else {
			LeftArm.position = RightArmLocation.position;
			LeftArmSprite.sortingOrder = 4;
			LeftUpperArm.position = RightArmLocation.position;
			LeftUpperArmSprite.sortingOrder = 3;
		}
	}

	// Calls animation triggers on the upper arms so they match up with the weapon
	public void SetUpperArmTrigger (string trigger, bool isRightArm) {
		Animator a = LeftUpperArmAnim;
		if (isRightArm) a = RightUpperArmAnim;
		a.SetTrigger (trigger);
	}

	// Checks which triggers are down on the local player
	void GetTriggersDown () {
		bool newRightTrigger = Input.GetButton ("Fire1");
		if (newRightTrigger != IsRightTriggerDown)
			CmdSetRightTrigger (newRightTrigger);
		IsRightTriggerDown = newRightTrigger;

		bool newLeftTrigger = Input.GetButton ("Fire2");
		if (newLeftTrigger != IsLeftTriggerDown)
			CmdSetLeftTrigger (newLeftTrigger);
		IsLeftTriggerDown = newLeftTrigger;

		if (Input.GetKeyDown (KeyCode.R)) {
			CmdReload ();
		}
	}
	[Command]
	void CmdSetRightTrigger (bool newTrigger) {
		IsRightTriggerDown = newTrigger;
	}
	[Command]
	void CmdSetLeftTrigger (bool newTrigger) {
		IsLeftTriggerDown = newTrigger;
	}
	[Command]
	void CmdReload () {
		RightArmWeapon.Reload ();
		LeftArmWeapon.Reload ();
	}

	// Fire the equipped weapons
	bool IsRightTriggerDown;
	void ShootRight (bool isFiring) {
		float kb = RightArmWeapon.Shoot (isFiring);
		// knockback
		if (kb != 0) {
			int direction = 1;
			if (facingRight) direction = -1;
			Vector3 kbforce = new Vector3 (direction * kb * Mathf.Cos ((RightArmRotation.Angle) * Mathf.PI / 180),
										-kb * Mathf.Sin ((RightArmRotation.Angle) * Mathf.PI / 180), 0);
			if (isLocalPlayer)
				rb.AddForce (kbforce);
			else
				RpcAddForce (kbforce);
		}
	}
	bool IsLeftTriggerDown;
	void ShootLeft (bool isFiring) {
		float kb = LeftArmWeapon.Shoot (isFiring);
		// knockback
		if (kb != 0) {
			int direction = 1;
			if (facingRight) direction = -1;
			Vector3 kbforce = new Vector3 (direction * kb * Mathf.Cos ((LeftArmRotation.Angle) * Mathf.PI / 180),
										-kb * Mathf.Sin ((LeftArmRotation.Angle) * Mathf.PI / 180), 0);
			if (isLocalPlayer)
				rb.AddForce (kbforce);
			else
				RpcAddForce (kbforce);
		}
	}

	// Calculate the angle of the gun towards the mouse
	// This is only called on the local player
	void GunAngle (Transform arm, PixelRotation armRotation) {
		Vector3 screenPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		bool shouldFlip = (facingRight && screenPos.x < transform.position.x ||
							!facingRight && screenPos.x > transform.position.x);
		if (shouldFlip)
			Flip (!facingRight);

		Vector3 newAngles;
		if (facingRight) {
			// Rotate the parts
			newAngles = new Vector3 (0, 0, Mathf.Atan2 ((screenPos.y - arm.position.y), (screenPos.x - arm.position.x)) * Mathf.Rad2Deg);
		} else {
			// Rotate the parts
			newAngles = new Vector3 (0, 0, -Mathf.Atan2 (-(screenPos.y - arm.position.y), -(screenPos.x - arm.position.x)) * Mathf.Rad2Deg);
		}
		armRotation.Angle = (int) newAngles.z;

		// This sets the upper arm rotation the same as the weapon rotation
		PixelRotation upperArmRotation = LeftUpperArmRotation;
		if (armRotation == RightArmRotation)
			upperArmRotation = RightUpperArmRotation;
		upperArmRotation.Angle = armRotation.Angle;

		CmdSetAngle (armRotation.Angle, arm == RightArm);
		if (shouldFlip)
			CmdFlip (facingRight);
	}
	[Command]
	void CmdSetAngle (int angle, bool isRightArm) {
		RpcSetAngle (angle, isRightArm);
	}
	[ClientRpc]
	void RpcSetAngle (int angle, bool isRightArm) {
		if (!isLocalPlayer) {
			PixelRotation pr = LeftArmRotation;
			if (isRightArm)
				pr = RightArmRotation;
			if (pr != null)
				pr.Angle = angle;
		}
	}
	[Command]
	void CmdFlip (bool newFacingRight) {
		RpcFlip (newFacingRight);
	}
	[ClientRpc]
	void RpcFlip (bool newFacingRight) {
		if (!isLocalPlayer)
			Flip (newFacingRight);
	}

	// Player movement (only ran on local player)
	float dx;
	void Movement () {
		// Running
		float newdx = Input.GetAxisRaw ("Horizontal");
		if ((dx < 0 && newdx >= 0) || (dx > 0 && newdx <= 0) || (dx == 0 && newdx != 0)) {
			CmdSetRunDirection (newdx);
		}
		dx = newdx;

		if (Mathf.Abs (rb.velocity.x) < WalkSpeed) {
			rb.AddForce (new Vector2 (dx * WalkSpeed * .1f, 0));
		}

		// Ground below
		bool grounded = GetGrounded ();

		// Jumping and jetpack controls
		float dy = Input.GetAxisRaw ("Vertical");
		if (JetPackEnabled) {
			rb.gravityScale = Input.GetAxis ("Vertical") * -4 - .1f;
		} else {
			rb.gravityScale = BaseGravity;
			if (dy > 0 && Mathf.Abs (rb.velocity.y) < .3f && grounded) {
				float tempJumpPower = JumpPower;
				if (JetPackEnabled)
					tempJumpPower *= .4f;
				rb.AddForce (new Vector2 (0, tempJumpPower));
				Vector2 tempVel = rb.velocity;
				tempVel.y++;
				rb.velocity = tempVel;
			}
		}
	}

	void Animation () {
		bool grounded = GetGrounded ();

		bool Running = dx != 0 && ((!JetPackEnabled) || (JetPackEnabled && grounded));
		LegAnimator.SetBool ("running", Running);
		bool backwards = (facingRight && dx < 0) || (!facingRight && dx > 0);
		LegAnimator.SetBool ("backwards", backwards);
		LegAnimator.SetBool ("jumping", Mathf.Abs (rb.velocity.y) > 2f || !grounded);
		if (JetPack.activeInHierarchy)
			JetPackAnimator.SetBool ("On", JetPackEnabled);

		if (Running && grounded) {
			RunningParticles.enableEmission = true;
			Vector3 ParticleRotation = RunningParticles.transform.eulerAngles;
			if ((facingRight && !backwards) || (!facingRight && backwards)) {
				ParticleRotation.y = 0;
			} else {
				ParticleRotation.y = 180;
			}
			RunningParticles.transform.eulerAngles = ParticleRotation;
		} else {
			RunningParticles.enableEmission = false;
		}

	}

	public GroundCollider GroundColliderScript;
	bool GetGrounded () {
		return GroundColliderScript.Grounded;
		//return Physics2D.Linecast (new Vector2 (transform.position.x, transform.position.y), new Vector2 (transform.position.x, transform.position.y - 2),
		//	1 << LayerMask.NameToLayer ("Ground"));
	}

	[Command]
	void CmdSetRunDirection (float dx) {
		RpcSetRunDirection (dx);
	}
	[ClientRpc]
	void RpcSetRunDirection (float dx) {
		this.dx = dx;
	}

	// Flip the direction the player character is facing
	void Flip (bool right) {
		//if (right != facingRight) {
		facingRight = right;
		Vector3 flippedScale = transform.localScale;
		flippedScale.x = Mathf.Abs (flippedScale.x);
		if (!right)
			flippedScale.x *= -1;
		transform.localScale = flippedScale;

		PositionRightArm ();
		PositionLeftArm ();
		//}
	}

	private MainGUI GUI;
	// Updates all parts of the GUI
	void UpdateGUI () {
		GUI.DisplayHealth (Health, MaxHealth);
		GUI.WeaponDisplayOne.text = RightArmWeapon.GUI ();
		GUI.WeaponDisplayTwo.text = LeftArmWeapon.GUI ();
	}

	// Apply damage and knockback to the player, only calculated on the server
	public void Damage (int DamageAmount, GameObject shooter, Vector3 force = default(Vector3)) {
		if (!isServer || Health == 0 || ForceField.activeInHierarchy)
			return;

		Health = Mathf.Max (Health - DamageAmount, 0);
		RpcFlashPlayerWhite ();
		RpcAddForce (force);
		if (isServer && isLocalPlayer) {
			FlashPlayerWhite ();
			rb.AddForce (force);
		}

		if (Health == 0) {
			GameManager.LogKill (shooter.GetComponent<NetworkIdentity> ().netId, netId);
			Die ();
		}
	}

	// Manages game information for when the player is killed
	public void Die () {
		UpdateGUI ();
		GameManager.CurrentGame.Kill (gameObject);
	}


	[ClientRpc]
	void RpcAddForce (Vector3 force) {
		if (isLocalPlayer)
			rb.AddForce (force);
	}

	// Flash the player white when damaged
	public Material SolidWhiteMaterial;
	public Material DefaultMaterial;
	[ClientRpc]
	void RpcFlashPlayerWhite () {
		FlashPlayerWhite ();
	}
	void FlashPlayerWhite () {
		BodySprite.material = SolidWhiteMaterial;
		LegSprite.material = SolidWhiteMaterial;
		RightArmSprite.material = SolidWhiteMaterial;
		LeftArmSprite.material = SolidWhiteMaterial;
		Invoke ("ResetPlayerColor", .08f);
	}
	void ResetPlayerColor () {
		BodySprite.material = DefaultMaterial;
		LegSprite.material = DefaultMaterial;
		RightArmSprite.material = DefaultMaterial;
		LeftArmSprite.material = DefaultMaterial;
	}

	// Set a player's alpha value;
	private List<ColorObject> colorObjects = new List<ColorObject> ();
	public void SetAlpha (float percent) {
		if (colorObjects.Count == 0) {
			SpriteRenderer[] alphaSprites = new SpriteRenderer[] { BodySprite, LegSprite, RightArmSprite, LeftArmSprite, RightUpperArmSprite, LeftUpperArmSprite, HeadSprite };
			foreach (SpriteRenderer s in alphaSprites) {
				if (s.GetComponent<ColorObject> () != null)
					colorObjects.Add(s.GetComponent<ColorObject> ());
			}
		}
		foreach (ColorObject c in colorObjects) {
			c.SetAlpha (percent);
		}
		SetAlpha (LeftArmSprite, percent);
		SetAlpha (RightArmSprite, percent);
	}

	public void SetAlpha (SpriteRenderer sp, float percent) {
		Color c = sp.color;
		c.a = percent;
		sp.color = c;
	}

	// The currently equipped powerup, stored locally
	PowerUp SecondPowerUp;
	GameObject SecondPowerUpObj;
	PowerUp EquippedPowerUp;
	GameObject EquippedPowerUpObj;
	// Detects the player colliding with a trigger
	void OnTriggerStay2D (Collider2D coll) {
		// Pick up a powerup, ran on local player
		if (isServer && coll.gameObject.layer == LayerMask.NameToLayer ("Items")) {
			PowerUp newPU = coll.GetComponent<PowerUp> ();

			// If this is true, then it is a powerup
			if (newPU != null) {
				if (SecondPowerUpObj == null) { // this means there is no powerup in the inventory
					SecondPowerUp = newPU;
					SecondPowerUpObj = coll.gameObject;
					SecondPowerUp.PickUp (this);
				}
			}
		}
	}

	// Uses the currently possessed powerup, ran on local player
	void PowerUpFunction () {
		if (Input.GetKeyDown (KeyCode.E)) {
			CmdEquipPowerUp ();
		}
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			CmdUsePowerUp ();
		}
	}

	[Command]
	void CmdEquipPowerUp () {
		if (SecondPowerUpObj != null) { // there is a powerup to use
			MainGUI.GUI.SetPowerUpIcon (null);
			if (EquippedPowerUpObj != null && EquippedPowerUpObj.name == SecondPowerUpObj.name) { // the secondary is the same as the primary
				//Debug.Log ("same type!");
				EquippedPowerUp.Setup ();
				NetworkServer.Destroy (SecondPowerUpObj);
			} else {
				if (EquippedPowerUpObj != null) {
					EquippedPowerUp.UnSetup ();
				}
				SecondPowerUp.Setup ();
				EquippedPowerUp = SecondPowerUp;
				EquippedPowerUpObj = SecondPowerUpObj;
			}

			SecondPowerUp = null;
			SecondPowerUpObj = null;
		}
	}
	[Command]
	void CmdUsePowerUp () {
		if (EquippedPowerUpObj != null) {
			EquippedPowerUp.UsePowerUp ();
		}
	}

	// These are used by the Jetpack class (and maybe other powerups in the future!)
	public void UnSetup () {
		CmdUnSetup ();
	}
	[Command]
	void CmdUnSetup () {
		if (EquippedPowerUp != null)
			EquippedPowerUp.UnSetup ();
	}
}