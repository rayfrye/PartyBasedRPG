using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SetupFactions : MonoBehaviour 
{
	#region components
	public Transform _transform;
	public GameData _gameData;
	public GenerateGameData _generateGameData;
	#endregion components

	public Dictionary<int,Lord> availableLords = new Dictionary<int,Lord>();
	public List<string> factionNames = new List<string>();
	public int numOfLordsToDraft;

	public void setupFactions
		(
			int id
			,string name
			,bool isPlayerControlled
			,List<int> draftLords
			,int[] positions
			,List<int> benchPositions
			)
	{
		Faction newFaction = gameObject.AddComponent<Faction>();
		
		newFaction.id = id;
		newFaction.factionName = name;
		newFaction.lords = draftLords;
		newFaction.isPlayerControlled = isPlayerControlled;
		newFaction.positions = positions;
		newFaction.bench = benchPositions;
		
		_gameData.factions.Add (id,newFaction);		
	}
	
	public void createFactions()
	{
		for(int i = 0; i < factionNames.Count; i++)
		{
			string factionName = factionNames[i];
			
			List<int> draftedLords = draftLords(numOfLordsToDraft, i);
			int[] positions = assignLordPositions(draftedLords,i,3,3);
			List<int> benchPositions = assignBenchPositions(draftedLords,positions);
			
			foreach(int draftedLord in draftedLords)
			{
				_gameData.lords[draftedLord].factionID = i;
			}
			
			setupFactions
				(
					//id
					i
					//name
					,factionName
					//isPlayerControlled
					,false
					//drafted lords
					,draftedLords
					//positions
					,positions
					//bench positions
					,benchPositions
					);
		}
		customFactions();
	}
	
	public void customFactions()
	{
		int factionID = _gameData.factions.Count;
		string factionName = "test";
		factionNames.Add (factionName);
		List<int> draftedLords = draftLords(3, factionID);
		int[] positions = assignLordPositions(draftedLords,factionID,1,2);
		List<int> benchPositions = assignBenchPositions(draftedLords,positions);
		
		foreach(int draftedLord in draftedLords)
		{
			_gameData.lords[draftedLord].factionID = factionID;
		}
		
		setupFactions
		(
			//id
			factionID
			//name
			,factionName
			//isPlayerControlled
			,false
			//drafted lords
			,draftedLords
			//positions
			,positions
			//bench positions
			,benchPositions
		);
	}
	
	public List<int> draftLords(int i, int factionID)
	{
		List<int> l = new List<int>();
		
		for(int p = 0; p < i; p++)
		{
			int lordID = UnityEngine.Random.Range (0,availableLords.Count);
			l.Add(availableLords[lordID].id);
			availableLords = removeDraftedLords(availableLords,availableLords[lordID].id);
		}
		
		return l;
	}
	
	int[] assignLordPositions(List<int> draftedLords, int factionID, int numFrontLine, int numBackLine)
	{
		int[] m = new int[numFrontLine+numBackLine];
		
		List<int> availablelordIDs = new List<int>();
		
		foreach(int i in draftedLords)
		{
			availablelordIDs.Add(i);
		}
		
		for(int n = 0; n < numFrontLine; n++)
		{
			int tempvalue = -1;
			int tempid = -1;
			
			foreach(int i in availablelordIDs)
			{
				if(_gameData.lords[i].maxHealth > tempvalue)
				{
					tempid = i;
					tempvalue = _gameData.lords[i].defense;
				}
			}
			m[n] = tempid;
			availablelordIDs.Remove (tempid);
		}
		
		for(int n = numFrontLine; n < numFrontLine+numBackLine; n++)
		{
			int tempvalue = -1;
			int tempid = -1;
			
			foreach(int i in availablelordIDs)
			{
				if(_gameData.lords[i].attack > tempvalue)
				{
					tempid = i;
					tempvalue = _gameData.lords[i].attack;
				}
			}
			m[n] = tempid;
			availablelordIDs.Remove (tempid);
		}

		return m;
	}
	
	public List<int> assignBenchPositions(List<int> draftedLords, int[] positions)
	{
		List<int> l = new List<int>();
		int[] draftedArray = draftedLords.ToArray();
		List<int> positionList = positions.ToList();
		
		foreach(int i in draftedArray)
		{
			if(!positionList.Contains (i))
			{
				l.Add (i);
			}
		}
		return l;
	}

	public Dictionary<int, Lord> removeDraftedLords(Dictionary<int, Lord> r, int id)
	{
		Dictionary<int, Lord> t = new Dictionary<int, Lord>();
		
		List<Lord> l = r.Values.ToList ();

		l.Remove (_gameData.lords[id]);
		
		for(int i = 0; i < l.Count; i++)
		{
			t.Add (i,l[i]);
		}
		
		return t;
	}
	
	public void generateFactionNames()
	{
		factionNames.Add ("Team 0");
		factionNames.Add ("Team 1");
		factionNames.Add ("Team 3");
		factionNames.Add ("Team 4");
		factionNames.Add ("Team 5");
		factionNames.Add ("Team 6");
		factionNames.Add ("Team 7");
	}

}
