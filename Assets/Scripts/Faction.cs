using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Faction : MonoBehaviour 
{
	public int id;
	public string factionName;
	public List<int> lords = new List<int>();
	public bool isPlayerControlled;
	public int[] positions = new int[6];
	public List<int> bench = new List<int>();
	public int frontLineLords = 3;
	public int backLineLords = 3;
	public int[] inventory;
	public int gold;
}
