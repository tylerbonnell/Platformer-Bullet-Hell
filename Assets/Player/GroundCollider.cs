using UnityEngine;
using System.Collections;

public class GroundCollider : MonoBehaviour {

	public bool Grounded;
	private Rigidbody2D rb;
	public ParticleSystem LandingParticles;

	void Start () {
		rb = transform.parent.GetComponent<Rigidbody2D> ();
	}

	private float DownwardSpeedToShowParticles = -10f;
	void OnTriggerEnter2D (Collider2D other) {
		if (!Grounded && rb.velocity.y < DownwardSpeedToShowParticles) {
			LandingParticles.Play ();
		}
	}

	void OnTriggerStay2D (Collider2D other) {
		Grounded = true;
	}

	void OnTriggerExit2D (Collider2D other) {
		Grounded = false;
	}
}
