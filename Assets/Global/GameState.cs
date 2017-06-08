using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomContents {
	public override bool Equals(System.Object obj) {
		PartyMember o = obj as PartyMember;
		if (o == null) return false;
		return true;
	}
}
[System.Serializable] 
public class Item {
	[System.Serializable]
	public struct Definition{
		public Sprite image;
	}
	public int index;
	public Item(int index) {
		this.index = index;
	}
	public Definition GetDef(SessionManager session) {
		return session.itemDefs[index];
	}
	public override bool Equals(System.Object obj) {
		Item o = obj as Item;
		if (o == null) return false;
		return o.index == index;
	}
}
[System.Serializable] 
public class PartyMember {
	public int image;
	public List<Item> inventory = new List<Item>();
	// TODO add skills here

	public Sprite GetImage(SessionManager session) {
		return session.partyImages[image];
	}
	public override bool Equals(System.Object obj) {
		PartyMember o = obj as PartyMember;
		if (o == null) return false;
		return Util.ListEquals(inventory, o.inventory) && image == o.image;
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
	public override bool Equals(System.Object obj) {
		RoomData o = obj as RoomData;
		if (o == null) return false;
		return pos.Equals(o.pos) && state == o.state && contents == o.contents;
	}
}
[System.Serializable]
public class Layout {
	public List<RoomData> rooms = new List<RoomData>();
	public List<Coord> doors = new List<Coord>();

	public override bool Equals(System.Object obj) {
		Layout o = obj as Layout;
		if (o == null) return false;
		return Util.ListEquals(rooms, o.rooms) && Util.ListEquals(doors, o.doors);
	}
}

[System.Serializable]
public class GameState {
	public Layout layout;
	public List<Item> inventory = new List<Item>();
	public List<PartyMember> party = new List<PartyMember>();
	public override bool Equals(System.Object obj) {
		GameState o = obj as GameState;
		if (o == null) return false;
		return layout.Equals(o.layout) && 
			Util.ListEquals(inventory, o.inventory) &&
			Util.ListEquals(party, o.party);
	}
}
public class Util {
	public static bool ListEquals<T>(List<T> first, List<T> second) {
		if (first == null && second == null) return true;
		if (first == null || second == null) return false;

		if (first.Count != second.Count) return false;
		for (int i = 0; i < first.Count; i += 1) {
			if (! first[i].Equals(second[i])) return false;
		}
		return true;
	}
}
