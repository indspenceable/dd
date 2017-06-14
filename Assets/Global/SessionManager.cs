using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SessionManager : MonoBehaviour {
	[SerializeField] GameObject mapPrefab;
	[SerializeField] GameObject encounterPrefab;
	[SerializeField] GameObject partyManagementPrefab;
	[SerializeField] GameObject mainMenuPrefab;
	[SerializeField] GameObject shopPrefab;
	[SerializeField] public UIManager ui;
	[SerializeField] public List<Sprite> partyImages;
	[SerializeField] public List<ItemDefinition> itemDefs;
	[SerializeField] public List<MonsterDefinition> monsterDefs;
	[SerializeField] public List<PartyMember> startingParty;

	private GameObject currentMode;

	[HideInInspector]
	private GameState _state;
	public GameState state {
		get { return _state; }
	}

	private bool Paused = false;
	public void TogglePause() {
		Paused = !Paused;
		ui.SetPaused(Paused);
	}
	public void UnPause() {
		Paused = false;
		ui.SetPaused(Paused);
	}
	public float DT() {
		return Paused ? 0f : Time.deltaTime;
	}

	void Start() {
		SwapToMainMenu();
	}

	public void StartGame() {
		StartCoroutine(DoGameStart());
	}

	public IEnumerator DoGameStart() {
		KillCurrentMode();
		_state = new GameState();
		yield return BuildLayout();
		yield return BuildParty();
		SwapToManagement();
		StartCoroutine(CheckSaves());
	}

	public void LoadSavedGame() {
		var formatter = new BinaryFormatter();
		FileStream stream = File.Open(filePath(), FileMode.Open);
		_state = (GameState)formatter.Deserialize(stream);
		stream.Close();
		SwapToManagement();
		StartCoroutine(CheckSaves());
	}

	// TODO this needs to go before production! but for the time being, it's a nice
	// safety net :)
	private string filePath() {
		return Application.persistentDataPath + "/dd.gd";
	}
	private IEnumerator CheckSaves() {
		var formatter = new BinaryFormatter();
		while (true) {
			MemoryStream outputStream = new MemoryStream();
			formatter.Serialize(outputStream, state);
			var data = outputStream.ToArray();
			var inputStream = new MemoryStream(data, 0, data.Length);
			var newState = (GameState)formatter.Deserialize(inputStream);
			if (!state.Equals(newState)) {
				Debug.Log("They're not equal!");
			}
			FileStream stream = File.Create(filePath()); //you can call it anything you want
			formatter.Serialize(stream, state);
			stream.Close();
			yield return new WaitForSeconds(1f);
		}

	}

	private void KillCurrentMode() {
		if (currentMode != null) {
			Destroy(currentMode);
			currentMode = null;
		}
	}

	public void SwapToMapMode(){
		KillCurrentMode();
		DungeonMap dm = Instantiate(mapPrefab).GetComponent<DungeonMap>();
		dm.Setup(this);
		currentMode = dm.gameObject;
	}

	public void SwapToEncounter(RoomContents.Encounter ec, int index) {
		KillCurrentMode();
		Encounter e = Instantiate(encounterPrefab).GetComponent<Encounter>();
		e.Setup(this, index, ec);
		currentMode = e.gameObject;
	}

	public void SwapToManagement() {
		KillCurrentMode();
		Management pm = Instantiate(partyManagementPrefab).GetComponent<Management>();
		pm.Setup(this);
		currentMode = pm.gameObject;
	}

	public void SwapToMainMenu() {
		KillCurrentMode();
		MainMenu mm = Instantiate(mainMenuPrefab).GetComponent<MainMenu>();
		mm.Setup(this);
		currentMode = mm.gameObject;
	}

	public void SwapToShopMode(List<Item> items) {
		KillCurrentMode();
		ManagementShop mm = Instantiate(shopPrefab).GetComponent<ManagementShop>();
		mm.Setup(this, items);
		currentMode = mm.gameObject;
	}

	private IEnumerator BuildLayout() {
		Layout layout = new Layout();
		layout.rooms.Add(new RoomData(new Coord(5,5)));
		layout.rooms[0].state = RoomData.State.CLEARED;
		layout.rooms[0].contents = new RoomContents.Empty();
		yield return null;
		int requiredRoomCount = 37;
		while (layout.rooms.Count < requiredRoomCount) {
			Coord pos = new Coord(Random.Range(0, 10), Random.Range(0, 10));
			var adjacentRooms = layout.rooms.FindAll(r => r.pos.DistanceTo(pos) == 1);
			var existant = layout.rooms.Find(r => r.pos.Equals(pos));
			if (adjacentRooms.Count > 0 && existant == null) {
				List<int> newConnections = new List<int>();
				while (newConnections.Count == 0) {
					for (int i = 0; i < adjacentRooms.Count; i+=1) {
						if (Random.Range(0f,1f) > 0.7f) {
							newConnections.Add(layout.rooms.IndexOf(adjacentRooms[i]));
						}
					}
				}
				foreach(var c in newConnections) {
					layout.doors.Add(new Coord(c, layout.rooms.Count));
				}
				layout.rooms.Add(BuildRoomData(pos));
//				Debug.Log("" + layout.rooms.Count*100f / requiredRoomCount + "% (" + layout.rooms.Count + "/" + requiredRoomCount + ")");
			}
		}
		state.layout = layout;
	}

	RoomData BuildRoomData(Coord pos) {
		var rd = new RoomData(pos);
		var contents = new List<RoomContents>();
		{
			var ec = new RoomContents.Encounter();
			ec.monsters = new List<int>();
			for (int i = Random.Range(1, 6); i > 0; i-=1) {
				ec.monsters.Add(Random.Range(0, monsterDefs.Count));
				contents.Add(ec);
			}
		}
		{
			var tr = new RoomContents.Treasure();
			contents.Add(tr);
		}
		{
			contents.Add(new RoomContents.Empty());
		}
		{
			contents.Add(new RoomContents.NewPartyMember());
		}
		{
			contents.Add(new RoomContents.Shop(this));
		}

		rd.contents = Util.Random(contents);
		return rd;
	}

	public IEnumerator BuildParty() {
		yield return null;
		state.party = new List<PartyMember>(startingParty);
	}

	public PartyMember RANDOM_PARTY_MEMBER___() {
		var partyMember = new PartyMember();
		partyMember.hp = 20;
		partyMember.damage = 0;
		partyMember.pcName = Util.GenerateName(Random.Range(5, 13));
		partyMember.image = Random.Range(0, partyImages.Count);
		int k = 1;
		for (int j = 0; j < k; j += 1) {
			partyMember.inventory.Add(RANDOM_ITEM___());
		}
		return partyMember;
	}

	public Item RANDOM_ITEM___() {
		return new Item(Random.Range(0, itemDefs.Count));
	}
}
