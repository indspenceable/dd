using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIcon : MonoBehaviour {
	[SerializeField] Image bg;
	[SerializeField] Image fg;
	[SerializeField] Sprite positiveBg;
	[SerializeField] Sprite negativeBg;
	public void Set(StatusEffectInstance s) {
		bg.sprite = s.definition.isBuff ? positiveBg : negativeBg;
		fg.sprite = s.definition.icon;
	}
}
