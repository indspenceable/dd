using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
public class ItemDefinition : ScriptableObject {
	public enum TargetMode {
		ENEMY = 0,
		FRIENDLY = 1,
//		SELF = 2,>.
	}

	public string itemName;
	public string description;
	public Sprite image;
	public int damage;
	public float readyTime;
	public TargetMode target;
	public GameObject itemActivationEffect;
	public float accuracy = 1f;

	public string Tooltip() {
		return itemName + "\n" + description;
	}
}
