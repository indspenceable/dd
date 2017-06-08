using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


[System.Serializable]
public class DungeonRoom {
	public DungeonRoom(int x, int y) {
		this.x = x;
		this.y = y;
		this.state = RoomComponent.State.UNEXPLORED;
	}
	public int x;
	public int y;
	public RoomComponent.State state;
	public Vector3 ToVec() {
		return new Vector3(x-4.5f, y-4.5f);
	}
	public bool Equals(DungeonRoom o) {
		return x == o.x  && y==o.y && state == o.state;
	}
}
[System.Serializable]
public class Link {
	public Link(int first, int second) {
		this.source = first;
		this.dest = second;
	}
	public int source;
	public int dest;
	public bool Equals(Link o) {
		return this.source == o.source && this.dest == o.dest;
	}
}
[System.Serializable]
public class Layout {
	public List<DungeonRoom> rooms = new List<DungeonRoom>();
	public List<Link> doors = new List<Link>();

	public bool Equals(Layout o) {
		if (rooms.Count != o.rooms.Count ||
			doors.Count != o.doors.Count) {
			return false;
		}
		for (int i = 0; i < rooms.Count; i += 1) {
			if (! rooms[i].Equals(o.rooms[i])) return false;
		}
		for (int i = 0; i < doors.Count; i += 1) {
			if (! doors[i].Equals(o.doors[i])) return false;
		}
		return true;
	}
}

[System.Serializable]
public class SerializableGameState {
	public Layout layout;

	public bool Equals(SerializableGameState o) {
		return layout.Equals(o.layout);
	}
}
public class SessionManager : MonoBehaviour {
	[SerializeField] GameObject mapPrefab;
	[SerializeField] GameObject encounterPrefab;
	private GameObject currentMode;
	public SerializableGameState state;

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
			var newState = (SerializableGameState)formatter.Deserialize(inputStream);
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

	public void SwapToEncounter() {
		KillCurrentMode();
		Encounter e = Instantiate(encounterPrefab).GetComponent<Encounter>();
		e.Setup(this);
		currentMode = e.gameObject;
	}

	private IEnumerator BuildLayout() {
		Layout layout = new Layout();
		layout.rooms.Add(new DungeonRoom(5,5));
		layout.rooms[0].state = RoomComponent.State.CLEARED;
		yield return null;
		int requiredRoomCount = 37;
		while (layout.rooms.Count < requiredRoomCount) {
			int x = Random.Range(0, 10);
			int y = Random.Range(0, 10);
			var adjacentRooms = layout.rooms.FindAll(r => {
				return (Mathf.Abs(r.x - x) + Mathf.Abs(r.y-y) == 1) &&
					((r.x == x) || (r.y == y));
			});
			var existant = layout.rooms.Find((r) => r.x == x && r.y == y);
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
					layout.doors.Add(new Link(c, layout.rooms.Count));
				}
				layout.rooms.Add(new DungeonRoom(x,y));
				Debug.Log("" + layout.rooms.Count*100f / requiredRoomCount + "% (" + layout.rooms.Count + "/" + requiredRoomCount + ")");
			}
//			yield return null;
		}
		state.layout = layout;
	}
}
