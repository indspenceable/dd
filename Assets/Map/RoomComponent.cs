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
		var room = manager.layout.rooms[index];
		if (room.state == RoomData.State.CLEARED) {
			sr.sprite = room.contents.ExploredSprite(manager.session);
		} else {
			sr.sprite = room.contents.UnexploredSprite(manager.session);
		}
	}

	public RoomData GetData() {
		return manager.layout.rooms[index];
	}

	void OnMouseDown() {
		this.manager.RoomClicked(this);
	}
}
