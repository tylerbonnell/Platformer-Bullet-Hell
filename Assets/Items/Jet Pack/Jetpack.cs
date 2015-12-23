using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Jetpack : NetworkBehaviour, PowerUp {

	PlayerControls pc;
	public float MaxFuel;
	public float CurrentFuel;
	public Sprite Icon;
	public SpriteRenderer childSR;

	public void PickUp (PlayerControls pc) {
		this.pc = pc;
		RpcPickUp (pc.netId);
	}
	[ClientRpc]
	void RpcPickUp (NetworkInstanceId id) {
		if (pc == null)
			pc = ClientScene.FindLocalObject (id).GetComponent<PlayerControls> ();	
		childSR.enabled = false;
		GetComponent<CircleCollider2D> ().enabled = false;
		if (pc.isLocalPlayer)
			MainGUI.GUI.SetPowerUpIcon (Icon);
	}

	// Called when the PowerUp is added to the inventory
	public void Setup () {
		RpcSetup ();
		//MainGUI.GUI.FadeOut (MainGUI.GUI.FuelTransform);
	}
	private bool equipped;
	[ClientRpc]
	void RpcSetup () {
		equipped = true;
		pc.JetPack.SetActive (true);
		CurrentFuel = MaxFuel;	
		if (pc.isLocalPlayer) {
			MainGUI.GUI.SetFuelVisible (true);
		}
		
	}

	// Called when this powerup is depleted or a new powerup replaces it
	public void UnSetup () {
		RpcUnSetup ();
		//NetworkServer.Destroy (gameObject);
	}
	[ClientRpc]
	void RpcUnSetup () {
		//Debug.Log ("UnSetup!");
		pc.JetPack.SetActive (false);
		pc.JetPackEnabled = false;

		//MainGUI.GUI.FadeOut (MainGUI.GUI.FuelTransformEmpty);
		if (pc.isLocalPlayer) {
			MainGUI.GUI.SetFuelVisible (false);
		}
		Destroy (gameObject);
	}

	// Called when the player pressed the "use PowerUp" button
	public void UsePowerUp () {
		if (pc.JetPack.activeInHierarchy) {
			RpcUsePowerUp ();
		}
	}
	[ClientRpc]
	void RpcUsePowerUp () {
		pc.JetPackEnabled = !pc.JetPackEnabled;
	}

	// Updates the fuel amount and shows on the GUI for local player
	void Update () {
		if (pc == null || !pc.isLocalPlayer || !equipped)
			return;
	
		MainGUI.GUI.DisplayFuel (CurrentFuel, MaxFuel);
		if (pc.JetPackEnabled) {
			CurrentFuel -= Time.deltaTime;
			if (CurrentFuel <= 0f) {
				pc.UnSetup ();
			}
		}
	}
}