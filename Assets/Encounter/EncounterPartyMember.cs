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
			Debug.Log("Clicked on another of my peers");
			e.SelectPartyMember(target);
		}
		public void MonsterClicked(EncounterMonster m) {
//			Debug.Log("Clicked on a monster! time to get to work.");
//			p.TakeAction(p.InitiateAttack(m));
//			e.InstallListener(null);
		}
		public void Cancel() {
			e.InstallListener(prev);
		}
		public void InstallUI() {
			p.MarkSelected(true);
			partyMemberActions = new List<UIButton>();
			// Display the buttons for actions!
			//TODO this should go for each item in the inventory
			var Buttons = new List<GameObject>();
			for (int i = 0; i < p.p.inventory.Count; i += 1)
			{
				UIButton go = e.CreateUIButton();
				var weapon = p.p.inventory[i];
				go.SetImage(weapon.GetDef(e.session).image);
//				go.transform.position = p.transform.position + new Vector3(-1, -1f);
				Debug.Log("I is " + i);
				var listener = new AttackWithWeapon(p, i, this, e);
				go.SetOnClick(() => e.InstallListener(listener));
				partyMemberActions.Add(go);
				Buttons.Add(go.gameObject);
			}
			{
				UIButton go = e.CreateUIButton();
//				go.SetText("Swap");
				go.SetImage(e.swapSprite);
//				go.transform.position = p.transform.position + new Vector3(1, -1f);
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

	public class AttackWithWeapon : Encounter.ActionListener {
		private EncounterPartyMember p;
		private Encounter e;
		Encounter.ActionListener prev;
		int weapon;

		public AttackWithWeapon(EncounterPartyMember p, int w, Encounter.ActionListener previous, Encounter e) {
			this.p = p;
			this.e = e;
			this.prev = previous;
			this.weapon = w;
		}

		public void PartyMemberClicked(EncounterPartyMember target) {
			e.SelectPartyMember(target);
		}
		public void MonsterClicked(EncounterMonster m) {
			var w = p.p.inventory[weapon].GetDef(e.session);
			p.TakeAction(p.Attack(m, w.damage), () => m != null, w.readyTime);
			e.InstallListener(null);
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

	public void Setup(Encounter e, PartyMember p) {
		this.e = e;
		this.p = p;
		GetComponent<SpriteRenderer>().sprite = p.GetImage(e.session);
	}
		
	protected override int HP() {
		return 5;
	}

	public IEnumerator SwapWith(EncounterPartyMember p) {
		float dt = 0f;
		float duration = 1f;
		Vector3 p1 = transform.position;
		Vector3 p2 = p.transform.position;
		while (dt < duration ) {
			yield return null;

			// TODO make sure we're not paused here.
			dt += Time.deltaTime;
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
		e.ClickPartyMember(this);
	}
}
