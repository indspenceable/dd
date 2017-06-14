using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BouncingText : MonoBehaviour {
	[SerializeField] Text text;
	[SerializeField] AnimationCurve bounce;
	[SerializeField] float duration;
	[SerializeField] float amplification;

	public void Setup(string text, Color c) {
		this.text.text = text;
		this.text.color = c;
		StartCoroutine(Bounce());
	}

	private IEnumerator Bounce() {
		Vector3 origin = transform.position;
		float dt = 0f;
		while (dt < duration) {
			yield return null;
			dt += Time.deltaTime;
			float y = bounce.Evaluate(dt/duration)*amplification;
			transform.position = origin + Vector3.up*y;
		}
		Destroy(gameObject);
	}
}
