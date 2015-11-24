using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// This class adds the arms to their respective players over the network. It also handles arm animation states.

public class WeaponAddToPlayer : NetworkBehaviour {

	[SyncVar]
	public NetworkInstanceId PlayerNetId;
	[SyncVar]
	public bool isRightArm;

	private PlayerControls pc;

	// Update is called once per frame
	void Update () {
		if (transform.parent == null) {
			GameObject Player = ClientScene.FindLocalObject (PlayerNetId);
			if (Player != null) {
				pc = Player.GetComponent<PlayerControls> ();
				if (isRightArm)
					pc.ConfigureRightArm (transform);
				else
					pc.ConfigureLeftArm (transform);
			}
		}
	}

	public void SetUpperArmTrigger (string trigger) {
		if (pc != null) {
			pc.SetUpperArmTrigger (trigger, isRightArm);
		} else {
			Debug.Log("pc is null");
		}
	}
}
