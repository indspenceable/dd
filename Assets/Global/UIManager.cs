using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	[SerializeField] GameObject paused;
	[SerializeField] Tooltip tt;
	public void SetPaused(bool p) {
		paused.gameObject.SetActive(p);
	}
	bool TT_Shown_This_Frame = false;
	public void ShowToolTip(string text, Vector2 pos) {
		tt.SetText(text);
		tt.gameObject.SetActive(true);
		tt.GetComponent<RectTransform>().anchoredPosition = pos;
		TT_Shown_This_Frame = true;
	}
	public void Update() {
		if (TT_Shown_This_Frame) {
			TT_Shown_This_Frame = false;
		} else {
			tt.gameObject.SetActive(false);
		}
	}
}
