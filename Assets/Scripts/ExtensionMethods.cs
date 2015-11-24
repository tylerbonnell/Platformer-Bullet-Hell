namespace ExtensionMethods {
	using UnityEngine;

	public static class MyExtensionMethods {
		public static void SetAlpha (this SpriteRenderer sp, float percent) {
			Color c = sp.color;
			c.a = percent;
			sp.color = c;
		}
	}
}