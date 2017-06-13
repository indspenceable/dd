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
				r.GetData().contents.Install(dm.session, r.index);
			} else {
				// TODO you should be able to go to some other rooms.
				// But for now, just take you to party management
				dm.session.SwapToManagement();
			}
		}
	}


	[SerializeField] GameObject roomPrefab;
	private EventListener el;
	SessionManager session;
	public Layout layout {
		get {
			return session.state.layout;
		}
	}
		
	public void RoomClicked(RoomComponent r){
		if (!session.ui.Blocking)
			el.ClickOnRoom(r);
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
			RoomComponent r = Instantiate(roomPrefab, layout.rooms[i].ToVec(), Quaternion.identity, transform).GetComponent<RoomComponent>();
			r.Setup(this, i);
		}
		// Build some doors, Man!
	}
}
