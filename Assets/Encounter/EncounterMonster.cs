using System.Collections;
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
			var w = Util.Random(def.weapons);
			TakeAction(Attack(encounter.GetRandomPartyMember(), w.damage), () => encounter != null, w.readyTime);
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
