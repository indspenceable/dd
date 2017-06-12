using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotspot : MonoBehaviour {
	public void SetMonster(EncounterMonster m) {
	}
	public void SetPartyMember(EncounterPartyMember p) {
		p.hotspot = this;
	}
}
