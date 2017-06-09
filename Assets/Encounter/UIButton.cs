using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class UIButton : MonoBehaviour {
	public void SetImage(Sprite s) {
		GetComponent<SpriteRenderer>().sprite = s;
	}
	private UnityAction action;
	public void SetOnClick(UnityAction action) {
		this.action = action;
	}
	void OnMouseDown() {
		if (action != null) {
			action.Invoke();
		}
	}
}
