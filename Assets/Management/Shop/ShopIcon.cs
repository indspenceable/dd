using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ShopIcon : MonoBehaviour {
	Image uiImage;
	SessionManager session;
	string tooltip;
	UnityAction onClick;
	public void Init(SessionManager session)  {
		this.session = session;
		this.uiImage = GetComponent<Image>();
		Clear();
	}
	public void Clear() {
		Set(null, null, null);
	}
	public void Set(string tooltip, Sprite image, UnityAction onClick) {
		if (image == null){
			uiImage.enabled = false;
		} else {
			uiImage.enabled = true;
			uiImage.sprite = image;
		}
		this.tooltip = tooltip;
		this.onClick = onClick;
	}

	public void OnMouseDown(){
		if (onClick != null) {
			onClick.Invoke();
		}
	}
	public void OnMouseOver(){
		if (tooltip != null) {
			session.ui.ShowToolTip(tooltip, Input.mousePosition);
		}
	}
}
