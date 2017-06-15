using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
public class ItemDefinition : ScriptableObject {
	public enum TargetMode {
		ENEMY = 0,
		FRIENDLY = 1,
	}

	public string itemName;
	public string description;
	public Sprite image;

	[Header("Mid-Encounter activation")]
	public bool usableInbattle = true;
	public TargetMode target;
	public float accuracy = 1f;
	public bool neverMiss;
	public bool armorPierce;
	public float readyTime;
	public GameObject itemActivationEffect;
	public ActivationEffect activationEffect;
	public int numberOfCharges = -1;

	[Header("Stats-related things")]
	public StatusModifier modifier;

	public string Tooltip() {
		return itemName + "\n" + description;
	}
}
