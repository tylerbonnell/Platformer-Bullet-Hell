using UnityEngine;
using System.Collections;

public interface PowerUp {

	// Called when the PowerUp is picked up, ON THE SERVER
	// Should vanish the item from the ground and add the icon to the GUI
	void PickUp (PlayerControls pc);

	// Called when the PowerUp is equipped, ON THE SERVER
	void Setup ();

	// Called when the PowerUp is removed or replaced with a DIFFERENT powerup, ON THE SERVER
	void UnSetup ();

	// Called when the player pressed the "use PowerUp" button, ON THE SERVER
	void UsePowerUp ();

}
