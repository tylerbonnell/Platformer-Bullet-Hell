using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
	
	// How long the object should shake for.
	public float shake = 0f;
	private float originalShake;
	
	// Amplitude of the shake. A larger value shakes the camera harder.
	public float shakePower;
	float decreaseFactor = 10f;

	public CameraMovement camMove;
	public static CameraShake cam;
	
	Vector3 originalPos;

	void Start(){
		originalPos = transform.localPosition;
		cam = this;
	}

	public void Shake(float newShakePower, float shakeLength = 5)
	{
		if (shake <= 0)
			originalPos = transform.localPosition;
		shakePower = Mathf.Max (shakePower * (shake / originalShake), newShakePower);
		shake = Mathf.Max(shake, shakeLength);
		shake = shakeLength;
		originalShake = shake;
		//Debug.Log ("shake!");
	}
	
	void Update()
	{
		originalPos = camMove.CalculatePosition (originalPos);
		if (shake > 0)
		{
			transform.localPosition = originalPos + Random.insideUnitSphere * shakePower * (shake / originalShake);
			
			shake -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shake = 0f;
			shakePower = 0f;
			transform.localPosition = originalPos;
		}
	}
}