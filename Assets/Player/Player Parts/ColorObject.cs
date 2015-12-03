using UnityEngine;
using System.Collections;

public class ColorObject : MonoBehaviour {

	public SpriteRenderer MainSprite;
	public SpriteRenderer SecondarySprite;

	public void SetColors (Color Main, Color Secondary) {
		SetMainColor (Main);
		SetSecondaryColor (Secondary);
	}

	public void SetMainColor (Color c) {
		MainSprite.color = c;
	}

	public void SetSecondaryColor (Color c) {
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
		SecondarySprite.material = m;
	}
}
