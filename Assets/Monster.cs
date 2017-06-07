using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {
	private Encounter e;
	[SerializeField] int hp;
	private int totalDamage = 0;
	public void SetEncounter(Encounter e) {
		this.e = e;
	}

	void OnMouseDown() {
		e.ClickMonster(this);
	}

	public void TakeDamage(int hitAmount) {
		this.totalDamage += hitAmount;
		if (this.totalDamage > hp) {
			RemoveHealthBar();
			Destroy(gameObject);
		}
	}

	private UIProgressBar healthBar;
	void Update() {
		if (totalDamage > 0) {
			if (healthBar == null ) {
				healthBar = e.CreateProgressBar(transform);
				healthBar.transform.position = transform.position + new Vector3(0,1);
			}
			healthBar.SetPct(1f - (totalDamage / (float)hp));
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
