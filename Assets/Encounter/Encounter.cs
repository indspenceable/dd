using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour {
	private List<Hotspot> allHotspots;
	[SerializeField] List<Hotspot> monsterHotspots;
	[SerializeField] List<Hotspot> playerHotspots;
	[SerializeField] GameObject monsterPrefab;
	[SerializeField] GameObject partyMemberPrefab;
	private List<Monster> monsters = new List<Monster>();
	private List<PartyMember> party = new List<PartyMember>();

	[SerializeField] GameObject progressBarPrefab;
	[SerializeField] GameObject uiButtonPrefab;
	[SerializeField] GameObject WorldCanvas;

	[SerializeField] List<Weapon> weapons;
	[SerializeField] GameManager gm;

	public void Start() {
//		this.gm = gm;
		allHotspots = new List<Hotspot>();
		allHotspots.AddRange(monsterHotspots);
		allHotspots.AddRange(playerHotspots);


		// Set up the players + monsters
		foreach(var hotspot in monsterHotspots) {
			BuildMonster(hotspot);
		}
		foreach(var hotspot in playerHotspots) {
			BuildPartyMember(hotspot);
		}

		// Install the default action listener!
		this.al = new EncounterEventListener(this);
		this.al.InstallUI();
	}

	public interface ActionListener {
		void PartyMemberClicked(PartyMember p);
		void MonsterClicked(Monster m);
		void Cancel();
		void InstallUI();
		void UninstallUI();
	}
	class NoOp : ActionListener {
		public void PartyMemberClicked(PartyMember p){}
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
		public void PartyMemberClicked(PartyMember p) {
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
		// TODO visual effects go here.
		yield return null;
//		gm.EndEncounter(this);
	}

	public void ClickPartyMember(PartyMember p) {
		al.PartyMemberClicked(p);
	}

	public void ClickMonster(Monster m) {
		al.MonsterClicked(m);
	}

	public void SelectPartyMember(PartyMember p) {
		Debug.Log("Selecting a party member!");
		InstallListener(new PartyMember.Selected(p, null, this));
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

	private void BuildPartyMember(Hotspot hotspot) {
		PartyMember p = Instantiate(partyMemberPrefab, hotspot.transform.position, Quaternion.identity, transform).GetComponent<PartyMember>();
		hotspot.SetPartyMember(p);
		p.SetupEncounter(this, weapons[Random.Range(0, weapons.Count)]);
		party.Add(p);
	}


}
