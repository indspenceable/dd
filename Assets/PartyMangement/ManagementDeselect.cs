using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementDeselect : MonoBehaviour {
	[SerializeField] PartyManagement pm;
	void OnMouseDown() {
		pm.Deselect();
	}
}
