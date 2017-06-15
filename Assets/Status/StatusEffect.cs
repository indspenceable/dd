﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatusModifier {
	public int armor = 0;
	public float speed = 1f;
}

[System.Serializable]
public class ActivationEffect {
	public int damage;
	public StatusEffect effect;
}

[CreateAssetMenu(fileName = "Status", menuName = "Status", order = 1)]
public class StatusEffect : ScriptableObject{
	public enum TriggerMode {
		NEVER,
		ROUNDS,
		TIME,
	}

	public string effectName;
	public Sprite icon;
	public StatusModifier modifier;
	public TriggerMode triggerMode;
	public int numberOfTriggers = 0;
	public float timeBetweenTriggers;
	public ActivationEffect activationEffect;

	public StatusEffectInstance BuildInstance() {
		return new StatusEffectInstance(this, numberOfTriggers);
	}
}

public class StatusEffectInstance {
	public StatusEffectInstance(StatusEffect se, int numberOfTriggers) {
		this.myEffect = se;
		this.remainingTriggers = numberOfTriggers;
	}
	public void Trigger(EncounterEntityBase target) {
		if (myEffect.activationEffect.damage != 0) {
			target.TakeDamage(myEffect.activationEffect.damage, true);
		}
		remainingTriggers -= 1;
	}
	public StatusEffect myEffect;
	public int remainingTriggers;
}