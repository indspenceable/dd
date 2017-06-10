using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : EncounterEntityBase {
	[SerializeField] int hp;

	public void SetEncounter(Encounter e) {
		this.e = e;
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
