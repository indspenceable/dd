using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementPartyInventorySlot : MonoBehaviour {
	public void Setup(SessionManager session, PartyManagement management, int myIndex){
		if (session.state.inventory.Count <= myIndex) {
			GetComponent<SpriteRenderer>().color = Color.magenta;
			return;
		}
		Item myItem = session.state.inventory[myIndex];
		GetComponent<SpriteRenderer>().sprite = myItem.GetDef(session).image;
	}
	public void SetupAsPartyInventory(SessionManager session, PartyManagement management, int partyIndex, int inventoryIndex){
		PartyMember pm = session.state.party[partyIndex];
		if (pm.inventory.Count <= inventoryIndex) {
			GetComponent<SpriteRenderer>().color = Color.magenta;
			return;
		}
		Item myItem = pm.inventory[inventoryIndex];
		GetComponent<SpriteRenderer>().sprite = myItem.GetDef(session).image;
	}
}
