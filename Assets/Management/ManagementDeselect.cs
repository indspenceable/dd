using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagementDeselect : MonoBehaviour {
	[SerializeField] Management pm;
	void OnMouseDown() {
		pm.Deselect();
	}
}
