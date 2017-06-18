using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectIndicator : MonoBehaviour {
	[HideInInspector]
	public List<StatusEffectInstance> statuses = new List<StatusEffectInstance>();
	private List<StatusEffectIcon> icons = new List<StatusEffectIcon>();
	[SerializeField]
	GameObject iconPrefab;
	public void Update() {
		while (icons.Count > statuses.Count) {
			Destroy(icons[icons.Count-1].gameObject);
			icons.RemoveAt(icons.Count-1);
		}
		while (icons.Count < statuses.Count) {
			icons.Add(Instantiate(iconPrefab, transform).GetComponent<StatusEffectIcon>());
		}
		for (int i = 0; i < statuses.Count; i += 1) {
			icons[i].Set(statuses[i]);
		}
	}
}
