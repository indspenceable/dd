using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMonster : EncounterEntityBase {
	[SerializeField] int hp;
	private MonsterDefinition def;

	public void Setup(Encounter e, MonsterDefinition def) {
		this.e = e;
		this.def = def;
		GetComponent<SpriteRenderer>().sprite = def.image;
	}

	new void Update() {
		if (this.currentAction == null) {
			// Try to take 
			var w = Util.Random(def.weapons);
			TakeAction(Attack(e.GetRandomPartyMember(), w.damage), () => e != null, w.readyTime);
		}
		base.Update();
	}

	protected override int HP() {
		return hp;
	}

	void OnMouseDown() {
		e.ClickMonster(this);
	}

	protected override void Destroy() {
		e.DestroyMonster(this);
	}
}
