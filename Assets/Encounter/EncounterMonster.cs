using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMonster : EncounterEntityBase {
	[SerializeField] int hp;

	public void Setup(Encounter e, MonsterDefinition def) {
		this.e = e;
		GetComponent<SpriteRenderer>().sprite = def.image;
	}

	void Update() {
		if (this.currentAction == null) {
			// Try to take 
			TakeAction(Attack(e.GetRandomPartyMember(), 3), () => e != null, 7f);
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
