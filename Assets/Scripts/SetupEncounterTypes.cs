using UnityEngine;
using System.Collections;

public class SetupEncounterTypes : MonoBehaviour 
{
	#region components
	public Transform _transform;
	public GameData _gameData;
	public GenerateGameData _generateGameData;
	#endregion components
	
	public void setupEncounterTypes
		(
			int id
			,string name
			)
	{
		EncounterType newEncounterType = gameObject.AddComponent<EncounterType>();
		
		newEncounterType.id = id;
		newEncounterType.encountertypeName = name;
		
		_gameData.encounterTypes.Add(id,newEncounterType);
	}
	
	public void createEncounterTypes()
	{
		object[] waitTimeParameter = new object[1];
		waitTimeParameter[0] = 2f;
		
		object[] smallHealParameter = new object[1];
		smallHealParameter[0] = 10;
		
		setupEncounterTypes
			(
				0
				,"Battle"
				);
		
		setupEncounterTypes
			(
				1
				,"Encounter Person"
				);
		
		setupEncounterTypes
			(
				2
				,"Find new area"
				);
	}
}
