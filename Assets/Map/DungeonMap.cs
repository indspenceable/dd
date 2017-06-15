using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			if (r.GetData().state == RoomData.State.UNEXPLORED) {
				// Let's go to a battle!
				// dm.StartEncounter(r);
				// 
				// JK actually lets ask the room contents to do whatever they do
				dm.session.StartCoroutine(r.GetData().contents.Install(dm.session, r.index));
			} else {
				// TODO you should be able to go to some other rooms.
				// But for now, just take you to party management
				dm.session.SwapToManagement();
			}
		}
	}


	[SerializeField] GameObject roomPrefab;
	[SerializeField] GameObject hallwayPrefab;
	private EventListener el;
	SessionManager session;
	public Layout layout {
		get {
			return session.state.layout;
		}
	}
		
	public void RoomClicked(RoomComponent r){
		if (!session.ui.Blocking && CanReach(r.index))
			el.ClickOnRoom(r);
	}


	private bool Check(int index) {
		return layout.rooms[index].state == RoomData.State.CLEARED;
	}
	public bool CanReach(int index) {
		if (Check(index)) {
			return true;
		}
		foreach(var pair in layout.doors) {
			if ((pair.x == index && Check(pair.y)) ||
				(pair.y == index && Check(pair.x))) {
				return true;
			}
		}
		return false;
	}

	public void Setup(SessionManager session) {
		this.session = session;
		BuildGameObjects();
		this.el = new GoToBattle(this);
	}
		
//	private IEnumerator Build() {
//
//		BuildGameObjects();
//		this.el = new GoToBattle(this);
//	}

	private void BuildGameObjects() {
		for (int i = 0; i < layout.rooms.Count; i+=1) {
			if (CanReach(i)) {
				RoomComponent r = Instantiate(roomPrefab, layout.rooms[i].ToVec(), Quaternion.identity, transform).GetComponent<RoomComponent>();
				r.Setup(this, i);
			}
		}
		foreach(var i in layout.doors) {
			if (CanReach(i.x) || CanReach(i.y)) {
				var v1 = layout.rooms[i.x].ToVec()/2f;
				var v2 = layout.rooms[i.y].ToVec()/2f;
				Instantiate(hallwayPrefab, v1+v2, Quaternion.identity, transform);
			}
		}
		// Build some doors, Man!
	}
}
