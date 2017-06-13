using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class UIButton : MonoBehaviour {
	private UnityAction action;
	private string tooltipText;
	private SessionManager session;
	public void SetImage(Sprite s) {
		GetComponent<SpriteRenderer>().sprite = s;
	}
	public void SetTooltip(string tooltipText, SessionManager session) {
		this.tooltipText = tooltipText;
		this.session = session;
	}
	public void SetOnClick(UnityAction action) {
		this.action = action;
	}
	void OnMouseDown() {
		if (action != null) {
			action.Invoke();
		}
	}
	void OnMouseOver() {
		if (tooltipText != null) {
			session.ui.ShowToolTip(tooltipText, Input.mousePosition);
		}
	}
}
