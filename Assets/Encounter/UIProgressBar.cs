using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIProgressBar : MonoBehaviour {
	[SerializeField] GameObject bg;
	[SerializeField] GameObject fg;
	public void SetPct(float pct) {
		fg.GetComponent<RectTransform>().sizeDelta = new Vector2(pct, 0.25f);
	}
}
