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
	}
	void Start() {
		sr = GetComponent<SpriteRenderer>();
	}
	void Update() {
		if (manager != null) {
			SetState(manager.layout.rooms[index].state);
		}
	}
	public RoomData.State GetState() {
		return manager.layout.rooms[index].state;
	}
	public void SetState(RoomData.State s) {
		if (s == RoomData.State.UNEXPLORED) {
			sr.color = Color.red;
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
