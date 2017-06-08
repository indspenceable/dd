﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Coord : System.IEquatable<Coord> {
	public int x;
	public int y;
	public Coord(int x, int y) {
		this.x = x;
		this.y = y;
	}
	public Coord plus(int x, int y) {
		return new Coord(this.x + x, this.y + y);
	}
	public static Coord operator + (Coord a, Coord b) {
		return new Coord(a.x + b.x, a.y + b.y);
	}
	public static Coord operator - (Coord a, Coord b) {
		return new Coord(a.x - b.x, a.y - b.y);
	}
	public static Coord operator * (Coord a, int b) {
		return new Coord(a.x * b, a.y * b);
	}
	public Vector3 ToVec() {
		return new Vector3(this.x, this.y);
	}
	public int DistanceTo(Coord other) {
		return Mathf.Abs(this.x - other.x) + Mathf.Abs(this.y - other.y);
	}
	public bool Equals(Coord other) {
		return this.x == other.x && this.y == other.y;
	}
	public override string ToString() {
		return "Coord<" + x + ", " + y + ">";
	}
}