using UnityEngine;
using System.Collections;

public interface Weapon {
	float Shoot (bool isTriggerDown); // shoot should be called every cycle
	void Reload ();
	bool CanRotate ();
	string GUI ();
}
