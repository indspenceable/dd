using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class RoomContents {
	public abstract IEnumerator Install(SessionManager session, int roomIndex);
	public virtual Sprite ExploredSprite(SessionManager session) {
		return null;
	}
	public virtual Sprite UnexploredSprite(SessionManager session) {
		return session.roomIcons.Unexplored;
	}

	[System.Serializable]
	public class Encounter : RoomContents {
		public List<int> monsters;
		public override bool Equals(System.Object obj) {
			Encounter o = obj as Encounter;
			if (o == null) return false;
			return true;
		}
		public override IEnumerator Install(SessionManager session, int roomIndex) {
			yield return session.ui.TextBox("Monsters! Get ready for a fight.");
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
			session.GetRoom(roomIndex).Clear(true);
			yield return session.ui.TextBox("You found an item! : " + item.GetDef(session).itemName);
			session.SwapToMapMode();
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
			if (session.GetRoom(roomIndex).state == RoomData.State.UNEXPLORED) {
				session.GetRoom(roomIndex).Clear(true);
				yield return session.ui.TextBox("Empty room. Carry on.");
			}
			session.SwapToMapMode();
		}
	}

	[System.Serializable]
	public class NewPartyMember : RoomContents {
		public override bool Equals(System.Object obj) {
			NewPartyMember o = obj as NewPartyMember;
			if (o == null) return false;
			return true;
		}
		public override IEnumerator Install(SessionManager session, int roomIndex) {
			if (session.state.party.Count >= 5) {
				yield return session.ui.TextBox("You found a new party member! Unfortunately, your party is full.");
			} else {
				var partyMember = session.RANDOM_PARTY_MEMBER___();
				session.state.party.Add(partyMember);
				yield return session.ui.TextBox("You found a new party member! Welcome, " + partyMember.pcName + "!");
			}
			session.state.layout.rooms[roomIndex].Clear(true);
			session.SwapToMapMode();
		}
	}

	[System.Serializable]
	public class Shop : RoomContents {
		private List<Item> items = new List<Item>();
		public Shop(SessionManager session) {
			int itemCount = Random.Range(2, 6);
			for (int i = 0; i < itemCount; i+=1) {
				items.Add(session.RANDOM_ITEM___());
			}
		}
		public override bool Equals(System.Object obj) {
			Shop o = obj as Shop;
			if (o == null) return false;
			return Util.ListEquals(items, o.items);
		}
		public override IEnumerator Install(SessionManager session, int roomIndex) {
			yield return null;
			session.state.layout.rooms[roomIndex].Clear(false);
			session.SwapToShopMode(items);
		}
		public override Sprite ExploredSprite(SessionManager session)
		{
			return session.roomIcons.ShopIcon;
		}
		public override Sprite UnexploredSprite(SessionManager session)
		{
			return session.roomIcons.ShopIcon;
		}
	}

	[System.Serializable]
	public class NextFloor : RoomContents {
		public override bool Equals(System.Object obj) {
			NextFloor o = obj as NextFloor;
			if (o == null) return false;
			return true;
		}
		public override IEnumerator Install(SessionManager session, int roomIndex) {
			if (session.GetRoom(roomIndex).state == RoomData.State.UNEXPLORED) {
				yield return session.ui.TextBox("You found a wormhole to a new floor!\n\nRevisist this location to travel through it.");
				session.GetRoom(roomIndex).Clear(false);
			} else {
				yield return session.ui.TextBox("You travel through the wormhole and find yourself on a new floor of the dungeon...");
				yield return session.BuildAndAddLayout();
			}
			session.SwapToMapMode();
		}
		public override Sprite ExploredSprite(SessionManager session)
		{
			return session.roomIcons.NextFloorIcon;
		}
	}
}

[System.Serializable] 
public class Item {
	public int index;
	public int chargesUsed;
	public Item(int index) {
		this.index = index;
	}
	public ItemDefinition GetDef(SessionManager session) {
		return session.itemDefs[index];
	}
	public override bool Equals(System.Object obj) {
		Item o = obj as Item;
		if (o == null) return false;
		return o.index == index && chargesUsed == o.chargesUsed;
	}
}
[System.Serializable] 
public class PartyMember {
	public string pcName;
	public int image;
	public int hp;
	public int damage;
	public List<Item> inventory = new List<Item>();
	// TODO add skills here

	public Sprite GetImage(SessionManager session) {
		return session.partyImages[image];
	}
	public override bool Equals(System.Object obj) {
		PartyMember o = obj as PartyMember;
		if (o == null) return false;
		return Util.ListEquals(inventory, o.inventory) && 
			image == o.image &&
			hp == o.hp &&
			damage == o.damage &&
			pcName == o.pcName;
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
	public void Clear(bool emptyContents) {
		this.state = State.CLEARED;
		if (emptyContents) {
			this.contents = new RoomContents.Empty();
		}
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
	public Layout layout {
		get {
			return floors[floors.Count -1];
		}
	}
	public List<Layout> floors = new List<Layout>();
	public List<Item> inventory = new List<Item>();
	public List<PartyMember> party = new List<PartyMember>();
	public List<PartyMember> graveyard = new List<PartyMember>();
	public int money;
	public override bool Equals(System.Object obj) {
		GameState o = obj as GameState;
		if (o == null) return false;
		return layout.Equals(o.layout) && 
			Util.ListEquals(inventory, o.inventory) &&
			Util.ListEquals(party, o.party) &&
			Util.ListEquals(graveyard, o.graveyard) &&
			money == o.money;
	}
}