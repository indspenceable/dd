using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomComponent : MonoBehaviour {
	private SpriteRenderer sr;
	private DungeonMap manager;
	public int index;

	public void Setup(DungeonMap manager, int index) {
		this.manager = manager;
		this.index = index;
		this.sr = GetComponent<SpriteRenderer>();
		Update();
	}
	void Update() {
		if (manager != null) {
			SetState(manager.layout.rooms[index].state);
		}
	}
	public RoomData GetData() {
		return manager.layout.rooms[index];
	}

	public void SetState(RoomData.State s) {
		if (s == RoomData.State.UNEXPLORED) {
			if (manager.layout.rooms[index].contents as RoomContents.Shop != null) {
				sr.color = Color.blue;
			} else {
				sr.color = Color.red;
			}
		} else if (s == RoomData.State.IN_PROGRESS) {
			sr.color = Color.magenta;
		} else {
			sr.color = Color.green;
		}
	}

	void OnMouseDown() {
		this.manager.RoomClicked(this);
	}
}
