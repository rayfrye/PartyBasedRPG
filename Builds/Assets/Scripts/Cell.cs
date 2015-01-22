using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Cell : MonoBehaviour 
{
	public int row;
	public int col;
	public int factionNum;
	public int spawnNum;
	public bool isInvalidSpace;
	public bool hasLord;
	public int lordID;
	public List<GameObject> path;
	
	public int pathScore;
	public int neighborPathScore;

	public int attackSpaceScore;
	public int neighborAttackSpaceScore;

	public int attackPathSpaceScore;
	public int neighborAttackPathSpaceScore;

	public int distanceToEnemy;
}
