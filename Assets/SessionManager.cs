using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
}
[System.Serializable]
public class Link {
	public Link(int first, int second) {
		this.source = first;
		this.dest = second;
	}
	public int source;
	public int dest;
}
[System.Serializable]
public class Layout {
	public List<DungeonRoom> rooms = new List<DungeonRoom>();
	public List<Link> doors = new List<Link>();
}
public class SessionManager : MonoBehaviour {
	[SerializeField] GameObject mapPrefab;
	[SerializeField] GameObject encounterPrefab;

	public Layout layout;

	private GameObject currentMode;

	IEnumerator Start() {
		yield return BuildLayout();
		SwapToMapMode();
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
		layout = new Layout();
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
	}
}
