using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActivationEffect : MonoBehaviour {
	[SerializeField] float duration;
	public IEnumerator Activate(Encounter encounter, EncounterEntityBase source, EncounterEntityBase target) {
		Vector3 sourcePos = source.transform.position;
		float dt = 0f;
		while (dt < duration) {
			yield return null;
			// TODO if our target disappears we should probably cancel this action.
			dt += encounter.session.DT();
			if (target == null ) {
				yield break;
			}
			transform.position = Vector3.Lerp(sourcePos, target.transform.position, dt/duration);
		}
	}
}
