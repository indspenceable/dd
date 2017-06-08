using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomContents {
	public bool Equals(RoomContents o) {
		return o!=null;
	}
}

[System.Serializable]
public class RoomData {
	public enum State {
		UNEXPLORED,
		IN_PROGRESS,
		CLEARED,
	}
	public RoomData(Coord pos) {
		this.pos = pos;
		this.state = RoomData.State.UNEXPLORED;
	}
	public Coord pos;
	public RoomData.State state;
	public RoomContents contents;
	public Vector3 ToVec() {
		return pos.ToVec() - new Vector3(4.5f, 4.5f);
	}
	public bool Equals(RoomData o) {
		return pos.Equals(o.pos) && state == o.state && contents == o.contents;
	}
}
[System.Serializable]
public class Layout {
	public List<RoomData> rooms = new List<RoomData>();
	public List<Coord> doors = new List<Coord>();

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
public class GameState {
	public Layout layout;

	public bool Equals(GameState o) {
		return layout.Equals(o.layout);
	}
}
