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

public class DungeonMap : MonoBehaviour {
	public interface EventListener {
		void ClickOnRoom(RoomComponent r);
	}
	public class NoOp : EventListener{
		public void ClickOnRoom(RoomComponent r){}
	}
	public class GoToBattle: EventListener {
		DungeonMap dm;
		public GoToBattle(DungeonMap dm) {
			this.dm = dm;
		}
		public void ClickOnRoom(RoomComponent r) {
			if (r.GetState() == RoomComponent.State.UNEXPLORED) {
				// Let's go to a battle!
//				dm.StartEncounter(r);
			} else {
				// TODO you should be able to go to some other rooms.
			}
		}
	}

	public Layout layout;
	[SerializeField] GameObject roomPrefab;
	private EventListener el;

	// Use this for initialization
	void Start () {
		this.el = new NoOp();
		StartCoroutine(Build());
	}
		
	public void RoomClicked(RoomComponent r){
		el.ClickOnRoom(r);
	}

	// Update is called once per frame
	void Update () {
	}

	public void OnDrawGizmos() {
		if (layout == null || layout.rooms.Count == 0) return;

		Gizmos.color = Color.white;
		foreach(var r in layout.rooms) {
			Gizmos.DrawWireCube(r.ToVec(), Vector3.one);
		}
		var rz = layout.rooms[0];
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(layout.rooms[0].ToVec(), Vector3.one);
		Gizmos.color = Color.yellow;
		foreach(var c in layout.doors) {
			var r = layout.rooms[c.source];
			var r2 = layout.rooms[c.dest];
			Gizmos.DrawLine(r.ToVec(), r2.ToVec());
		}
	}

	private IEnumerator Build() {
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
			yield return null;
		}
		BuildGameObjects();
		this.el = new GoToBattle(this);
	}

	private void BuildGameObjects() {
		for (int i = 0; i < layout.rooms.Count; i+=1) {
			RoomComponent r = Instantiate(roomPrefab, layout.rooms[i].ToVec(), Quaternion.identity, transform).GetComponent<RoomComponent>();
			r.Setup(this, i);
		}
	}
}
