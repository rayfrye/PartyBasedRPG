using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class GenerateGameData : MonoBehaviour 
{
	Transform _transform;
	public SetupGrid _setupGrid;
	public GameData _gameData;
	public SetupLords _setupLords;
	public SetupLordClasses _setupLordClasses;
	public SetupEncounterTypes _setupEncounterTypes;
	public SetupEncounters _setupEncounters;
	public SetupFactions _setupFactions;
	public SetupQuests _setupQuests;
	public ReadCSV _readCSV;
	public SetupLevel _setupLevel;
	public RunBattle _runBattle;
	public RunOutsideOfBattle _runOutsideOfBattle;

	public int numOfLordsPerFaction;

	#region sprites
	public Sprite floor_woodTile;
	#endregion sprites

	// Use this for initialization
	void Start () 
	{
		_transform = GetComponent<Transform>();
		_setupGrid = _transform.GetComponent<SetupGrid>();
		_gameData = _transform.GetComponent<GameData>();
		_setupLords = _transform.gameObject.AddComponent<SetupLords>();
		_setupLordClasses = _transform.gameObject.AddComponent<SetupLordClasses>();
		_setupEncounterTypes = _transform.gameObject.AddComponent<SetupEncounterTypes>();
		_setupEncounters = _transform.gameObject.AddComponent<SetupEncounters>();
		_setupQuests = _transform.gameObject.AddComponent<SetupQuests>();
		_setupFactions = _transform.gameObject.AddComponent<SetupFactions>();
		_readCSV = _transform.gameObject.AddComponent<ReadCSV>();
		_setupLevel = _transform.gameObject.AddComponent<SetupLevel>();
		//_runBattle = _transform.gameObject.AddComponent<RunBattle>();
		_runOutsideOfBattle = _transform.gameObject.AddComponent<RunOutsideOfBattle>();

		generateData();

		checkClasses ();
		checkLord();
		checkFactions();
	}

	void generateData()
	{
		_gameData._transform = _transform;

		_setupLordClasses._transform = _transform;
		_setupLordClasses._gameData = _gameData;
		_setupLordClasses._generateGameData = this;
		_setupLordClasses.createClasses();

		_setupLords._transform = _transform;
		_setupLords._gameData = _gameData;
		_setupLords._generateGameData = this;
		_setupLords.generateNames ();
		_setupLords.createLords();

		_setupFactions._transform = _transform;
		_setupFactions._gameData = _gameData;
		_setupFactions._generateGameData = this;
		_setupFactions.numOfLordsToDraft = numOfLordsPerFaction;
		_setupFactions.generateFactionNames ();
		_setupFactions.availableLords = _gameData.lords;
		_setupFactions.createFactions();
		//_setupFactions.customFactions ();

		_setupEncounterTypes._transform = _transform;
		_setupEncounterTypes._gameData = _gameData;
		_setupEncounterTypes._generateGameData = this;
		_setupEncounterTypes.createEncounterTypes();

		_setupEncounters._transform = _transform;
		_setupEncounters._gameData = _gameData;
		_setupEncounters._generateGameData = this;
		_setupEncounters.createEncounters();

		_setupQuests._transform = _transform;
		_setupQuests._gameData = _gameData;
		_setupQuests._generateGameData = this;
		_setupQuests.createQuests();

//		_setupLevel._transform = _transform;
//		_setupLevel._gameData = _gameData;
//		_setupLevel._generateGameData = this;
//		_setupLevel._readCSV = _readCSV;
//		_setupLevel.getUIElements();
//		_setupLevel._scale = 5;
//		_setupLevel.createGrid("woodtest.csv");

		int rows = 9;
		int cols = 9;

		int factionID1 = 0;
		int factionID2 = 1;
		
		_gameData.factions[factionID1].isPlayerControlled = true;
		_setupGrid._gameData = _gameData;
		_setupGrid._readCSV = _readCSV;
		_setupGrid.objectScale = 3;
		_setupGrid.cellSize = 16;
		_setupGrid.getUIElements();
		_setupGrid.makeGrid (rows,cols,"Stone Room.csv");

		//_setupGrid._runBattle = _runBattle;
		//_setupGrid.putFactionsOnGrid(factionID1,factionID2,rows,cols);

//		_runBattle._transform = _transform;
//		_runBattle._gameData = _gameData;
//		_runBattle._setupGrid = _setupGrid;
//		_runBattle.battleOrder = _setupGrid.getBattleOrder(factionID1,factionID2);;
//		_runBattle.factionIDSide1 = factionID1;
//		_runBattle.factionIDSide2 = factionID2;
//		_runBattle.getUIElements();
//		_runBattle.setInitialState();

		_runOutsideOfBattle._transform = _transform;
		_runOutsideOfBattle._gameData = _gameData;
		_runOutsideOfBattle._setupGrid = _setupGrid;
		_runOutsideOfBattle.targetLord = _setupGrid.putLordOnGrid(0,3,3);

	}

	void checkClasses()
	{
		string log = "";
		for(int i = 0; i < _gameData.lordClasses.Count; i++)
		{
			log+=
			"Class: " + i
			+"\n\t id: "+ _gameData.lordClasses[i].id
			+"\n\t name: "+ _gameData.lordClasses[i].name
			+"\n\t attack: "+ _gameData.lordClasses[i].attack
			+"\n\t defense: "+ _gameData.lordClasses[i].defense
			+"\n\t speed: "+ _gameData.lordClasses[i].speed
			+"\n";
		}
		Debug.Log(log);
	}

	void checkLord()
	{
		string log = "";
		for(int i = 0; i < _gameData.lords.Count; i++)
		{
			log+=
				"Lord: " + i
					+"\n\t id: " + _gameData.lords[i].id
					+"\n\t name: " + _gameData.lords[i].lordName
					+"\n\t classid: " + _gameData.lordClasses[_gameData.lords[i].classid].lordclassName
					+"\n\t level: " + _gameData.lords[i].level
					+"\n\t attack: " + _gameData.lords[i].attack
					+"\n\t defense: " + _gameData.lords[i].defense
					+"\n\t speed: " + _gameData.lords[i].speed
					+"\n\t exp: " + _gameData.lords[i].exp
					+"\n\t isavailable: " + _gameData.lords[i].isAvailable
					+"\n\t factionid: " + _gameData.lords[i].factionID
					+"\n";
		}
		Debug.Log(log);
	}

	void checkFactions()
	{
		string log = "";
		for(int i = 0; i < _gameData.factions.Count; i++)
		{
			string lordNames = "";
			for(int p = 0; p < _gameData.factions[i].lords.Count; p++)
			{
				lordNames += "\n\t\t" + _gameData.lords[_gameData.factions[i].lords[p]].lordName + ", " + _gameData.lordClasses[_gameData.lords[_gameData.factions[i].lords[p]].classid].lordclassName;
			}

			log+=
				"Faction: " + i
					+"\n\t id: " + _gameData.factions[i].id
					+"\n\t name: " + _gameData.factions[i].factionName
					+"\n\t isPlayerControlled: " + _gameData.factions[i].isPlayerControlled
					+"\n\t lordNames: " + lordNames
					+"\n";
		}

		Debug.Log(log);
	}
}
