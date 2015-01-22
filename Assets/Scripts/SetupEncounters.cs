using UnityEngine;
using System.Collections;

public class SetupEncounters : MonoBehaviour 
{
	#region components
	public Transform _transform;
	public GameData _gameData;
	public GenerateGameData _generateGameData;
	#endregion components

	public void setupEncounters
		(
			int id
			,string name
			,int typeid
			,string coroutine
			,object[] coroutineParams
			)
	{
		Encounter newEncounter = gameObject.AddComponent<Encounter>();
		
		newEncounter.id = id;
		newEncounter.encounterName = name;
		newEncounter.typeid = typeid;
		newEncounter.coroutine = coroutine;
		newEncounter.coroutineParams = coroutineParams;
		
		_gameData.encounters.Add (id,newEncounter);
	}
	
	public void createEncounters()
	{
		object[] coroutineParams = new object[1];
		coroutineParams[0] = 8;
		
		setupEncounters
			(
				0
				,"Battle with orcs"
				,0
				,"runBattle"
				,coroutineParams
				);
		
		coroutineParams = new object[1];
		coroutineParams[0] = 9;
		
		setupEncounters
			(
				1
				,"Battle with ogres"
				,0
				,"runBattle"
				,coroutineParams
				);
		
		coroutineParams = new object[1];
		coroutineParams[0] = 10;
		
		setupEncounters
			(
				2
				,"Battle with wolves"
				,0
				,"runBattle"
				,coroutineParams
				);
		
		coroutineParams = new object[1];
		coroutineParams[0] = 2f;
		
		setupEncounters
			(
				3
				,"Find small town"
				,2
				,"wait"
				,coroutineParams
				);
		
		coroutineParams = new object[1];
		coroutineParams[0] = 2f;
		
		setupEncounters
			(
				4
				,"Find large city"
				,2
				,"fullRest"
				,coroutineParams
				);
		
		coroutineParams = new object[1];
		coroutineParams[0] = 2f;
		
		setupEncounters
			(
				5
				,"Find massive ruins"
				,2
				,"shortRest"
				,coroutineParams
				);
		
		coroutineParams = new object[1];
		coroutineParams[0] = 2f;
		
		setupEncounters
			(
				6
				,"Meet thief in road"
				,1
				,"wait"
				,coroutineParams
				);
		
		coroutineParams = new object[1];
		coroutineParams[0] = 11;
		
		setupEncounters
			(
				7
				,"Battle with goblins"
				,0
				,"runBattle"
				,coroutineParams
				);
	}
}
