using UnityEngine;
using System.Collections;

public class SlowMo : MonoBehaviour {

	public float _Speed;
	public float _DurationOfSlowMo;
	public float _TransitionLength;

	private static float TimeElapsed;
	private static float Speed_;
	private static float DurationOfSlowMo_;
	private static float TransitionLength_;
	private static bool isInSlowMo;
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKeyDown ("f")) {
			isInSlowMo = true;
			SlowTime (_Speed, _DurationOfSlowMo, _TransitionLength);
		}*/

		if (isInSlowMo) {
			if (TimeElapsed < DurationOfSlowMo_) {
				Time.timeScale = Speed_;
			} else if (TimeElapsed < DurationOfSlowMo_ + TransitionLength_) {
				Time.timeScale = Speed_ + (TimeElapsed - DurationOfSlowMo_)/TransitionLength_ * (1 - Speed_);
			} else {
				isInSlowMo = false;
				Time.timeScale = 1f;
			}
			TimeElapsed += Time.deltaTime;
			Debug.Log(Time.timeScale);
		}
	}

	public static void SlowTime (float Speed, float DurationOfSlowMo, float TransitionLength) {
		isInSlowMo = true;
		TimeElapsed = 0;
		Speed_ = Speed;
		DurationOfSlowMo_ = DurationOfSlowMo;
		TransitionLength_ = TransitionLength;
	}
}
