using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	[SerializeField] GameObject paused;
	[SerializeField] Tooltip toolTip;
	[SerializeField] public TextBox textBox;
	public bool Blocking {
		get {
			return textBox.blocking;
		}
	}
	public void SetPaused(bool p) {
		paused.gameObject.SetActive(p);
	}
	bool TT_Shown_This_Frame = false;
	public void ShowToolTip(string text, Vector2 pos) {
		toolTip.SetText(text);
		toolTip.gameObject.SetActive(true);
		toolTip.GetComponent<RectTransform>().anchoredPosition = pos;
		TT_Shown_This_Frame = true;
	}
	public void Update() {
		if (TT_Shown_This_Frame) {
			TT_Shown_This_Frame = false;
		} else {
			toolTip.gameObject.SetActive(false);
		}
	}
}
