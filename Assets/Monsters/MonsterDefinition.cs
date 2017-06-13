using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster", order = 1)]
public class MonsterDefinition : ScriptableObject {
	public Sprite image;
	public List<ItemDefinition> weapons;
	public int hp;
	public int armor;
}

