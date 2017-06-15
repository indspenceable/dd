using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterMonster : EncounterEntityBase {
	private MonsterDefinition def;
	private int damage = 0;
	private List<Item> items = new List<Item>();

	public void Setup(Encounter e, MonsterDefinition def) {
		this.encounter = e;
		this.def = def;
		GetComponent<SpriteRenderer>().sprite = def.image;
		foreach(var id in def.items) {
			items.Add(new Item(e.session.itemDefs.IndexOf(id)));
		}
	}

	new void Update() {
		if (this.currentAction == null) {
			// Try to take 
			var i = Util.Random(items);
			var w = i.GetDef(encounter.session);
			if (w.target == ItemDefinition.TargetMode.ENEMY) {
				var target = Util.Random(encounter.GetPartyMembers());
				TakeAction(UseItem(target, i), () => encounter != null && target != null, w.readyTime);
			} else if (w.target == ItemDefinition.TargetMode.FRIENDLY) {
				var target = Util.Random(encounter.GetMonsters());
				TakeAction(UseItem(target, i), () => encounter != null && target != null, w.readyTime);

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
	protected override int BaseArmor() {
		return def.armor;
	}
	protected override int Damage() {
		return damage;
	}
	protected override float Evasion() {
		return def.evasion;
	}
	protected override void SetDamage(int damage) {
		this.damage = damage;
	}
	protected override List<Item> Items() {
		return items;
	}

	void OnMouseDown() {
		encounter.ClickMonster(this);
	}

	protected override void Destroy() {
		encounter.DestroyMonster(this);
	}
}
