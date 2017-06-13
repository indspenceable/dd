using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class RoomContents {
	public abstract IEnumerator Install(SessionManager session, int roomIndex);
	[System.Serializable]
	public class Encounter : RoomContents {
		public List<int> monsters;
		public override bool Equals(System.Object obj) {
			Encounter o = obj as Encounter;
			if (o == null) return false;
			return true;
		}
		public override IEnumerator Install(SessionManager session, int roomIndex) {
			yield return session.ui.textBox.CoroutineShow("Monsters! Get ready for a fight.");
			session.SwapToEncounter(this, roomIndex);
		}
	}

	[System.Serializable]
	public class Treasure : RoomContents {
		public override bool Equals(System.Object obj) {
			Treasure o = obj as Treasure;
			if (o == null) return false;
			return true;
		}
		public override IEnumerator Install(SessionManager session, int roomIndex) {
			var item = session.RANDOM_ITEM___();
			session.state.inventory.Add(item);
			session.state.layout.rooms[roomIndex].state = RoomData.State.CLEARED;
			yield return session.ui.textBox.CoroutineShow("You found an item! : " + item.GetDef(session).itemName);
//			session.SwapToMapMode();
		}
	}

	[System.Serializable]
	public class Empty : RoomContents {
		public override bool Equals(System.Object obj) {
			Empty o = obj as Empty;
			if (o == null) return false;
			return true;
		}
		public override IEnumerator Install(SessionManager session, int roomIndex) {
			session.state.layout.rooms[roomIndex].state = RoomData.State.CLEARED;
			yield return session.ui.textBox.CoroutineShow("You found an empty room. Oh well!");
//			session.SwapToMapMode();
		}
	}
}

[System.Serializable] 
public class Item {
	public int index;
	public Item(int index) {
		this.index = index;
	}
	public ItemDefinition GetDef(SessionManager session) {
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
	public string pcName;
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
		// TODO this should be cleaned up after the roomcontents part gets more fleshed out
		return pos.Equals(o.pos) && state == o.state && (contents == o.contents || o.contents.Equals(contents));
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