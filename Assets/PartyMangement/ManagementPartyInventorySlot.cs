using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ManagementPartyInventorySlot : MonoBehaviour {
	private UnityAction action;
	private SessionManager session;
	private PartyManagement management;
	public void Setup(SessionManager session, PartyManagement management) {
		this.session = session;
		this.management = management;
	}


	public void SetupAsGroupInventorySlot(int myIndex){
		if (session.state.inventory.Count <= myIndex) {
			GetComponent<SpriteRenderer>().color = Color.clear;
			return;
		}
		Item myItem = session.state.inventory[myIndex];
		GetComponent<SpriteRenderer>().color = Color.white;
		GetComponent<SpriteRenderer>().sprite = myItem.GetDef(session).image;
		action = SendToPartyMember(myIndex);
	}
	public void SetupAsPartyMemberInventorySlot(int partyIndex, int inventoryIndex){
		PartyMember pm = session.state.party[partyIndex];
		if (pm.inventory.Count <= inventoryIndex) {
			GetComponent<SpriteRenderer>().color = Color.clear;
			return;
		}
		Item myItem = pm.inventory[inventoryIndex];
		GetComponent<SpriteRenderer>().color = Color.white;
		GetComponent<SpriteRenderer>().sprite = myItem.GetDef(session).image;
		action = SendToParty(pm, inventoryIndex);
	}

	UnityAction SendToParty(PartyMember pm, int itemIndex) {
		return () => {
			var val = pm.inventory[itemIndex];
			pm.inventory.RemoveAt(itemIndex);
			session.state.inventory.Add(val);
			management.RefreshView();
		};
	}

	UnityAction SendToPartyMember(int itemIndex) {
		return () => {
			var pm = session.state.party[management.currentPartyMember];
			var val = session.state.inventory[itemIndex];
			session.state.inventory.RemoveAt(itemIndex);
			pm.inventory.Add(val);
			management.RefreshView();
		};
	}

	void OnMouseDown() {
		if (action != null) {
			action.Invoke();
		}
	}
}
