using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour {
	[SerializeField] Text text;
	[SerializeField] GameObject container;
	private static List<KeyCode> KEYCODES = new List<KeyCode>{KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4};

	public bool blocking {
		get;
		private set;
	}

	public struct TextboxChoice {
		public string s;
		public IEnumerator cb;
		public TextboxChoice(string s, IEnumerator cb) {
			this.s = s;
			this.cb = cb;
		}
	}

	private void _Show(string s) {
		text.text = s;

		container.SetActive(true);
		blocking = true;
	}
	private void _Hide() {
		container.SetActive(false);
		blocking = false;
	}

	public IEnumerator Show(string s, params TextboxChoice[] choicesArray) {
		var choices = new List<TextboxChoice>(choicesArray);

		IEnumerator finalChoice = null;
		if (choices.Count > 0) {
			string fullStr = s + "\n";
			for (int i = 0; i < choices.Count; i += 1) {
				fullStr += "\n" + (i+1) + " - " + choices[i].s;
			}
			_Show(fullStr);
			while (finalChoice == null ) {
				yield return null;
				for (int i = 0; i < choices.Count; i += 1) {
					if (Input.GetKeyDown(KEYCODES[i])) {
						finalChoice = choices[i].cb;
					}
				}
			}
		} else {
			_Show(s);
			while (!Input.GetKeyDown(KeyCode.Space)) {
				yield return null;
			}
		}
		_Hide();
		yield return finalChoice;
	}
}
