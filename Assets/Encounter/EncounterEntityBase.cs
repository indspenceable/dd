using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterEntityBase : MonoBehaviour {
	[SerializeField] GameObject SelectedReticle;
	protected Encounter encounter;
	protected PartyMember backingPartyMember;
	public Hotspot hotspot;
	[SerializeField]
	private int totalDamage = 0;
	protected abstract int HP();
	protected abstract int Armor();

	public void MarkSelected(bool amSelected) {
		this.SelectedReticle.SetActive(amSelected);
	}

	private List<GameObject> leasedObjects;
	protected Coroutine currentAction;
	public void TakeAction(IEnumerator action, ValidityCheck valid, float time) {
		if (currentAction != null) {
			StopCoroutine(currentAction);
			CleanupLeasedObjects();
		}
		leasedObjects = new List<GameObject>();
		currentAction = StartCoroutine(InitiateAction(time, action, valid));
	}
	public delegate bool ValidityCheck();
	public IEnumerator InitiateAction(float duration, IEnumerator action, ValidityCheck valid) {
		UIProgressBar b = encounter.CreateProgressBar(transform);
		leasedObjects.Add(b.gameObject);
		b.transform.position = transform.position + new Vector3(0f, 0.5f);
		float dt = 0f;
		while (dt < duration) {
			yield return null;
			// TODO if our target disappears we should probably cancel this action.
			dt += encounter.session.DT();
			b.SetPct(dt/duration);


			if (!valid()) {
				CleanupLeasedObjects();
				yield break;
			}
		}
		yield return null;
		CleanupLeasedObjects();

		StartCoroutine(action);
	}

	public IEnumerator Attack(EncounterEntityBase m, int damage) {
		// TODO animate this
		m.TakeDamage(damage);
		yield return null;
	}

	private void CleanupLeasedObjects() {
		foreach(var go in leasedObjects) {
			Destroy(go);
		}
		StopCoroutine(currentAction);
		currentAction = null;
	}

	protected abstract void Destroy();

	public void TakeDamage(int hitAmount) {
		int reducedhitAmount = Mathf.Max(hitAmount - Armor(), 0);
		this.totalDamage += reducedhitAmount;
		if (this.totalDamage >= HP()) {
			RemoveHealthBar();
			Destroy();
		}
	}

	private UIProgressBar healthBar;
	protected void Update() {
		if (totalDamage > 0) {
			if (healthBar == null ) {
				healthBar = encounter.CreateProgressBar(transform);
				healthBar.transform.position = transform.position + new Vector3(0,1);
			}
			healthBar.SetPct(1f - (totalDamage / (float)HP()));
		} else {
			RemoveHealthBar();
		}
	}

	private void RemoveHealthBar() {
		if (healthBar != null) {
			Destroy(healthBar.gameObject);
			healthBar = null;
		}
	}
}
