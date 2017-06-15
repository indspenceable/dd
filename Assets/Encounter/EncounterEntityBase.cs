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
	protected abstract int BaseHP();
	protected abstract List<Item> Items();
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
		foreach(var se in statusEffects) {
			if (se.myEffect.triggerMode == StatusEffect.TriggerMode.ROUNDS) {
				se.Trigger(this);
				Debug.Log("RT: " + se.remainingTriggers);
				Debug.Log(this.SpeedModifier());
			}
		}
		statusEffects.RemoveAll(se => se.remainingTriggers == 0);
	}

	public void ApplyActivationEffect(ActivationEffect ae, bool armorPierce) {
		if (ae.damage != 0) {
			TakeDamage(ae.damage, armorPierce);
		}
		if (ae.effect != null) {
			AddStatusEffect(ae.effect);
		}
	}

	public IEnumerator UseItem(EncounterEntityBase target, Item item) {
		ItemDefinition itemDefinition = item.GetDef(encounter.session);
		var ae = itemDefinition.activationEffect;
		if (itemDefinition.itemActivationEffect != null) {
			var effect = Instantiate(itemDefinition.itemActivationEffect, transform.position, Quaternion.identity, transform).GetComponent<ItemActivationEffect>();
			yield return effect.Activate(encounter, this, target);
			Destroy(effect.gameObject);
		}
		if (target == null ) yield break;
		if (itemDefinition.neverMiss || RollAccuracy(target.Evasion(), itemDefinition.accuracy)) {
			target.ApplyActivationEffect(ae, itemDefinition.armorPierce);
		} else {
			encounter.session.ui.BounceText("miss", Color.blue, target.transform.position);
		}
		if (itemDefinition.numberOfCharges != -1) {
			UseCharge(item);
		}
		yield return null;
	}
	private void UseCharge(Item i) {
		i.chargesUsed += 1;
		if (i.chargesUsed >= i.GetDef(encounter.session).numberOfCharges) {
			Items().Remove(i);
		}
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

	public bool StillHasItem(Item i ) {
		return Items().Contains(i);
	}

	private List<StatusModifier> AllModifiers() {
		List<StatusModifier> rtn = new List<StatusModifier>();
		foreach(var i in Items()) {
			rtn.Add(i.GetDef(encounter.session).modifier);
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

	private int TotalHP() {
		return Mathf.Max(BaseHP() + AllModifiers().Sum(modifier => modifier.hp), 1);
	}

	public void TakeDamage(int hitAmount, bool ignoreArmor) {
		int processedHitAmount = hitAmount;
		if (hitAmount >= 0) {
			int armor = ignoreArmor ? 0 : FullArmor();
			processedHitAmount = Mathf.Max(hitAmount - armor, 0);
		} else if (hitAmount < 0) {
			processedHitAmount = Mathf.Max(hitAmount, -Damage());
		} // if it's 0, then no amount of vulnerability will make it deal damage.
		Color c = hitAmount > 0 ? Color.red : (hitAmount < 0 ? Color.green : Color.grey);
		// TODO there is a race condition here. Id unno why it triggers on this line particularly though...
		if (gameObject != null ) {
			encounter.session.ui.BounceText("" + Mathf.Abs(processedHitAmount), c, transform.position);
			this.SetDamage(Damage() + processedHitAmount);
			if (this.Damage() >= TotalHP()) {
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
			healthBar.SetPct(1f - (Damage() / (float)TotalHP()));
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

	protected abstract string Tooltip();
	void OnMouseOver() {
		encounter.session.ui.ShowToolTip(Tooltip(), Input.mousePosition);
	}
}
