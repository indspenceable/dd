using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "Monster", order = 1)]
public class MonsterDefinition : ScriptableObject {
	public string monsterName;
	public Sprite image;
	public List<ItemDefinition> items;
	public int hp;
	public int armor;
	public float evasion;
	public int difficulty;
}

