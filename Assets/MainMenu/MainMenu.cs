using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {
	private SessionManager session;
	public void Setup(SessionManager session){
		this.session = session;
	}
	public void StartNewGame() {
		session.StartGame();
	}
	public void LoadSavedGame() {
		session.LoadSavedGame();
	}
}
