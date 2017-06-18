using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplay : MonoBehaviour {
	[SerializeField] Text text; 
	public void Set(int i ){
		text.text = "$" +i;
	}
}
