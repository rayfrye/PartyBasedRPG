using UnityEngine;
using System.Collections;

public class SetupQuests : MonoBehaviour 
{
	#region components
	public Transform _transform;
	public GameData _gameData;
	public GenerateGameData _generateGameData;
	#endregion components

	public void setupQuest
	(
		int id
		,string name
	)
	{
		Quest newQuest = gameObject.AddComponent<Quest>();
		
		newQuest.id = id;
		newQuest.questName = name;
		
		_gameData.quests.Add (id,newQuest);		
	}
	
	public void createQuests()
	{
		setupQuest
		(
			0
			,"Quest 1"
		);
	}
}
