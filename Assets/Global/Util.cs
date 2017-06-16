using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

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
	public static T Random<T>(List<T> list) {
		if (list == null ) {
			Debug.LogError("null List in Util.Random");
		}
		return list[Random.Range(0, list.Count)];
	}
	public static void Shuffle<T>(List<T> list) {
		for (int i = list.Count-1; i> 0; i-=1) {
			T val = list[i];
			int rand = Random.Range(0, i);
			list[i] = list[rand];
			list[rand] = val;
		}
	}

	// TODO replace this! For dev, shamelessly stolen from: 
	// https://scratch.mit.edu/discuss/topic/103440/?page=1#post-899736
	public static string GenerateName(int length)
	{
		List<string> consonants = new List<string>{"b","c","d","f","g","h","j","k","l","m","n","p","q","r","s","sh","z","zh",
			"t","v","w","x","y"};
		List<string> vowels = new List<string>{ "a", "e", "i", "o", "u"};
		StringBuilder name = new StringBuilder();
		bool Cons = (Random.Range(0,2)==0);
		int i = 0;
		while (i < length)
		{
			if(Cons)
			{
				name.Append(Random(consonants));
				Cons = false;
				i++;
			}
			else
			{
				name.Append(Random(vowels));
				Cons = true;
				i++;
			}
		}
		name[0] = name[0].ToString().ToUpper()[0];
		return name.ToString();
	}
}
