using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementPartyMember : MonoBehaviour {
	private SessionManager session;
	private PartyManagement management;
	private int myIndex;

	public void Setup(SessionManager session, PartyManagement management, int myIndex){
		this.session = session;
		this.management = management;
		this.myIndex = myIndex;
		if (session.state.party.Count <= myIndex) {
			GetComponent<SpriteRenderer>().color = Color.magenta;
			return;
		}
		PartyMember pm = session.state.party[myIndex];
		GetComponent<SpriteRenderer>().sprite = pm.GetImage(session);
	}
	void OnMouseDown() {
		if (session.state.party.Count > myIndex){
			management.ClickPartyMember(myIndex);
		}
	}
}
