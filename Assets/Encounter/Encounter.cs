using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour {
	private List<Hotspot> allHotspots;
	[SerializeField] List<Hotspot> monsterHotspots;
	[SerializeField] List<Hotspot> playerHotspots;
	[SerializeField] GameObject monsterPrefab;
	[SerializeField] GameObject partyMemberPrefab;
	private List<EncounterMonster> monsters = new List<EncounterMonster>();
	private List<EncounterPartyMember> party = new List<EncounterPartyMember>();

	[SerializeField] GameObject progressBarPrefab;
	[SerializeField] GameObject uiButtonPrefab;
	[SerializeField] GameObject WorldCanvas;
	[SerializeField] public Sprite swapSprite;

	[HideInInspector]
	public SessionManager session;
	private int roomIndex;

	public void Setup(SessionManager session, int roomIndex, RoomContents.Encounter ec) {
		this.session = session;
		this.roomIndex = roomIndex;
		allHotspots = new List<Hotspot>();
		allHotspots.AddRange(monsterHotspots);
		allHotspots.AddRange(playerHotspots);


		for (int i = 0; i < ec.monsters.Count; i+=1){
			BuildMonster(monsterHotspots[i], session.monsterDefs[ec.monsters[i]]);
			i+= 1;
		}
		for (int i = 0; i < session.state.party.Count; i+=1) {
			BuildPartyMember(playerHotspots[i], session.state.party[i]);
		}

		// Install the default action listener!
		this.al = new EncounterEventListener(this);
		this.al.InstallUI();
	}

	public interface ActionListener {
		void PartyMemberClicked(EncounterPartyMember p);
		void MonsterClicked(EncounterMonster m);
		void Cancel();
		void InstallUI();
		void UninstallUI();
	}
	class NoOp : ActionListener {
		public void PartyMemberClicked(EncounterPartyMember p){}
		public void MonsterClicked(EncounterMonster m){}
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
		public void MonsterClicked(EncounterMonster m){
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

	public void DestroyMonster(EncounterMonster m) {
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

	public void ClickMonster(EncounterMonster m) {
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

	private void BuildMonster(Hotspot hotspot, MonsterDefinition def) {
		EncounterMonster m = Instantiate(monsterPrefab, hotspot.transform.position, Quaternion.identity, transform).GetComponent<EncounterMonster>();
		hotspot.SetMonster(m);
		m.Setup(this, def);
		monsters.Add(m);
	}

	private void BuildPartyMember(Hotspot hotspot, PartyMember pm) {
		EncounterPartyMember p = Instantiate(partyMemberPrefab, hotspot.transform.position, Quaternion.identity, transform).GetComponent<EncounterPartyMember>();
		hotspot.SetPartyMember(p);
		p.Setup(this, pm);
		party.Add(p);
	}

	public EncounterPartyMember GetRandomPartyMember() {
		return party[Random.Range(0, party.Count)];
	}
}
