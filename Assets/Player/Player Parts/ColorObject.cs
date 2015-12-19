using UnityEngine;
using System.Collections;

public class ColorObject : MonoBehaviour {

	public SpriteRenderer MainSprite;
	public SpriteRenderer SecondarySprite;

	public Material defaultMaterial;
	public Material solidWhiteMaterial;

	private Color mainBackupColor = Color.white;
	private Color secondaryBackupColor = Color.white;

	private bool isWhite = false;

	public void SetColors (Color Main, Color Secondary) {
		SetMainColor (Main);
		SetSecondaryColor (Secondary);
	}

	public void SetMainColor (Color c) {
		MainSprite.color = c;
	}

	public void SetSecondaryColor (Color c) {
		if (SecondarySprite != null)
			SecondarySprite.color = c;
	}

	public void SetAlpha (float percent) {
		Color c = MainSprite.color;
		c.a = percent;
		MainSprite.color = c;

		if (SecondarySprite != null) {
			c = SecondarySprite.color;
			c.a = percent;
			SecondarySprite.color = c;
		}
	}

	public void SetMaterial (Material m) {
		MainSprite.material = m;
		if (SecondarySprite != null) {
			SecondarySprite.material = m;
		}
	}

	public void ToggleFlashing (bool enable) {
		if (enable && !isWhite) {
			SetMaterial (solidWhiteMaterial);
			mainBackupColor = MainSprite.color;
			if (SecondarySprite != null)
				secondaryBackupColor = SecondarySprite.color;
			SetColors (Color.white, Color.white);
		} else if (!enable) {
			SetMaterial(defaultMaterial);
			SetColors(mainBackupColor, secondaryBackupColor);
		}
		isWhite = enable;
	}
}
