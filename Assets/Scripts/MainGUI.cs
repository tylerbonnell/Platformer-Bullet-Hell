using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainGUI : MonoBehaviour {
	public static MainGUI GUI;

	public RectTransform HealthBarTransform;
	public Text WeaponDisplayOne;
	public Text WeaponDisplayTwo;
	public Timer Timer;
	public RectTransform FuelTransform;
	public RectTransform FuelTransformEmpty;
	public Image PowerUpIcon;

	void Start () {
		GUI = this;
		//FuelImage = FuelTransform.GetComponent<Image> ();
		//FuelEmptyImage = FuelTransformEmpty.GetComponent<Image> ();
		//PowerUpIcon.CrossFadeAlpha (0f, 0f, true);
		//FuelImage.CrossFadeAlpha (0f, 0f, true);
		//FuelEmptyImage.CrossFadeAlpha (0f, 0f, true);
	}

	// Displays the health bar
	private float HealthBarMaxWidth = 120f;
	public void DisplayHealth (int Health, int MaxHealth) {
		HealthBarTransform.localScale = Vector3.Lerp (HealthBarTransform.localScale,
												new Vector3 (HealthBarMaxWidth * Health / MaxHealth, HealthBarTransform.localScale.y,
												HealthBarTransform.localScale.z), .05f);
		if (HealthBarTransform.localScale.x < .5f && Health > 0) {
			HealthBarTransform.localScale = new Vector3 (.5f, HealthBarTransform.localScale.y, HealthBarTransform.localScale.z);
		} else if (Health == 0) {
			HealthBarTransform.localScale = new Vector3 (0, HealthBarTransform.localScale.y, HealthBarTransform.localScale.z);
		}
	}

	// Displays the amount of fuel remaining
	private float FuelBarMaxWidth = 80f;
	public void DisplayFuel (float Fuel, float MaxFuel) {
		FuelTransform.localScale = Vector3.Lerp (FuelTransform.localScale,
												new Vector3 (FuelBarMaxWidth * Fuel / MaxFuel, FuelTransform.localScale.y,
												FuelTransform.localScale.z), .05f);
		if (FuelTransform.localScale.x < .5f && Fuel > 0) {
			FuelTransform.localScale = new Vector3 (.5f, FuelTransform.localScale.y, FuelTransform.localScale.z);
		} else if (Fuel == 0) {
			FuelTransform.localScale = new Vector3 (0, FuelTransform.localScale.y, FuelTransform.localScale.z);
		}
	}

	public void SetFuelVisible (bool visible) {
		if (visible) {
			SetPowerUpYPosition (32f);
			FuelTransform.gameObject.SetActive (true);
			FuelTransformEmpty.gameObject.SetActive (true);
			//FuelImage.CrossFadeAlpha (1f, FadeSpeed, true);
			//FuelEmptyImage.CrossFadeAlpha (1f, FadeSpeed, true);
		} else {
			SetPowerUpYPosition (22f);
			FuelTransform.gameObject.SetActive (false);
			FuelTransformEmpty.gameObject.SetActive (false);
			//FuelImage.CrossFadeAlpha (0f, FadeSpeed, true);
			//FuelEmptyImage.CrossFadeAlpha (0f, FadeSpeed, true);
		}
	}

	public void SetPowerUpIcon (Sprite s) {
		if (s == null) {
			PowerUpIcon.gameObject.SetActive (false);
			//PowerUpIcon.CrossFadeAlpha (0f, FadeSpeed, true);
		} else {
			//PowerUpIcon.CrossFadeAlpha (1f, FadeSpeed, true);
			PowerUpIcon.gameObject.SetActive (true);
			PowerUpIcon.sprite = s;
		}
	}

	void SetPowerUpYPosition (float yPos) {
		Vector2 pos = PowerUpIcon.rectTransform.anchoredPosition;
		pos.y = yPos;
		PowerUpIcon.rectTransform.anchoredPosition = pos;
	}
}
