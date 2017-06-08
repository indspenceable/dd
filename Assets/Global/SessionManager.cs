using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SessionManager : MonoBehaviour {
	[SerializeField] GameObject mapPrefab;
	[SerializeField] GameObject encounterPrefab;
	private GameObject currentMode;
	public GameState state;

	IEnumerator Start() {
		yield return BuildLayout();
		SwapToMapMode();
		StartCoroutine(CheckSaves());
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
				Debug.Log("They're not equal!");
			} else {
				Debug.Log("All g!");
			}
			yield return new WaitForSeconds(0.75f);
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

	public void SwapToEncounter(int index) {
		KillCurrentMode();
		Encounter e = Instantiate(encounterPrefab).GetComponent<Encounter>();
		e.Setup(this, index);
		currentMode = e.gameObject;
	}

	private IEnumerator BuildLayout() {
		Layout layout = new Layout();
		layout.rooms.Add(new RoomData(new Coord(5,5)));
		layout.rooms[0].state = RoomData.State.CLEARED;
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
				layout.rooms.Add(new RoomData(pos));
//				Debug.Log("" + layout.rooms.Count*100f / requiredRoomCount + "% (" + layout.rooms.Count + "/" + requiredRoomCount + ")");
			}
		}
		state.layout = layout;
	}
}
