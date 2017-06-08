using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManagement : MonoBehaviour {
	private SessionManager session;
	[SerializeField] List<ManagementPartyMember> partyMembers;
	[SerializeField] List<ManagementPartyInventorySlot> leftSlots;
	[SerializeField] List<ManagementPartyInventorySlot>rightSlots;
	public void Setup(SessionManager session) {
		this.session = session;

		for (int i = 0; i < partyMembers.Count; i+=1) {
			partyMembers[i].Setup(session, this, i);
		}

		for (int i = 0; i < leftSlots.Count; i += 1) {
			leftSlots[i].Setup(session, this, i);
		}
		for (int i = 0; i < rightSlots.Count; i += 1) {
//			rightSlots[i].Setup(session, this, 999);
		}

		// TODO you should be able to drag items between inventory slots to move them all around

	}
	public void ClickPartyMember(int index) {
		rightSlots[1].gameObject.SetActive(true);
		rightSlots[1].gameObject.GetComponent<SpriteRenderer>().sprite = session.state.party[index].GetImage(session);
		for (int i = 3; i < rightSlots.Count; i += 1) {
			rightSlots[i].SetupAsPartyInventory(session, this, index, i-3);
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
