using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
	[SerializeField] GameObject paused;
	[SerializeField] Tooltip toolTip;
	[SerializeField] TextBox textBox;
	[SerializeField] MoneyDisplay money;
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
	public IEnumerator TextBox(string s, params TextBox.TextboxChoice[] choices) {
		yield return textBox.Show(s, choices);
	}

	public void SetActive(bool active) {
		money.gameObject.SetActive(active);
	}

	public void SetMoney(int money) {
		this.money.Set(money);
	}

	bool TT_Shown_This_Frame = false;
	public void ShowToolTip(string text, Vector2 pos) {
		toolTip.SetText(text);
		toolTip.gameObject.SetActive(true);

		toolTip.rectTransform.anchoredPosition = pos;

		Vector3[] v = new Vector3[4];
		toolTip.rectTransform.GetWorldCorners (v);

		float maxX = Mathf.Max (v [0].x, v [1].x, v [2].x, v [3].x);
		float maxY = Mathf.Max (v [0].y, v [1].y, v [2].y, v [3].y);


		if (maxX > Screen.width){
			toolTip.rectTransform.anchoredPosition = pos - new Vector2(toolTip.rectTransform.rect.size.x, 0);
		}
		if (maxY > Screen.height) {
			toolTip.rectTransform.anchoredPosition = pos - new Vector2(0, toolTip.rectTransform.rect.size.y);
		}
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
