using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SessionManager : MonoBehaviour {
	[Header("Prefabs")]
	[SerializeField] GameObject mapPrefab;
	[SerializeField] GameObject encounterPrefab;
	[SerializeField] GameObject partyManagementPrefab;
	[SerializeField] GameObject mainMenuPrefab;
	[SerializeField] GameObject shopPrefab;
	[Header("Heirarchy GameObjects")]
	[SerializeField] public UIManager ui;
	[Header("Content Data")]
	[SerializeField] public List<Sprite> partyImages;
	[SerializeField] public List<ItemDefinition> itemDefs;
	[SerializeField] public List<MonsterDefinition> monsterDefs;
	[SerializeField] public List<PartyMember> startingParty;

	[System.Serializable]
	public struct RoomIcons {
		public Sprite Unexplored;
//		public Sprite Empty;
		public Sprite ShopIcon;
		public Sprite NextFloorIcon;
		public Sprite AltarIcon;
	}
	[Header("Map Icons")]
	public RoomIcons roomIcons;

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
		return (Paused||ui.Blocking) ? 0f : Time.deltaTime;
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
		_state.money = 1000;
		yield return BuildAndAddLayout();
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

	private string filePath() {
		return Application.persistentDataPath + "/dd.gd";
	}

	// TODO this needs to go before production! but for the time being, it's a nice
	// safety net :)

	private IEnumerator CheckSaves() {
		var formatter = new BinaryFormatter();
		while (true) {
			MemoryStream outputStream = new MemoryStream();
			formatter.Serialize(outputStream, state);
			var data = outputStream.ToArray();
			var inputStream = new MemoryStream(data, 0, data.Length);
			var newState = (GameState)formatter.Deserialize(inputStream);
			if (!state.Equals(newState)) {
				Debug.LogError("They're not equal!");
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

	void Update() {
		if (state != null ) {
			ui.SetActive(true);
			ui.SetMoney(state.money);
		} else {
			ui.SetActive(false);
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

	public RoomData GetRoom(int roomIndex) {
		return state.layout.rooms[roomIndex];
	}

	public IEnumerator BuildAndAddLayout() {
		Layout layout = new Layout();
		layout.rooms.Add(new RoomData(new Coord(5,5)));
		layout.rooms[0].Clear(true);
		yield return null;
		int requiredRoomCount = 37;

		int difficulty = (state.floors.Count * 7) + 5;
		List<RoomContents> contents = BuildRoomContents(requiredRoomCount, difficulty);


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
				RoomData roomData = new RoomData(pos);
				roomData.contents = contents[contents.Count-1];
				contents.RemoveAt(contents.Count-1);
				layout.rooms.Add(roomData);
			}
		}
		state.floors.Add(layout);
	}

	int RandomMonsterDefByDifficulty(int desiredDifficulty) {
		int currentDifficulty = desiredDifficulty;
		while (currentDifficulty>0) {
			List<MonsterDefinition> defs = monsterDefs.FindAll(m => m.difficulty == currentDifficulty);
			if (defs.Count > 0) {
				Util.Shuffle(defs);
				return monsterDefs.IndexOf(defs[0]);
			}
			currentDifficulty-=1;
		}
		Debug.LogError("Unable to find a monster with difficulty of " +desiredDifficulty);
		return 0;
	}

	RoomContents.Encounter BuildEncounter(int desiredDifficulty)  {
		var ec = new RoomContents.Encounter();
		ec.monsters = new List<int>();
		int NumberOfMonsters = Random.Range(1,6);
		int remainingDifficulty;
		while (2 * NumberOfMonsters > desiredDifficulty) {
			NumberOfMonsters -= 1;
		}
		if (NumberOfMonsters == 0) {
			Debug.LogError("Trying to build an encounter with zero monsters.");
		}
		remainingDifficulty = desiredDifficulty - NumberOfMonsters;
		int[] monsterDifficulty = new int[NumberOfMonsters];

		for (int i = 0; i < NumberOfMonsters; i += 1) {
			monsterDifficulty[i] = ((remainingDifficulty / NumberOfMonsters) + Random.Range(-2,3));
			if (monsterDifficulty[i] < 1) {
				monsterDifficulty[i] = 1;
			}
		}
		string str = "";
		int sum = 0;
		foreach (var i in monsterDifficulty) {
			str = str + " " + i;
			sum += i;
		}

//		Debug.Log("Made an encounter with " + NumberOfMonsters + "     " + str);
//		Debug.Log("Total difficulty: " + (NumberOfMonsters + sum));

		for (int i = 0; i < NumberOfMonsters; i += 1) {
			ec.monsters.Add(RandomMonsterDefByDifficulty(monsterDifficulty[i]));
		}

		return ec;

	}

	List<RoomContents> BuildRoomContents(int desiredCount, int difficulty) {
		var rtn = new List<RoomContents>();
		for (int i = 0; i < 1; i += 1) {
			rtn.Add(new RoomContents.NextFloor());
		}
		for (int i = 0; i < 3; i += 1) {
			rtn.Add(new RoomContents.Shop(this));
		}
		for (int i = 0; i < 5; i += 1) {
			rtn.Add(new RoomContents.Treasure());
		}
		for (int i = 0; i < 2; i += 1) {
			rtn.Add(new RoomContents.NewPartyMember());
		}
		for (int i = 0; i < 3; i += 1) {
		rtn.Add(new RoomContents.MinorBlessing(RANDOM_BLESSING___()));
		}
		for (int i = 0; i < 18; i += 1) {
			rtn.Add(BuildEncounter(difficulty));
		}
		while (rtn.Count<desiredCount) {
			rtn.Add(new RoomContents.Empty());
		}

		Util.Shuffle(rtn);
		return rtn;
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
		int image = Random.Range(0, partyImages.Count);
		while (state.party.Find(pm => pm.image == image) != null) {
			image = Random.Range(0, partyImages.Count);
		}
		partyMember.image = image;
		int k = 1;
		for (int j = 0; j < k; j += 1) {
			partyMember.inventory.Add(RANDOM_ITEM___());
		}
		return partyMember;
	}

	public Item RANDOM_ITEM___() {
		var def = Util.Random(itemDefs.FindAll(id => id.isLoot));
		return new Item(itemDefs.IndexOf(def));
	}

	public Item RANDOM_BLESSING___() {
		var def = Util.Random(itemDefs.FindAll(id => id.isBlessing));
		return new Item(itemDefs.IndexOf(def));
	}
}
