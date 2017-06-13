using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBox : MonoBehaviour {
	[SerializeField] Text text;
	[SerializeField] GameObject container;
	public bool blocking {
		get;
		private set;
	}
	public void Show(string s) {
		text.text = s;

		container.SetActive(true);
		blocking = true;
	}
	public void Hide() {
		container.SetActive(false);
		blocking = false;
	}
	public IEnumerator CoroutineShow(string s) {
		Show(s);
		while (blocking) {
			yield return null;
		}
	}

	public void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			Hide();
		}
	}
}
