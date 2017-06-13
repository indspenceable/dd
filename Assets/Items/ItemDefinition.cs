﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 1)]
public class ItemDefinition : ScriptableObject {
	public string itemName;
	public string description;
	public Sprite image;
	public int damage;
	public float readyTime;

	public string Tooltip() {
		return itemName + "\n" + description;
	}
}
