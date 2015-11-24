using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Timer : MonoBehaviour {

	public Text TimerTextField;
	//private bool started = false;

	/*(public void StartTimer (int minutes, int seconds) {
		this.seconds = seconds;
		started = true;
	}*/

	void Start () {
		TimerTextField.text = "";
	}

	public void DisplayTime (int seconds) {
		string str = "" + ((int) seconds) / 60 + ":";
		string SecondsString = "" + (int) (seconds % 60);
		if (SecondsString.Length == 1)
			SecondsString = "0" + SecondsString;
		str += SecondsString;
		TimerTextField.text = str;
	}

	public void DisplayString (string str) {
		TimerTextField.text = str;
	}
}
