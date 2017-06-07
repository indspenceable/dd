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
			UIButton go = e.CreateUIButton();
			go.SetText("Bow");
			go.transform.position = p.transform.position + new Vector3(0, -1f);
			go.SetOnClick(() => e.InstallListener(new Attack(p, this, e)));
			partyMemberActions.Add(go);

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
			p.TakeAction(p.InitiateAttack(m));
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

	[SerializeField] GameObject SelectedReticle;

	private Encounter e;
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
	public void TakeAction(IEnumerator action) {
		if (currentAction != null) {
			StopCoroutine(currentAction);
			CleanupLeasedObjects();
		}
		leasedObjects = new List<GameObject>();
		currentAction = StartCoroutine(action);
	}

	public IEnumerator InitiateAttack(Monster m) {
		UIProgressBar b = e.CreateProgressBar();
		leasedObjects.Add(b.gameObject);
		b.transform.position = transform.position + new Vector3(0f, 0.5f);
		float dt = 0f;
		float duration = 7f;
		while (dt < duration) {
			yield return null;
			// TODO make sure we check if we're paused here!
			// if our target disappears we should probably cancel this action.
			if (m == null ){
				CleanupLeasedObjects();
				yield break;
			}
			dt += Time.deltaTime;
			b.SetPct(dt/duration);
		}
		m.TakeDamage(3);
		CleanupLeasedObjects();
	}

	private void CleanupLeasedObjects() {
		foreach(var go in leasedObjects) {
			Destroy(go);
		}
		StopCoroutine(currentAction);
		currentAction = null;
	}
}
