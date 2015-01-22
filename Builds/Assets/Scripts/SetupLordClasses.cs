using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SetupLordClasses : MonoBehaviour 
{
	#region components
	public Transform _transform;
	public GameData _gameData;
	public GenerateGameData _generateGameData;
	#endregion components

	public void setupClass
	(
		int id
		,string name
		,string description
		,int attack
		,int defense
		,int speed
		,int attackRange
		,int maxHealth
	)
	{
		LordClass newClass = gameObject.AddComponent<LordClass>();
		
		newClass.id = id;
		newClass.lordclassName = name;
		newClass.description = description;
		newClass.attack = attack;
		newClass.defense = defense;
		newClass.speed = speed;
		newClass.attackRange = attackRange;
		newClass.maxHealth = maxHealth;
		//newClass.sprite = classSprites[id];

		_gameData.lordClasses.Add (id,newClass);		
	}

	public void createClasses()
	{
		setupClass
		(
			//id
			0
			//name
			,"Fighter"
			//description
			,"Fighter placeholder desc"
			//attack
			,6 
			//defense
			,7 
			//speed
			,3
			//attack range
			,1
			//maxHealth
			,7
		);
		setupClass
		(
			//id
			1
			//name
			,"Defender"
			//description
			,"Defender placeholder desc"
			//attack
			,4
			//defense
			,10
			//speed
			,2
			//attack range
			,1
			//maxHealth
			,10
		);
		setupClass
		(
			//id
			2
			//name
			,"Rogue"
			//description
			,"Rogue placeholder desc"
			//attack
			,10
			//defense
			,5
			//speed
			,5
			//attack range
			,3
			//maxHealth
			,5
		);
	}
}
