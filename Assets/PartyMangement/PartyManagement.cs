using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManagement : MonoBehaviour {
	private SessionManager session;
	[SerializeField] List<ManagementPartyMember> partyMembers;
	[SerializeField] List<ManagementPartyInventorySlot> leftSlots;
	[SerializeField] List<ManagementPartyInventorySlot>rightSlots;

	[System.NonSerialized]
	public int currentPartyMember = -1;
	public void Setup(SessionManager session) {
		this.session = session;
		foreach(var l in leftSlots) {
			l.Setup(session, this);
		}
		foreach(var r in rightSlots) {
			r.Setup(session, this);
		}
		Deselect();
		RefreshView();
	}

	public void RefreshView() {
		for (int i = 0; i < partyMembers.Count; i+=1) {
			partyMembers[i].Setup(session, this, i);
		}
		RefreshGroupInventorySlots();
		if (currentPartyMember > -1) {
			RefreshPMInventorySlots();
		} else {
			Deselect();
		}
	}

	public void RefreshGroupInventorySlots() {
		for (int i = 0; i < leftSlots.Count; i += 1) {
			leftSlots[i].SetupAsGroupInventorySlot(i);
		}
	}

	public void ClickPartyMember(int index) {
		currentPartyMember = index;
		RefreshPMInventorySlots();
	}
	public void RefreshPMInventorySlots() {
		rightSlots[1].gameObject.SetActive(true);
		rightSlots[1].gameObject.GetComponent<SpriteRenderer>().sprite = session.state.party[currentPartyMember].GetImage(session);
		for (int i = 3; i < rightSlots.Count; i += 1) {
			rightSlots[i].SetupAsPartyMemberInventorySlot(currentPartyMember, i-3);
			rightSlots[i].gameObject.SetActive(true);
		}
	}
	public void Deselect() {
		for (int i = 0; i < rightSlots.Count; i += 1) {
			rightSlots[i].gameObject.SetActive(false);
		}
	}
	public void ReturnToMap() {
		session.SwapToMapMode();
	}
}
