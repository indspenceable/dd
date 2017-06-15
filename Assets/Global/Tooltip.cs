using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {
	[SerializeField] Text text;

	[HideInInspector] 
	public RectTransform rectTransform;

	void Start() {
		rectTransform = GetComponent<RectTransform>();
	}
	public void SetText(string t) {
		text.text = t;
	}
}
