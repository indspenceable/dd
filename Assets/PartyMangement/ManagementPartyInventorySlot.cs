using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ManagementPartyInventorySlot : MonoBehaviour {
	private UnityAction action;
	private SessionManager session;
	private PartyManagement management;
	private string tt_string;
	public void Setup(SessionManager session, PartyManagement management) {
		this.session = session;
		this.management = management;
		this.tt_string = null;
	}


	public void SetupAsGroupInventorySlot(int myIndex){
		if (session.state.inventory.Count <= myIndex) {
			GetComponent<Image>().enabled = false;
			tt_string = null;
			return;
		}
		Item myItem = session.state.inventory[myIndex];
		GetComponent<Image>().enabled = true;
		GetComponent<Image>().sprite = myItem.GetDef(session).image;
		tt_string = myItem.GetDef(session).Tooltip();
		action = SendToPartyMember(myIndex);
	}
	public void SetupAsPartyMemberInventorySlot(int partyIndex, int inventoryIndex){
		PartyMember pm = session.state.party[partyIndex];
		if (pm.inventory.Count <= inventoryIndex) {
			GetComponent<Image>().enabled = false;
			tt_string = null;
			return;
		}
		Item myItem = pm.inventory[inventoryIndex];
		GetComponent<Image>().enabled = true;
		GetComponent<Image>().sprite = myItem.GetDef(session).image;
		tt_string = myItem.GetDef(session).Tooltip();
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

	public void OnMouseDown(){
		if (action != null) {
			action.Invoke();
		}
	}
//
//
	public void OnMouseOver(){
		if (tt_string != null) {
			session.ui.ShowToolTip(tt_string, Input.mousePosition);
		}
	}
}
