using UnityEngine;
using System.Collections;

public class ExplosionBehavior : MonoBehaviour {

	public SpriteRenderer Sprite;
	public GameObject shooter;
	public float ExplosionLength;
	private float TimeElapsed;
	public float MaxShakePower;
	public float MinShakePower;
	public float ShakeRadius;
	public float ExplosionForce;
	public int DamageValue;

	// Use this for initialization
	void Start () {
		CameraShake.cam.Shake (Mathf.Max (ShakeRadius / Vector3.Distance (CameraShake.cam.camMove.player.transform.position, transform.position) * MaxShakePower, MinShakePower));
	}

	// Update is called once per frame
	void Update () {
		TimeElapsed += Time.deltaTime;
		if (TimeElapsed > ExplosionLength * .7f)
			Sprite.color = Color.black;
		if (TimeElapsed > ExplosionLength)
			Destroy (gameObject);
	}

	void OnTriggerEnter2D (Collider2D other) {
		Damageable dmg = other.GetComponent<Damageable> ();
		if (dmg != null) {
			dmg.Damage (DamageValue, shooter, (other.transform.position - transform.position).normalized * ExplosionForce);
		}
	}
}
