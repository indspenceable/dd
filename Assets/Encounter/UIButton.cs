using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class UIButton : MonoBehaviour {
	public void SetText(string t) {
		GetComponentInChildren<Text>().text = t;
	}
	public void SetOnClick(UnityAction action) {
		GetComponent<Button>().onClick.AddListener(action);
	}
}
