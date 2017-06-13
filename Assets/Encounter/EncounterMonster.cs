﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMonster : EncounterEntityBase {
	private MonsterDefinition def;

	public void Setup(Encounter e, MonsterDefinition def) {
		this.encounter = e;
		this.def = def;
		GetComponent<SpriteRenderer>().sprite = def.image;
	}

	new void Update() {
		if (this.currentAction == null) {
			// Try to take 
			var w = Util.Random(def.items);
			if (w.target == ItemDefinition.TargetMode.ENEMY) {
				TakeAction(UseItem(Util.Random(encounter.GetPartyMembers()), w), () => encounter != null, w.readyTime);
			} else if (w.target == ItemDefinition.TargetMode.FRIENDLY) {
				TakeAction(UseItem(Util.Random(encounter.GetMonsters()), w), () => encounter != null, w.readyTime);

//			} else if 	(w.target == ItemDefinition.TargetMode.SELF) {
			} else {
				// Uhhh..... Should never get here.
				Debug.LogError("Unknown target type. " + w.target);
			}
		}
		base.Update();
	}

	protected override int HP() {
		return def.hp;
	}
	protected override int Armor() {
		return def.armor;
	}

	void OnMouseDown() {
		encounter.ClickMonster(this);
	}

	protected override void Destroy() {
		encounter.DestroyMonster(this);
	}
}
