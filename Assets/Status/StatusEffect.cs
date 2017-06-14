using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusModifier {
	public int armor = 0;
	public float speed = 1f;
}

[CreateAssetMenu(fileName = "Status", menuName = "Status", order = 1)]
public class StatusEffect : ScriptableObject{
	public string effectName;
	public Sprite icon;
	float duration = 10f;
	public StatusModifier modifier;
	public StatusEffectInstance BuildInstance() {
		return new StatusEffectInstance(this, duration);
	}
}

public class StatusEffectInstance {
	public StatusEffectInstance(StatusEffect se, float duration) {
		this.myEffect = se;
		this.remainingDuration = duration;
	}
	public StatusEffect myEffect;
	public float remainingDuration;
}