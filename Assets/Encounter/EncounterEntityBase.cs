using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EncounterEntityBase : MonoBehaviour {
	[SerializeField] GameObject SelectedReticle;
	protected Encounter encounter;
	protected PartyMember backingPartyMember;
	public Hotspot hotspot;
	protected abstract int Damage();
	protected abstract void SetDamage(int damage);
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

	public IEnumerator UseItem(EncounterEntityBase target, ItemDefinition item) {
		// TODO animate this
		if (item.itemActivationEffect != null) {
			var effect = Instantiate(item.itemActivationEffect, transform.position, Quaternion.identity, transform).GetComponent<ItemActivationEffect>();
			yield return effect.Activate(encounter, this, target);
			Destroy(effect.gameObject);
		}
		target.TakeDamage(item.damage);
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
		int processedHitAmount = hitAmount;
		if (hitAmount >= 0) {
			processedHitAmount = Mathf.Max(hitAmount - Armor(), 0);
		} else {
			processedHitAmount = Mathf.Max(hitAmount, -Damage());
		}
		Color c = hitAmount > 0 ? Color.red : hitAmount < 0 ? Color.green : Color.grey;
		encounter.session.ui.BounceText("" + Mathf.Abs(processedHitAmount), c, transform.position);
		this.SetDamage(Damage() + processedHitAmount);
		if (this.Damage() >= HP()) {
			RemoveHealthBar();
			Destroy();
		}
	}

	private UIProgressBar healthBar;
	protected void Update() {
		if (Damage() > 0) {
			if (healthBar == null ) {
				healthBar = encounter.CreateProgressBar(transform);
				healthBar.transform.position = transform.position + new Vector3(0,1);
			}
			healthBar.SetPct(1f - (Damage() / (float)HP()));
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
