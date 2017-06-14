using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class EncounterEntityBase : MonoBehaviour {
	[SerializeField] GameObject SelectedReticle;
	protected Encounter encounter;
	protected PartyMember backingPartyMember;
	public Hotspot hotspot;
	protected List<StatusEffectInstance> statusEffects = new List<StatusEffectInstance>();

	protected abstract int Damage();
	protected abstract void SetDamage(int damage);
	protected abstract int HP();
	protected abstract List<ItemDefinition> Items();
	protected abstract int BaseArmor();
	protected abstract float Evasion();

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
			dt += encounter.session.DT() * (1/SpeedModifier());
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
		if (target == null ) yield break;
		if (RollAccuracy(target.Evasion(), item.accuracy)) {
			if (item.damage != 0) {
				target.TakeDamage(item.damage);
			}
			if (item.effect != null) {
				target.AddStatusEffect(item.effect);
			}
		} else {
			encounter.session.ui.BounceText("miss", Color.blue, target.transform.position);
		}
		yield return null;
	}
	protected bool RollAccuracy(float evasion, float itemAccuracy) {
		float roll=Random.Range(0f, 1f);
//		Debug.Log("Roll is " + roll);
//		Debug.Log("Evasion is " + evasion);
//		Debug.Log("itemAccuracy is " + itemAccuracy);
//		Debug.Log("LHS is " + roll);
//		Debug.Log("RHS is " + (1-((1-evasion)*itemAccuracy)));
//		Debug.Log("Result " + (roll > 1-((1-evasion)*itemAccuracy)));
		return roll > 1-((1-evasion)*itemAccuracy);
	}

	private void CleanupLeasedObjects() {
		foreach(var go in leasedObjects) {
			Destroy(go);
		}
		StopCoroutine(currentAction);
		currentAction = null;
	}

	protected abstract void Destroy();

	private List<StatusModifier> AllModifiers() {
		List<StatusModifier> rtn = new List<StatusModifier>();
		foreach(var i in Items()) {
			rtn.Add(i.modifier);
		}
		foreach(var e in statusEffects) {
			rtn.Add(e.myEffect.modifier);
		}
		return rtn;
	}

	private int FullArmor() {
		// TODO is this too inefficient?
		return BaseArmor() + AllModifiers().Sum(modifier => modifier.armor);
	}


	private float SpeedModifier() {
		float speed = 1f;
		foreach(var modifier in AllModifiers()) {
			speed *= modifier.speed;
		}
		return speed;
	}

	void AddStatusEffect(StatusEffect e) {
		statusEffects.Add(e.BuildInstance());
	}

	void TakeDamage(int hitAmount) {
		int processedHitAmount = hitAmount;
		if (hitAmount >= 0) {
			processedHitAmount = Mathf.Max(hitAmount - FullArmor(), 0);
		} else {
			processedHitAmount = Mathf.Max(hitAmount, -Damage());
		}
		Color c = hitAmount > 0 ? Color.red : hitAmount < 0 ? Color.green : Color.grey;
		// TODO there is a race condition here. Id unno why it triggers on this line particularly though...
		if (gameObject != null ) {
			encounter.session.ui.BounceText("" + Mathf.Abs(processedHitAmount), c, transform.position);
			this.SetDamage(Damage() + processedHitAmount);
			if (this.Damage() >= HP()) {
				RemoveHealthBar();
				Destroy();
			}
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
