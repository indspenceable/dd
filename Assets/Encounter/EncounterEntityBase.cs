using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterEntityBase : MonoBehaviour {
	[SerializeField] GameObject SelectedReticle;
	protected Encounter e;
	protected PartyMember p;
	public Hotspot hotspot;
	private int totalDamage = 0;
	protected abstract int HP();

	public void MarkSelected(bool amSelected) {
		this.SelectedReticle.SetActive(amSelected);
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
			dt += Time.deltaTime;
			b.SetPct(dt/duration);
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
		this.totalDamage += hitAmount;
		if (this.totalDamage >= HP()) {
			RemoveHealthBar();
			Destroy();
		}
	}

	private UIProgressBar healthBar;
	void Update() {
		if (totalDamage > 0) {
			if (healthBar == null ) {
				healthBar = e.CreateProgressBar(transform);
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
