using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RunBattle : MonoBehaviour 
{
	public Transform _transform;
	public GameData _gameData;
	public SetupGrid _setupGrid;

	#region factioninfo
	public int factionIDSide1;
	public int factionIDSide2;

	#endregion factioninfo

	#region playerturnvars
	public List<int> battleOrder;
	public int currentBattleOrderNum;
	public int currentNodeInPath;
	public List<GameObject> cellsToMoveTo = new List<GameObject>();
	public bool foundNearestSpace;

	public List<GameObject> possibleMovementSpaces = new List<GameObject>();
	public List<GameObject> possibleAttackSpaces = new List<GameObject>();

	public bool isMoving;
	public bool isAttacking;
	public bool alreadyAttacked;
	public bool alreadyMoved;
	public bool turnIsFinished;
	#endregion playerturnvars

	#region pathfinding
	List<GameObject> attackNodesAlreadyChecked = new List<GameObject>();
	#endregion pathfinding

	#region colors
	public Color32 c_possibleMovementSpaceBlue;
	public Color32 c_friendlyBlue;
	public Color32 c_invalidSpaceRed;
	public Color32 c_enemyRed;
	public Color32 c_currentTurnWhite;
	public Color32 c_enemyInAttackRange;
	public Color32 c_transparent;
	#endregion colors

	#region UI
	public GameObject Canvas;
	public GameObject cell;
	public GameObject cellContainer;
	public GameObject endTurnButton;
	
	public Font arial;
	#endregion UI

	public void getUIElements()
	{
		Canvas = GameObject.Find ("Canvas");
		cell = GameObject.Find ("Cell");
		cellContainer = GameObject.Find ("Cell Container");
		endTurnButton = GameObject.Find ("End Turn Button");

		c_possibleMovementSpaceBlue = new Color32(92,131,223,150);
		c_friendlyBlue = new Color32(130,249,126,150);
		c_invalidSpaceRed = new Color32(251,126,126,150);
		c_enemyRed = new Color32(255,19,19,150);
		c_currentTurnWhite = new Color32(255,255,255,150);
		c_enemyInAttackRange = new Color32(255,168,75,150);
		
		endTurnButton.GetComponent<Button>().onClick.AddListener(delegate{StartCoroutine(endTurn());});
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(currentBattleOrderNum < battleOrder.Count)
		{
			if(!_gameData.factions[getCurrentLord().factionID].isPlayerControlled)
			{
				if(!turnIsFinished)
				{
					AITakeTurn();
				}
				else
				{
					incrementBattleOrder();
				}
			}
		}

		if(isMoving)
		{
			if(cellsToMoveTo.Count > 0)
			{
				moveLord(getCurrentLord(),cellsToMoveTo[cellsToMoveTo.Count-1].GetComponent<Cell>().row,cellsToMoveTo[cellsToMoveTo.Count-1].GetComponent<Cell>().col);
			}
			else
			{
				alreadyMoved = true;
				isMoving = false;
				updateCellColors (true);
			}
		}
	}

	public void setInitialState()
	{
		updateCellColors(false);

		alreadyAttacked = false;
		alreadyMoved = false;
		isMoving = false;
		isAttacking = false;
		turnIsFinished = false;
		foundNearestSpace = false;

		cellsToMoveTo = new List<GameObject>();
	}

	public void updateCellColors(bool clearMovementSpaces)
	{
		GameObject[] allnodes = GameObject.FindGameObjectsWithTag("Cell");
		possibleMovementSpaces = assignCellPathScores (getCurrentLord ());
		//possibleAttackSpaces = assignAttackCellScores (getCurrentLord(),getCurrentLord().attackRange,GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol),false);
		//assignAttackCellScores (getCurrentLord(),getCurrentLord().attackRange,GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol),false);
		if(getCurrentLord().factionID == factionIDSide1)
		{
			cellsToMoveTo = nearestSpaceToAttackFrom(getCurrentLord(),_gameData.factions[factionIDSide2]).GetComponent<Cell>().path;
		}
		else
		{
			cellsToMoveTo = nearestSpaceToAttackFrom(getCurrentLord(),_gameData.factions[factionIDSide1]).GetComponent<Cell>().path;
		}
		
		foreach(GameObject node in allnodes)
		{
			if(!node.GetComponent<Cell>().hasLord)
			{
				Button tempButton = node.transform.GetComponentInChildren<Button>();
				tempButton.GetComponent<Image>().color = c_transparent;
				ColorBlock tempColorBlock = tempButton.GetComponent<Button>().colors;
				tempColorBlock.normalColor = c_transparent;
				tempButton.colors = tempColorBlock;
			}
			else
			{
				Color32 tempCellColor = c_friendlyBlue;
				
				if(_gameData.lords[node.GetComponent<Cell>().lordID].factionID == factionIDSide2)
				{
					tempCellColor = c_enemyRed;
				}

				Button tempButton = node.GetComponentInChildren<Button>();
				tempButton.GetComponent<Image>().color = tempCellColor;
				ColorBlock tempColorBlock = tempButton.GetComponent<Button>().colors;
				tempColorBlock.normalColor = tempCellColor;
				tempButton.colors = tempColorBlock;
			}
		}
		
		foreach(GameObject node in possibleMovementSpaces)
		{
			if(clearMovementSpaces)
			{
				Button tempButton = node.transform.GetComponentInChildren<Button>();
				tempButton.GetComponent<Image>().color = c_transparent;
				ColorBlock tempColorBlock = tempButton.GetComponent<Button>().colors;
				tempColorBlock.normalColor = c_transparent;
				tempButton.colors = tempColorBlock;
			}
			else
			{
				Button tempButton = node.transform.GetComponentInChildren<Button>();
				tempButton.GetComponent<Image>().color = c_possibleMovementSpaceBlue;
				ColorBlock tempColorBlock = tempButton.GetComponent<Button>().colors;
				tempColorBlock.normalColor = c_possibleMovementSpaceBlue;
				tempButton.colors = tempColorBlock;
			}
		}
		
		foreach(GameObject node in possibleAttackSpaces)
		{
			Button tempButton = node.transform.GetComponentInChildren<Button>();
			tempButton.GetComponent<Image>().color = c_enemyInAttackRange;
			ColorBlock tempColorBlock = tempButton.GetComponent<Button>().colors;
			tempColorBlock.normalColor = c_enemyInAttackRange;
			tempButton.colors = tempColorBlock;
		}

		{
			Button tempButton = GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol).transform.GetComponentInChildren<Button>();
			tempButton.GetComponent<Image>().color = c_currentTurnWhite;
			ColorBlock tempColorBlock = tempButton.GetComponent<Button>().colors;
			tempColorBlock.normalColor = c_currentTurnWhite;
			tempButton.colors = tempColorBlock;
		}
	}

	public Lord getCurrentLord()
	{
		if(battleOrder[currentBattleOrderNum] == -1)
		{
			incrementBattleOrder();
		}

		return _gameData.lords[battleOrder[currentBattleOrderNum]];
	}

	public void doDamage(Lord lord, int row, int col)
	{
		int damage = UnityEngine.Random.Range (0,lord.attack);

		Cell cellToAttack = GameObject.Find ("Cell"+row+","+col).GetComponent<Cell>();
		
		if(_gameData.lords[cellToAttack.lordID].currentHealth - damage > 0)
		{
			_gameData.lords[cellToAttack.lordID].currentHealth -= damage;
		}
		else
		{
//			List<int> tempBattleOrder = new List<int>();
//			
//			for(int i = 0; i < battleOrder.Count; i++)
//			{
//				if(battleOrder[i] != cellToAttack.lordID)
//				{
//					tempBattleOrder.Add (battleOrder[i]);
//				}
//				else
//				{
//					tempBattleOrder.Add (-1);
//				}
//			}
//			
//			battleOrder = tempBattleOrder;
//			
//			cellToAttack.hasLord = false;
//
//			cellToAttack.lordID = -1;	
		}
	}

	public IEnumerator endTurn()
	{
		incrementBattleOrder();
		
		yield return new WaitForSeconds(1f);
	}
	
	#region AI
	public void AITakeTurn()
	{
		Lord lord = getCurrentLord();

		if(!isMoving && !alreadyAttacked)
		{
			if(possibleAttackSpaces.Count > 0)
			{
				StartCoroutine(AIAttack(lord, possibleAttackSpaces));
			}
		}

		if(!isAttacking && !alreadyMoved && !isMoving)
		{
			StartCoroutine(AIMove(lord));
		}
		
		if(alreadyMoved && !isAttacking && possibleAttackSpaces.Count == 0)
		{
			turnIsFinished = true;
		}
		
		if(alreadyAttacked && alreadyMoved)
		{
			turnIsFinished = true;
		}
	}

	public IEnumerator AIAttack(Lord lord, List<GameObject> possibleEnemySpaces)
	{
		isAttacking = true;
		float waitTime = 0f;
		GameObject cellToAttack = possibleEnemySpaces[0];
		Lord lordToAttack = _gameData.lords[cellToAttack.GetComponent<Cell>().lordID];

		foreach(GameObject possibleEnemySpace in possibleEnemySpaces)
		{
			if(possibleEnemySpace.GetComponent<Cell>().lordID != -1)
			{
				Lord possibleLord = _gameData.lords[possibleEnemySpace.GetComponent<Cell>().lordID];
				
				if(lordToAttack.currentHealth > possibleLord.currentHealth)
				{
					cellToAttack = possibleEnemySpace;
					lordToAttack = possibleLord;
				}
			}
		}
		
		int row = cellToAttack.GetComponent<Cell>().row;
		int col = cellToAttack.GetComponent<Cell>().col;
		
		doDamage (lord,row,col);
		
		isAttacking = false;
		alreadyAttacked = true;
		
		yield return new WaitForSeconds(waitTime);
	}

	public IEnumerator AIMove(Lord lord)
	{
		float waitTime = 0f;

		if(!foundNearestSpace)
		{
			if(!isMoving)
			{
				cellsToMoveTo = GameObject.Find ("Cell"+lord.currentRow+","+lord.currentCol).GetComponent<Cell>().path;
			}

			if(lord.factionID == factionIDSide1)
			{
				cellsToMoveTo = nearestSpaceToAttackFrom(lord,_gameData.factions[factionIDSide2]).GetComponent<Cell>().path;
			}
			else
			{
				cellsToMoveTo = nearestSpaceToAttackFrom(lord,_gameData.factions[factionIDSide1]).GetComponent<Cell>().path;
			}
		}

		isMoving = true;
		
		yield return new WaitForSeconds(waitTime);
	}
	#endregion AI

	#region playerInput
	public IEnumerator handleClick(int row, int col)
	{
		float waitTime = 2f;

		Lord lord = getCurrentLord();

		GameObject cell = GameObject.Find ("Cell" + row + "," + col);
		int lordIDInTargetCell = cell.GetComponent<Cell>().lordID;
		
		if(lordIDInTargetCell > -1 && possibleAttackSpaces.Contains(cell))
		{
			if(!alreadyAttacked)
			{
				if(_gameData.lords[lordIDInTargetCell].factionID != lord.factionID)
				{
					doDamage (lord,row,col);
					possibleMovementSpaces = assignCellPathScores (lord);
					alreadyAttacked = true;
				}
			}
		}
		else
		{
			if(!alreadyMoved && lordIDInTargetCell == -1 && possibleMovementSpaces.Contains (cell))
			{
				cellsToMoveTo = GameObject.Find ("Cell"+row+","+col).GetComponent<Cell>().path;
				isMoving = true;
			}
		}
		
		if(alreadyMoved && possibleAttackSpaces.Count == 0)
		{
			alreadyAttacked = true;
		}
		
		if(alreadyAttacked && possibleMovementSpaces.Count == 0)
		{
			alreadyMoved = true;
		}
		
		if(alreadyMoved && alreadyAttacked)
		{
			incrementBattleOrder();
		}
		
		yield return new WaitForSeconds(waitTime);
	}
	#endregion playerInput

	#region movement
	public void moveLord(Lord lord, int newRow, int newCol)
	{
		GameObject newCell = GameObject.Find ("Cell" + newRow + "," + newCol);
		GameObject oldCell = GameObject.Find ("Cell" + lord.currentRow + "," + lord.currentCol);
		
		if(newCell != oldCell)
		{
			moveAlongPath (GameObject.Find (lord.lordName), GameObject.Find ("Cell" + newRow + "," + newCol).GetComponent<Cell>().path);
			
			newCell.GetComponent<Cell>().lordID = lord.id;
			
			newCell.GetComponent<Cell>().hasLord = true;
			newCell.transform.FindChild ("Cell" + newRow + "," + newCol+"_button").GetComponent<Image>().color = c_currentTurnWhite;
			
			oldCell.GetComponent<Cell>().lordID = -1;
			oldCell.GetComponent<Cell>().hasLord = false;
		}
	}
	
	void moveAlongPath(GameObject player, List<GameObject> path)
	{
		isMoving = true;

		if(currentNodeInPath < path.Count)
		{
			move (player,path[currentNodeInPath],1f);
			
			int xDistance = (int) (player.GetComponent<RectTransform>().localPosition.x - path[currentNodeInPath].GetComponent<RectTransform>().localPosition.x);
			int yDistance = (int) (player.GetComponent<RectTransform>().localPosition.y - path[currentNodeInPath].GetComponent<RectTransform>().localPosition.y);
			
			if(xDistance == 0 && yDistance == 0)
			{
				player.GetComponent<RectTransform>().localPosition = path[currentNodeInPath].GetComponent<RectTransform>().localPosition;
				_gameData.lords[player.GetComponent<LordAvatar>().lordID].currentRow = path[currentNodeInPath].GetComponent<Cell>().row;
				_gameData.lords[player.GetComponent<LordAvatar>().lordID].currentCol = path[currentNodeInPath].GetComponent<Cell>().col;
				player.rigidbody2D.velocity = new Vector2(0,0);
				currentNodeInPath++;
			}
		}

		if(currentNodeInPath == path.Count)
		{
			currentNodeInPath = 1;
			isMoving = false;
			alreadyMoved = true;
			GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol).GetComponent<Cell>().hasLord = false;
			getCurrentLord().currentRow = path[path.Count-1].GetComponent<Cell>().row;
			getCurrentLord().currentCol = path[path.Count-1].GetComponent<Cell>().col;
			GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol).GetComponent<Cell>().hasLord = true;

			if(!alreadyAttacked)
			{
				//possibleAttackSpaces = assignAttackCellScores(getCurrentLord(),getCurrentLord().attackRange,GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol),false);
				//assignAttackCellScores(getCurrentLord(),getCurrentLord().attackRange,GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol),false);
				if(getCurrentLord().factionID == factionIDSide1)
				{
					nearestSpaceToAttackFrom(getCurrentLord(),_gameData.factions[factionIDSide2]);
				}
				else
				{
					nearestSpaceToAttackFrom(getCurrentLord(),_gameData.factions[factionIDSide1]);
				}
				
				updateCellColors(true);
			}
		}
	}
	
	void move(GameObject player, GameObject destination, float movementSpeed)
	{
		bool isHorizontalMovement;
		Lord playerLord = getCurrentLord();
		
		if(playerLord.currentRow == destination.GetComponent<Cell>().row)
		{
			isHorizontalMovement = true;
		}
		else
		{
			isHorizontalMovement = false;
		}
		
		if(isHorizontalMovement)
		{
			bool isLeft;
			
			if(player.GetComponent<RectTransform>().localPosition.x > destination.GetComponent<RectTransform>().localPosition.x)
			{
				isLeft = true;
			}
			else
			{
				isLeft = false;
			}
			if(isLeft)
			{
				player.rigidbody2D.velocity = new Vector2(-movementSpeed,0);
			}
			else
			{
				player.rigidbody2D.velocity = new Vector2(movementSpeed,0);
			}
		}
		else
		{
			bool isUp;
			
			if(player.GetComponent<RectTransform>().localPosition.y > destination.GetComponent<RectTransform>().localPosition.y)
			{
				isUp = false;
			}
			else
			{
				isUp = true;
			}
			if(isUp)
			{
				player.rigidbody2D.velocity = new Vector2(0,movementSpeed);
			}
			else
			{
				player.rigidbody2D.velocity = new Vector2(0,-movementSpeed);
			}
		}
	}
	#endregion movement

	#region setnodeproperties
	public void incrementBattleOrder()
	{
		if(currentBattleOrderNum < battleOrder.Count-1)
		{
			currentBattleOrderNum++;
		}
		else
		{
			currentBattleOrderNum = 0;
		}

		alreadyAttacked = false;
		alreadyMoved = false;
		isMoving = false;
		isAttacking = false;
		turnIsFinished = false;
		foundNearestSpace = false;
		cellsToMoveTo.Clear ();
		resetCells();
		
		updateCellColors(false);
	}

	public void resetCells()
	{
		GameObject[] navCells = GameObject.FindGameObjectsWithTag("Cell");
		
		foreach(GameObject navCell in navCells)
		{
			Cell navCellComponent = navCell.GetComponent<Cell>();
			navCellComponent.path.Clear ();
			navCellComponent.pathScore = 0;
			navCellComponent.neighborPathScore = 0;
			//navCellComponent.attackSpaceScore = 0;
			//navCellComponent.neighborAttackSpaceScore = 0;
			navCellComponent.distanceToEnemy = 0;
		}
	}

	public List<GameObject> assignCellPathScores(Lord lord)
	{
		List<GameObject> nodesToCheck = new List<GameObject>();
		List<GameObject> nodesInMovementRange = new List<GameObject>();
		List<GameObject> newNodesToCheck = new List<GameObject>();
		List<GameObject> nodesAlreadyChecked = new List<GameObject>();
		int movementRange = lord.speed;
		
		GameObject origin = GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol));
		GameObject originButton = GameObject.Find ("Cell"  + (lord.currentRow) + "," + (lord.currentCol)+"_button");
		Cell originCell = origin.GetComponent<Cell>();
		
		originCell.pathScore = 0;

		nodesAlreadyChecked.Add (origin);

		List<GameObject> initialNodesToCheck = new List<GameObject>();

		initialNodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow - 1) + "," + (lord.currentCol)));
		initialNodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow + 1) + "," + (lord.currentCol)));
		initialNodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol - 1)));
		initialNodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol + 1)));

		foreach(GameObject initialNodeToCheck in initialNodesToCheck)
		{
			if(initialNodeToCheck)
			{
				if(!initialNodeToCheck.GetComponent<Cell>().hasLord)
				{
					nodesToCheck.Add (initialNodeToCheck);
				}
			}
		}
		
		foreach(GameObject nodeToCheck in nodesToCheck)
		{
			if(nodeToCheck)
			{
				Cell nodeToCheckCell = nodeToCheck.GetComponent<Cell>();
				
				if(!nodeToCheckCell.isInvalidSpace && (!nodeToCheckCell.hasLord || nodeToCheckCell == originCell))
				{
					nodeToCheckCell.neighborPathScore = 0;
					nodeToCheckCell.pathScore = 1;
					nodeToCheckCell.path.Clear ();
					nodeToCheckCell.path.Add (origin);
					nodeToCheckCell.path.Add (nodeToCheck);
					
					nodesAlreadyChecked.Add (nodeToCheck);
					
					if(nodeToCheckCell.pathScore <= movementRange)
					{
						nodesInMovementRange.Add (nodeToCheck);
					}
					
					string[] gameObjectsToCheck = new string[4];
					gameObjectsToCheck[0] = "Cell" + (nodeToCheckCell.row - 1) + "," + (nodeToCheckCell.col);
					gameObjectsToCheck[1] = "Cell" + (nodeToCheckCell.row + 1) + "," + (nodeToCheckCell.col);
					gameObjectsToCheck[2] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col - 1);
					gameObjectsToCheck[3] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col + 1);
					
					foreach(string gameObjectToCheck in gameObjectsToCheck)
					{
						GameObject gmRef = GameObject.Find (gameObjectToCheck);
						
						if(
							gmRef 
							&& (!nodesAlreadyChecked.Contains (gmRef) || gmRef.GetComponent<Cell>().pathScore > nodeToCheckCell.pathScore + 1)
							//&& !nodeToCheckCell.hasLord
							)
						{
							Cell gmRefCell = gmRef.GetComponent<Cell>();
							
							if(!gmRefCell.hasLord)
							{
								newNodesToCheck.Add (gmRef);
								gmRefCell.neighborPathScore = nodeToCheckCell.pathScore;
								
								gmRefCell.path.Clear ();
								
								foreach(GameObject node in nodeToCheckCell.path)
								{
									gmRefCell.path.Add (node);
								}
								
								gmRefCell.path.Add (gmRef);
							}
						}
					}
				}
			}
		}
		
		nodesToCheck = newNodesToCheck;
		
		for(int i = 0; i < nodesToCheck.Count; i++)
		{
			GameObject nodeToCheck = nodesToCheck[i];
			
			if(nodeToCheck)
			{
				Cell nodeToCheckCell = nodeToCheck.GetComponent<Cell>();
				
				if(!nodeToCheckCell.isInvalidSpace && (!nodeToCheckCell.hasLord || nodeToCheckCell == originCell))
				{
					nodeToCheckCell.pathScore = nodeToCheckCell.neighborPathScore + 1;
					
					nodesAlreadyChecked.Add (nodeToCheck);
					
					if(nodeToCheckCell.pathScore <= movementRange)
					{
						nodesInMovementRange.Add (nodeToCheck);
					}
					
					string[] gameObjectsToCheck = new string[4];
					gameObjectsToCheck[0] = "Cell" + (nodeToCheckCell.row - 1) + "," + (nodeToCheckCell.col);
					gameObjectsToCheck[1] = "Cell" + (nodeToCheckCell.row + 1) + "," + (nodeToCheckCell.col);
					gameObjectsToCheck[2] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col - 1);
					gameObjectsToCheck[3] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col + 1);
					
					foreach(string gameObjectToCheck in gameObjectsToCheck)
					{
						GameObject gmRef = GameObject.Find (gameObjectToCheck);
						bool addNode = true;
						
						if(gmRef)
						{
							if(nodesToCheck.Contains (gmRef))
							{
								foreach(GameObject tempNode in nodesToCheck)
								{
									if(tempNode == gmRef)
									{
										if(tempNode.GetComponent<Cell>().pathScore > nodeToCheckCell.pathScore + 1)
										{
											addNode = true;
										}
									}
									else
									{
										addNode = false;
									}
								}
							}
							else
							{
								addNode = true;
							}
							
							if(
								(!nodesAlreadyChecked.Contains (gmRef) || gmRef.GetComponent<Cell>().pathScore > nodeToCheckCell.pathScore + 1)
								&& addNode
								//&& !nodeToCheckCell.hasLord
								)
							{
								Cell gmRefCell = gmRef.GetComponent<Cell>();
								
								if(!gmRefCell.hasLord)
								{
									newNodesToCheck.Add (gmRef);
									gmRefCell.neighborPathScore = nodeToCheckCell.pathScore;
									
									gmRefCell.path.Clear ();
									
									foreach(GameObject node in nodeToCheckCell.path)
									{
										gmRefCell.path.Add (node);
									}
									
									gmRefCell.path.Add (gmRef);
								}
							}
						}
					}
				}
			}
		}
		
		return nodesInMovementRange;
	}

