using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Zone", menuName = "Zone", order = 1)]
public class Zone : ScriptableObject {
	public List<MonsterDefinition> monsterDefs;
	public string description;
}
