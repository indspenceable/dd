using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyMember : MonoBehaviour {
	public class Selected : Encounter.ActionListener {
		private PartyMember p;
		private Encounter e;
		Encounter.ActionListener prev;
		private List<UIButton> partyMemberActions;

		public Selected(PartyMember p, Encounter.ActionListener previous, Encounter e) {
			this.p = p;
			this.e = e;
			this.prev = previous;
		}

		public void PartyMemberClicked(PartyMember target) {
			Debug.Log("Clicked on another of my peers");
			e.SelectPartyMember(target);
		}
		public void MonsterClicked(Monster m) {
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
			{
				UIButton go = e.CreateUIButton();
				go.SetText("Bow");
				go.transform.position = p.transform.position + new Vector3(-1, -1f);
				go.SetOnClick(() => e.InstallListener(new Attack(p, this, e)));
				partyMemberActions.Add(go);
			}
			{
				UIButton go = e.CreateUIButton();
				go.SetText("Swap");
				go.transform.position = p.transform.position + new Vector3(1, -1f);
				go.SetOnClick(() => e.InstallListener(new Swap(p, this, e)));
				partyMemberActions.Add(go);
			}
		}
		public void UninstallUI() {
			p.MarkSelected(false);
			foreach(var bu in partyMemberActions) {
				Destroy(bu.gameObject);
			}
		}
	}

	public class Attack : Encounter.ActionListener {
		private PartyMember p;
		private Encounter e;
		Encounter.ActionListener prev;

		public Attack(PartyMember p, Encounter.ActionListener previous, Encounter e) {
			this.p = p;
			this.e = e;
			this.prev = previous;
		}

		public void PartyMemberClicked(PartyMember target) {
			Debug.Log("Clicked on another of my peers");
			e.SelectPartyMember(target);
		}
		public void MonsterClicked(Monster m) {
			Debug.Log("Clicked on a monster! time to get to work.");
			p.TakeAction(p.InitiateAttack(m), 7f);
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
		private PartyMember p;
		private Encounter e;
		Encounter.ActionListener prev;

		public Swap(PartyMember p, Encounter.ActionListener previous, Encounter e) {
			this.p = p;
			this.e = e;
			this.prev = previous;
		}

		public void PartyMemberClicked(PartyMember target) {
			p.TakeAction(p.SwapWith(target), 3f);
			e.InstallListener(null);
		}
		public void MonsterClicked(Monster m) {
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

	[SerializeField] GameObject SelectedReticle;

	private Encounter e;
	public Hotspot hotspot;
	public void SetEncounter(Encounter e) {
		this.e = e;
	}
	public void MarkSelected(bool amSelected) {
		this.SelectedReticle.SetActive(amSelected);
	}

	// TODO this should probably be something that tracks which mousebutton was used ETC.
	void OnMouseDown() {
		e.ClickPartyMember(this);
	}

	private List<GameObject> leasedObjects;
	private Coroutine currentAction;
	public void TakeAction(IEnumerator action, float time) {
		if (currentAction != null) {
			StopCoroutine(currentAction);
			CleanupLeasedObjects();
		}
		leasedObjects = new List<GameObject>();
		currentAction = StartCoroutine(InitiateAction(time, action));
	}

	public IEnumerator InitiateAction(float duration, IEnumerator action) {
		UIProgressBar b = e.CreateProgressBar(transform);
		leasedObjects.Add(b.gameObject);
		b.transform.position = transform.position + new Vector3(0f, 0.5f);
		float dt = 0f;
		while (dt < duration) {
			yield return null;
			// TODO make sure we check if we're paused here!
			// TODO if our target disappears we should probably cancel this action.

			// now that it's generalized this doesn't work!
			//			if (m == null ){
			//				CleanupLeasedObjects();
			//				yield break;
			//			}
			dt += Time.deltaTime;
			b.SetPct(dt/duration);
		}
		CleanupLeasedObjects();

		StartCoroutine(action);
	}

	public IEnumerator InitiateAttack(Monster m) {
		// TODO animate this
		m.TakeDamage(3);
		yield return null;
	}

	public IEnumerator SwapWith(PartyMember p) {
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

	private void CleanupLeasedObjects() {
		foreach(var go in leasedObjects) {
			Destroy(go);
		}
		StopCoroutine(currentAction);
		currentAction = null;
	}
}