//	public List<GameObject> assignAttackCellScores(Lord lord, int attackRange)
//	{
//		List<GameObject> nodesToCheck = new List<GameObject>();
//		List<GameObject> nodesInAttackRange = new List<GameObject>();
//		List<GameObject> newNodesToCheck = new List<GameObject>();
//
//		GameObject attackSpace = GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol));
//		
//		nodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow - 1) + "," + (lord.currentCol)));
//		nodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow + 1) + "," + (lord.currentCol)));
//		nodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol - 1)));
//		nodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol + 1)));
//		
//		foreach(GameObject nodeToCheck in nodesToCheck)
//		{
//			if(nodeToCheck)
//			{
//				Cell nodeToCheckCell = nodeToCheck.GetComponent<Cell>();
//				
//				if(!nodeToCheckCell.isInvalidSpace)
//				{
//					nodeToCheckCell.neighborAttackPathSpaceScore = 0;
//					nodeToCheckCell.attackSpaceScore = 1;
//					
//					if(!attackNodesAlreadyChecked.Contains(nodeToCheck))
//					{
//						attackNodesAlreadyChecked.Add (nodeToCheck);
//					}
//
//					if(nodeToCheckCell.attackSpaceScore <= attackRange 
//					   && !nodesInAttackRange.Contains (nodeToCheck) 
//					   && nodeToCheckCell.hasLord
//					   )
//					{
//						if(lord.factionID != _gameData.lords[nodeToCheckCell.lordID].factionID)
//						{
//							nodesInAttackRange.Add (nodeToCheck);
//						}
//					}
//					
//					string[] gameObjectsToCheck = new string[4];
//					gameObjectsToCheck[0] = "Cell" + (nodeToCheckCell.row - 1) + "," + (nodeToCheckCell.col);
//					gameObjectsToCheck[1] = "Cell" + (nodeToCheckCell.row + 1) + "," + (nodeToCheckCell.col);
//					gameObjectsToCheck[2] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col - 1);
//					gameObjectsToCheck[3] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col + 1);
//					
//					foreach(string gameObjectToCheck in gameObjectsToCheck)
//					{
//						GameObject gmRef = GameObject.Find (gameObjectToCheck);
//						
//						if(
//							gmRef 
//							&& 
//							(
//							(!attackNodesAlreadyChecked.Contains (gmRef) 
//						 || gmRef.GetComponent<Cell>().attackSpaceScore > nodeToCheckCell.attackSpaceScore + 1)
//							|| gmRef.GetComponent<Cell>().attackSpaceScore == 0
//							)
//							)
//						{
//							Cell gmRefCell = gmRef.GetComponent<Cell>();
//							
//							if(!gmRefCell.hasLord || gmRef == attackSpace)
//							{
//								newNodesToCheck.Add (gmRef);
//								gmRefCell.neighborAttackSpaceScore = nodeToCheckCell.attackSpaceScore;
//							}
//						}
//					}
//				}
//			}
//		}
//		
//		nodesToCheck = newNodesToCheck;
//		
//		for(int i = 0; i < nodesToCheck.Count; i++)
//		{
//			GameObject nodeToCheck = nodesToCheck[i];
//			
//			if(nodeToCheck)
//			{
//				Cell nodeToCheckCell = nodeToCheck.GetComponent<Cell>();
//				
//				if(!nodeToCheckCell.isInvalidSpace /*&& !nodeToCheckCell.hasLord*/)
//				{
//					if(!attackNodesAlreadyChecked.Contains(nodeToCheck))
//					{
//						attackNodesAlreadyChecked.Add (nodeToCheck);
//						nodeToCheckCell.attackSpaceScore = nodeToCheckCell.neighborAttackSpaceScore + 1;
//					}
//					else
//					{
//						if(nodeToCheckCell.attackSpaceScore > nodeToCheckCell.neighborAttackSpaceScore + 1)
//						{
//							nodeToCheckCell.attackSpaceScore = nodeToCheckCell.neighborAttackSpaceScore + 1;
//						}
//					}
//					
//					if(nodeToCheckCell.attackSpaceScore <= attackRange 
//					   && !nodesInAttackRange.Contains (nodeToCheck) 
//					   && nodeToCheckCell.hasLord
//					   )
//					{
//						if(lord.factionID != _gameData.lords[nodeToCheckCell.lordID].factionID)
//						{
//							nodesInAttackRange.Add (nodeToCheck);
//						}
//					}
//					
//					string[] gameObjectsToCheck = new string[4];
//					gameObjectsToCheck[0] = "Cell" + (nodeToCheckCell.row - 1) + "," + (nodeToCheckCell.col);
//					gameObjectsToCheck[1] = "Cell" + (nodeToCheckCell.row + 1) + "," + (nodeToCheckCell.col);
//					gameObjectsToCheck[2] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col - 1);
//					gameObjectsToCheck[3] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col + 1);
//					
//					foreach(string gameObjectToCheck in gameObjectsToCheck)
//					{
//						GameObject gmRef = GameObject.Find (gameObjectToCheck);
//						
//						if(
//							gmRef 
//							&& 
//							(
//							(!attackNodesAlreadyChecked.Contains (gmRef) 
//						 || gmRef.GetComponent<Cell>().attackSpaceScore > nodeToCheckCell.attackSpaceScore + 1)
//							|| gmRef.GetComponent<Cell>().attackSpaceScore == 0
//							)
//							)
//						{
//							Cell gmRefCell = gmRef.GetComponent<Cell>();
//							
//							if(!gmRefCell.hasLord || gmRef == attackSpace)
//							{
//								newNodesToCheck.Add (gmRef);
//								gmRefCell.neighborAttackSpaceScore = nodeToCheckCell.attackSpaceScore;
//							}
//						}
//					}
//				}
//			}
//		}
//		
//		return nodesInAttackRange;
//	}

	public List<GameObject> assignAttackCellScores(Lord lord, int attackRange, GameObject attackSpace, bool isAIAttacking)
	{
		List<GameObject> nodesToCheck = new List<GameObject>();
		List<GameObject> nodesInAttackRange = new List<GameObject>();
		List<GameObject> newNodesToCheck = new List<GameObject>();

		List<GameObject> initialNodesToCheck = new List<GameObject>();

		//initialNodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol)));
		initialNodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow - 1) + "," + (lord.currentCol)));
		initialNodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow + 1) + "," + (lord.currentCol)));
		initialNodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol - 1)));
		initialNodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol + 1)));

		foreach(GameObject initialNodeToCheck in initialNodesToCheck)
		{
			if(initialNodeToCheck)
			{
				if(!initialNodeToCheck.GetComponent<Cell>().hasLord && !initialNodeToCheck.GetComponent<Cell>().isInvalidSpace)
				{
					nodesToCheck.Add (initialNodeToCheck);
				}
			}
		}
		
		foreach(GameObject nodeToCheck in nodesToCheck)
		{
			if(nodeToCheck)
			{
				Cell nodeToCheckCell = nodeToCheck.GetComponent<Cell>();
				
				if(!nodeToCheckCell.isInvalidSpace)
				{
					nodeToCheckCell.neighborAttackPathSpaceScore = 0;
					nodeToCheckCell.attackSpaceScore = 1;

					if(!attackNodesAlreadyChecked.Contains(nodeToCheck))
					{
						attackNodesAlreadyChecked.Add (nodeToCheck);
					}

//					if(isAIAttacking)
					{
						if(nodeToCheckCell.attackSpaceScore <= attackRange && !nodesInAttackRange.Contains (nodeToCheck))
						{
							nodesInAttackRange.Add (nodeToCheck);

							if(nodeToCheckCell.lordID != -1)
							{
								possibleAttackSpaces.Add (nodeToCheck);
							}
						}
					}
//					else
//					{
//						if(nodeToCheckCell.attackSpaceScore <= attackRange 
//						   && !nodesInAttackRange.Contains (nodeToCheck) 
//						   && nodeToCheckCell.hasLord
//						   )
//						{
//							if(lord.factionID != _gameData.lords[nodeToCheckCell.lordID].factionID)
//							{
//								nodesInAttackRange.Add (nodeToCheck);
//								possibleAttackSpaces.Add (nodeToCheck);
//							}
//						}
//					}
					
					string[] gameObjectsToCheck = new string[4];
					gameObjectsToCheck[0] = "Cell" + (nodeToCheckCell.row - 1) + "," + (nodeToCheckCell.col);
					gameObjectsToCheck[1] = "Cell" + (nodeToCheckCell.row + 1) + "," + (nodeToCheckCell.col);
					gameObjectsToCheck[2] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col - 1);
					gameObjectsToCheck[3] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col + 1);
					
					foreach(string gameObjectToCheck in gameObjectsToCheck)
					{
						GameObject gmRef = GameObject.Find (gameObjectToCheck);
						
						if(
							gmRef 
							&& 
							(
							(!attackNodesAlreadyChecked.Contains (gmRef) 
						 	|| gmRef.GetComponent<Cell>().attackSpaceScore > nodeToCheckCell.attackSpaceScore + 1)
							|| gmRef.GetComponent<Cell>().attackSpaceScore == 0
							)
							)
						{
							Cell gmRefCell = gmRef.GetComponent<Cell>();
							
							if(!gmRefCell.hasLord || gmRef == attackSpace)
							{
								newNodesToCheck.Add (gmRef);
								gmRefCell.neighborAttackSpaceScore = nodeToCheckCell.attackSpaceScore;
							}
						}
					}
				}
			}
		}
		
		nodesToCheck = newNodesToCheck;
		
		for(int i = 0; i < nodesToCheck.Count; i++)
		{
			GameObject nodeToCheck = nodesToCheck[i];
			
			if(nodeToCheck)
			{
				Cell nodeToCheckCell = nodeToCheck.GetComponent<Cell>();
				
				if(!nodeToCheckCell.isInvalidSpace)
				{
					if(!attackNodesAlreadyChecked.Contains(nodeToCheck))
					{
						attackNodesAlreadyChecked.Add (nodeToCheck);
						nodeToCheckCell.attackSpaceScore = nodeToCheckCell.neighborAttackSpaceScore + 1;
					}
					else
					{
						if(nodeToCheckCell.attackSpaceScore > nodeToCheckCell.neighborAttackSpaceScore + 1)
						{
							nodeToCheckCell.attackSpaceScore = nodeToCheckCell.neighborAttackSpaceScore + 1;
						}
					}
					
//					if(isAIAttacking)
					{
						if(nodeToCheckCell.attackSpaceScore <= attackRange && !nodesInAttackRange.Contains (nodeToCheck))
						{
							nodesInAttackRange.Add (nodeToCheck);

							if(nodeToCheckCell.lordID != -1)
							{
								possibleAttackSpaces.Add (nodeToCheck);
							}
						}
					}
//					else
//					{
//						if(nodeToCheckCell.attackSpaceScore <= attackRange 
//						   && !nodesInAttackRange.Contains (nodeToCheck) 
//						   && nodeToCheckCell.hasLord
//						   )
//						{
//							if(lord.factionID != _gameData.lords[nodeToCheckCell.lordID].factionID)
//							{
//								nodesInAttackRange.Add (nodeToCheck);
//								possibleAttackSpaces.Add (nodeToCheck);
//							}
//						}
//					}
					
					string[] gameObjectsToCheck = new string[4];
					gameObjectsToCheck[0] = "Cell" + (nodeToCheckCell.row - 1) + "," + (nodeToCheckCell.col);
					gameObjectsToCheck[1] = "Cell" + (nodeToCheckCell.row + 1) + "," + (nodeToCheckCell.col);
					gameObjectsToCheck[2] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col - 1);
					gameObjectsToCheck[3] = "Cell" + (nodeToCheckCell.row) + "," + (nodeToCheckCell.col + 1);
					
					foreach(string gameObjectToCheck in gameObjectsToCheck)
					{
						GameObject gmRef = GameObject.Find (gameObjectToCheck);
						
						if(
							gmRef 
							&& 
							(
							(!attackNodesAlreadyChecked.Contains (gmRef) 
							|| gmRef.GetComponent<Cell>().attackSpaceScore > nodeToCheckCell.attackSpaceScore + 1)
							|| gmRef.GetComponent<Cell>().attackSpaceScore == 0
							)
							)
						{
							Cell gmRefCell = gmRef.GetComponent<Cell>();
							
							if(!gmRefCell.hasLord || gmRef == attackSpace)
							{
								newNodesToCheck.Add (gmRef);
								gmRefCell.neighborAttackSpaceScore = nodeToCheckCell.attackSpaceScore;
							}
						}
					}
				}
			}
		}

		return nodesInAttackRange;
	}
	
	public GameObject nearestSpaceToAttackFrom(Lord lord, Faction enemyFaction)
	{
		GameObject space = GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol));

		resetNodeAttackScores();
		possibleAttackSpaces.Clear ();
		
		/* Store cells that are in attack range of the enemy for the current lord */
		List<GameObject> cellsInRangeOfEnemy = new List<GameObject>();
		
		/* Store cells that are in the movement range of the current lord */
		List<GameObject> movementNodes = possibleMovementSpaces;
		movementNodes.Add (space);
		
		/* Store cells that are in movement range and in attack range */
		List<GameObject> movementAndAttackNodes = new List<GameObject>();
		
		/* Is there overlap in the movement rnage and the attack range? */
		bool movementRangeInAttackRange = false;
		
		/* Check each enemy in the opposing faction for their location */
		foreach(int enemyLordID in enemyFaction.lords)
		{
			if(battleOrder.Contains (_gameData.lords[enemyLordID].id))
			{
				List<GameObject> tempCellsInRangeOfEnemy = assignAttackCellScores(_gameData.lords[enemyLordID],lord.attackRange,space,true);

				foreach(GameObject node in tempCellsInRangeOfEnemy)
				{
					/* Check to make sure the node we are adding is not already in our list */
					if(!cellsInRangeOfEnemy.Contains (node))
					{
						cellsInRangeOfEnemy.Add (node);
						
						/* Check to see if the node we are adding is also in the range of possible movement spaces for the current lord */
						if(movementNodes.Contains (node))
						{
							movementRangeInAttackRange = true;
							
							movementAndAttackNodes.Add (node);
						}
					}
				}
			}
		}
		
		attackNodesAlreadyChecked.Clear ();

		/* If there is overlap between the possible movement spaces and the possible attack spaces, consider only those spaces */
		/* If there is no overlap, well need to determine the nearest node in the possible movement spaces to the nearest attack space and move there */
		if(movementRangeInAttackRange)
		{
			/* Find the list of nodes that are in range of the enemy, the current movement range, and are the nearest to the current position */
			int attackDistance = 0;
			foreach(GameObject node in movementAndAttackNodes)
			{
				Cell nodeCell = node.GetComponent<Cell>();
				int attackSpaceScore = nodeCell.attackSpaceScore;
				
				if(attackSpaceScore > attackDistance)
				{
					attackDistance = attackSpaceScore;
					space = node;
				}
			}
		}
		else
		{
			/* Get list of nodes that are in range of the enemy and have already had a path assigned to them. */
			List<GameObject> nodesInRangeOfEnemyWithPath = new List<GameObject>();
			foreach(GameObject node in cellsInRangeOfEnemy)
			{
				Cell nodeCell = node.GetComponent<Cell>();
				
				if(nodeCell.pathScore > 0)
				{
					nodesInRangeOfEnemyWithPath.Add (node);
				}
			}
			
			/* If there already is a path to a node within attack range of the enemy, search that node path for the nearest node to move to. */
			if(nodesInRangeOfEnemyWithPath.Count > 0)
			{
				int attackDistance = 10000;
				/* Find nodes with smallest attack distance that is in the movement range and move to that node */
				foreach(GameObject node in movementNodes)
				{
					Cell nodeCell = node.GetComponent<Cell>();
					int attackSpaceScore = nodeCell.attackSpaceScore;
					
					if(attackSpaceScore < attackDistance)
					{
						attackDistance = attackSpaceScore;
						space = node;
					}
				}
			}
		}
		
		foundNearestSpace = true;

		return space;
	}

	public void resetNodeAttackScores()
	{
		GameObject[] nodes = GameObject.FindGameObjectsWithTag("Cell");

		foreach(GameObject node in nodes)
		{
			node.GetComponent<Cell>().attackSpaceScore = 0;
			node.GetComponent<Cell>().neighborAttackSpaceScore = 0;
		}
	}
	
	#endregion setnodeproperties
	
}
