using UnityEngine;
using System.Collections;

// An entity is an object in the world that can be damaged
public interface Damageable {

	// Damages the object by DamageAmount
	// Passes in the person (shooter) responsible for the damage
	// Applies a physical force to the object
	void Damage (int DamageAmount, GameObject shooter, Vector3 force = default(Vector3));
}
