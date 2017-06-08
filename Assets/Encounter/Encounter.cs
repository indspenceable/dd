﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour {
	private List<Hotspot> allHotspots;
	[SerializeField] List<Hotspot> monsterHotspots;
	[SerializeField] List<Hotspot> playerHotspots;
	[SerializeField] GameObject monsterPrefab;
	[SerializeField] GameObject partyMemberPrefab;
	private List<Monster> monsters = new List<Monster>();
	private List<EncounterPartyMember> party = new List<EncounterPartyMember>();

	[SerializeField] GameObject progressBarPrefab;
	[SerializeField] GameObject uiButtonPrefab;
	[SerializeField] GameObject WorldCanvas;

	[SerializeField] List<Weapon> weapons;
	[SerializeField] SessionManager session;
	private int roomIndex;

	public void Setup(SessionManager session, int roomIndex) {
		this.session = session;
		this.roomIndex = roomIndex;
		allHotspots = new List<Hotspot>();
		allHotspots.AddRange(monsterHotspots);
		allHotspots.AddRange(playerHotspots);

		// Set up the players + monsters
		foreach(var hotspot in monsterHotspots) {
//			BuildMonster(hotspot);
		}
		BuildMonster(monsterHotspots[0]);
//		foreach(var hotspot in playerHotspots) {
//			BuildPartyMember(hotspot);
//		}
		for (int i = 0; i < session.state.party.Count; i+=1) {
			BuildPartyMember(playerHotspots[i], session.partyImages[session.state.party[i].image]);
		}

		// Install the default action listener!
		this.al = new EncounterEventListener(this);
		this.al.InstallUI();
	}

	public interface ActionListener {
		void PartyMemberClicked(EncounterPartyMember p);
		void MonsterClicked(Monster m);
		void Cancel();
		void InstallUI();
		void UninstallUI();
	}
	class NoOp : ActionListener {
		public void PartyMemberClicked(EncounterPartyMember p){}
		public void MonsterClicked(Monster m){}
		public void Cancel(){}
		public void InstallUI(){}
		public void UninstallUI(){}
	}
	private ActionListener al = null;
	class EncounterEventListener : ActionListener {
		private Encounter e;
		public EncounterEventListener(Encounter e) {
			this.e = e;
		}
		public void PartyMemberClicked(EncounterPartyMember p) {
			e.SelectPartyMember(p);
		}
		public void MonsterClicked(Monster m){
			// Nothing!
		}
		public void Cancel() {
			// Nothing!
		}
		public void InstallUI(){}
		public void UninstallUI(){}
	}

	public void InstallListener(ActionListener al) {
		if (this.al == al) {
			return;
		}
		this.al.UninstallUI();
		if (al == null) {
			this.al = new EncounterEventListener(this);
		} else {
			this.al = al;
		}
		this.al.InstallUI();
	}

	public void DestroyMonster(Monster m) {
		monsters.Remove(m);
		Destroy(m.gameObject);
		if (monsters.Count == 0) {
			InstallListener(new NoOp());
			StartCoroutine(GoToMapScreen());
		}
	}

	private IEnumerator GoToMapScreen(){
		yield return null;
		session.state.layout.rooms[roomIndex].state = RoomData.State.CLEARED;
		session.SwapToMapMode();
	}

	public void ClickPartyMember(EncounterPartyMember p) {
		al.PartyMemberClicked(p);
	}

	public void ClickMonster(Monster m) {
		al.MonsterClicked(m);
	}

	public void SelectPartyMember(EncounterPartyMember p) {
		Debug.Log("Selecting a party member!");
		InstallListener(new EncounterPartyMember.Selected(p, null, this));
	}

	public UIProgressBar CreateProgressBar(Transform t) {
		return Instantiate(progressBarPrefab, t).GetComponent<UIProgressBar>();
	}

	public UIButton CreateUIButton() {
		return Instantiate(uiButtonPrefab, WorldCanvas.transform).GetComponent<UIButton>();
	}

	private void BuildMonster(Hotspot hotspot) {
		Monster m = Instantiate(monsterPrefab, hotspot.transform.position, Quaternion.identity, transform).GetComponent<Monster>();
		hotspot.SetMonster(m);
		m.SetEncounter(this);
		monsters.Add(m);
	}

	private void BuildPartyMember(Hotspot hotspot, Sprite s) {
		EncounterPartyMember p = Instantiate(partyMemberPrefab, hotspot.transform.position, Quaternion.identity, transform).GetComponent<EncounterPartyMember>();
		hotspot.SetPartyMember(p);
		p.Setup(this, s, weapons[Random.Range(0, weapons.Count)]);
		party.Add(p);
	}


}
