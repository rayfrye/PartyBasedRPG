using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoBehaviour 
{
	#region components
	public Transform _transform;
	public GenerateGameData _generateGameData;
	#endregion components

	#region encounterTypes
	public Dictionary<int,EncounterType> encounterTypes = new Dictionary<int,EncounterType>();
	#endregion encounterTypes
	
	#region encounters
	public Dictionary<int,Encounter> encounters = new Dictionary<int,Encounter>();
	#endregion encounters
	
	#region quests
	public Dictionary<int,Quest> quests = new Dictionary<int,Quest>();
	#endregion quests
	
	#region lordclass
	public Dictionary<int, LordClass> lordClasses = new Dictionary<int, LordClass>();
	#endregion lordclass
	
	#region lord
	public Dictionary<int,Lord> lords = new Dictionary<int,Lord>();
	#endregion lord
	
	#region factions
	public Dictionary<int,Faction> factions = new Dictionary<int,Faction>();
	#endregion factions

	#region dialogues
	public Dictionary<int,Dialogue> dialogues = new Dictionary<int,Dialogue>();
	#endregion dialogues

	#region tiles
	public List<GameObject> tiles = new List<GameObject>();
	#endregion tiles

}
