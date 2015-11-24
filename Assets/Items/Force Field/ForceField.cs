using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ForceField : NetworkBehaviour, PowerUp {

	private PlayerControls pc;
	public Sprite Icon;

	public void PickUp (PlayerControls pc) {
		this.pc = pc;
		RpcPickUp (pc.netId);
	}
	[ClientRpc]
	void RpcPickUp (NetworkInstanceId id) {
		if (pc == null)
			pc = ClientScene.FindLocalObject (id).GetComponent<PlayerControls> ();
		GetComponent<SpriteRenderer> ().enabled = false;
		GetComponent<CircleCollider2D> ().enabled = false;
		if (pc.isLocalPlayer)
			MainGUI.GUI.SetPowerUpIcon (Icon);
	}

	// Called when the PowerUp is added to the inventory, ON THE SERVER
	public void Setup () {
		RpcSetup ();
		CancelInvoke ();
		Invoke ("UnSetup", 5f);
	}
	[ClientRpc]
	void RpcSetup () {		
		GetComponent<SpriteRenderer> ().enabled = false;
		GetComponent<CircleCollider2D> ().enabled = false;

		pc.ForceField.SetActive (true);

	}

	// Called when the PowerUp is removed or replaced with a DIFFERENT powerup, ON THE SERVER
	public void UnSetup () {
		RpcUnsetup ();
	}
	[ClientRpc]
	void RpcUnsetup () {
		pc.ForceField.SetActive (false);
		Destroy (gameObject);
	}

	// Called when the player pressed the "use PowerUp" button, ON THE SERVER
	public void UsePowerUp () {	}
}
