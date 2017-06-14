using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	[SerializeField] GameObject paused;
	[SerializeField] Tooltip toolTip;
	[SerializeField] TextBox textBox;
	[SerializeField] GameObject bouncingTextPrefab;
	[SerializeField] Canvas OverlayCanvas;
	[SerializeField] Canvas GameWorldCanvas;
	public bool Blocking {
		get {
			return textBox.blocking;
		}
	}
	public void SetPaused(bool p) {
		paused.gameObject.SetActive(p);
	}
	public IEnumerator TextBox(string s) {
		yield return textBox.CoroutineShow(s);
	}

	bool TT_Shown_This_Frame = false;
	public void ShowToolTip(string text, Vector2 pos) {
		toolTip.SetText(text);
		toolTip.gameObject.SetActive(true);
		toolTip.GetComponent<RectTransform>().anchoredPosition = pos;
		TT_Shown_This_Frame = true;
	}
	public void BounceText(string text, Color c, Vector3 pos) {
		Instantiate(bouncingTextPrefab, pos, Quaternion.identity, GameWorldCanvas.transform).GetComponent<BouncingText>().Setup(text, c);
	}
	void Update() {
		if (TT_Shown_This_Frame) {
			TT_Shown_This_Frame = false;
		} else {
			toolTip.gameObject.SetActive(false);
		}
	}
}
