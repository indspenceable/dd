using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterPartyMember : EncounterEntityBase {
	public class Selected : Encounter.ActionListener {
		private EncounterPartyMember p;
		private Encounter e;
		Encounter.ActionListener prev;
		private List<UIButton> partyMemberActions;

		public Selected(EncounterPartyMember p, Encounter.ActionListener previous, Encounter e) {
			this.p = p;
			this.e = e;
			this.prev = previous;
		}

		public void PartyMemberClicked(EncounterPartyMember target) {
			e.SelectPartyMember(target);
		}
		public void MonsterClicked(EncounterMonster m) {}
		public void Cancel() {
			e.InstallListener(prev);
		}
		public void InstallUI() {
			p.MarkSelected(true);
			partyMemberActions = new List<UIButton>();
			// Display the buttons for actions!
			//TODO this should go for each item in the inventory
			var Buttons = new List<GameObject>();
			for (int i = 0; i < p.backingPartyMember.inventory.Count; i += 1)
			{
				UIButton go = e.CreateUIButton();
				var weapon = p.backingPartyMember.inventory[i];
				go.SetImage(weapon.GetDef(e.session).image);
				go.SetTooltip(weapon.GetDef(e.session).Tooltip(), e.session);
				var listener = new ApplyItem(p, i, this, e);
				go.SetOnClick(() => e.InstallListener(listener));
				partyMemberActions.Add(go);
				Buttons.Add(go.gameObject);
			}
			{
				UIButton go = e.CreateUIButton();
				go.SetImage(e.swapSprite);
				go.SetOnClick(() => e.InstallListener(new Swap(p, this, e)));
				partyMemberActions.Add(go);
				Buttons.Add(go.gameObject);
			}
			for(int i = 0; i <Buttons.Count; i+=1) {
				float theta = (180 + Mathf.Lerp(0f, 180f, (i+1)/(float)(Buttons.Count+1)));
				float thetaRad = Mathf.Deg2Rad * theta;
				Buttons[i].transform.position = p.transform.position + new Vector3(Mathf.Cos(thetaRad), Mathf.Sin(thetaRad))*1.5f;
			}
		}
		public void UninstallUI() {
			p.MarkSelected(false);
			foreach(var bu in partyMemberActions) {
				Destroy(bu.gameObject);
			}
		}
	}

	public class ApplyItem : Encounter.ActionListener {
		private EncounterPartyMember p;
		private Encounter e;
		Encounter.ActionListener prev;
		ItemDefinition item;

		public ApplyItem(EncounterPartyMember p, int w, Encounter.ActionListener previous, Encounter e) {
			this.p = p;
			this.e = e;
			this.prev = previous;
			this.item = p.backingPartyMember.inventory[w].GetDef(e.session);
		}

		private void Act(EncounterEntityBase eeb) {
			p.TakeAction(p.UseItem(eeb, item), () => eeb != null, item.readyTime);
			e.InstallListener(null);
		}

		public void PartyMemberClicked(EncounterPartyMember target) {
			if (item.target == ItemDefinition.TargetMode.FRIENDLY) {
				Act(target);
			}
		}
		public void MonsterClicked(EncounterMonster target) {
			if (item.target == ItemDefinition.TargetMode.ENEMY) {
				Act(target);
			}
		}
		public void Cancel() {
			e.InstallListener(prev);
		}
		public void InstallUI() {
			p.MarkSelected(true);
		}
		public void UninstallUI() {
			p.MarkSelected(false);
		}
	}

	public class Swap : Encounter.ActionListener {
		private EncounterPartyMember p;
		private Encounter e;
		Encounter.ActionListener prev;

		public Swap(EncounterPartyMember p, Encounter.ActionListener previous, Encounter e) {
			this.p = p;
			this.e = e;
			this.prev = previous;
		}

		public void PartyMemberClicked(EncounterPartyMember target) {
			p.TakeAction(p.SwapWith(target), () => target != null, 3f);
			e.InstallListener(null);
		}
		public void MonsterClicked(EncounterMonster m) {
			// Can't swap with enemy! Yet.
			// TODO make this work?
		}
		public void Cancel() {
			e.InstallListener(prev);
		}
		public void InstallUI() {
			p.MarkSelected(true);
		}
		public void UninstallUI() {
			p.MarkSelected(false);
		}
	}

	protected override void Destroy() {
		throw new System.NotImplementedException("EncounterPartyMember#Destroy not implemented");
	}

	public void Setup(Encounter encounter, PartyMember p) {
		this.encounter = encounter;
		this.backingPartyMember = p;
		GetComponent<SpriteRenderer>().sprite = p.GetImage(encounter.session);
	}

	// TODO these stats should be stored in the GameState
	protected override int HP() {
		return 50;
	}
	protected override int Armor() {
		return 0;
	}

	public IEnumerator SwapWith(EncounterPartyMember p) {
		float dt = 0f;
		float duration = 1f;
		Vector3 p1 = transform.position;
		Vector3 p2 = p.transform.position;
		while (dt < duration ) {
			yield return null;

			dt += encounter.session.DT();
			transform.position = Vector3.Lerp(p1, p2, dt/duration);
			p.transform.position = Vector3.Lerp(p2, p1, dt/duration);
		}
		// TODO if someone dies in the middle of a swap
		Hotspot h1 = this.hotspot;
		Hotspot h2 = p.hotspot;
		h1.SetPartyMember(p);
		h2.SetPartyMember(this);
	}

	void OnMouseDown() {
		encounter.ClickPartyMember(this);
	}
}
