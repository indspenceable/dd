using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {
	[SerializeField] Text text;
	public void SetText(string t) {
		text.text = t;
	}
}
