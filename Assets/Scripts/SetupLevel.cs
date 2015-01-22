using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SetupLevel : MonoBehaviour 
{
	public int _scale;
	public GameObject player;
	bool movePlayer = false;
	List<GameObject> cellsToMoveTo;
	int currentNode = 1;
	int xDistance = 0;
	int yDistance = 0;
	public int[,] grid;

	#region components
	public Transform _transform;
	public GameData _gameData;
	public GenerateGameData _generateGameData;
	public ReadCSV _readCSV;
	#endregion components	
	
	#region UI
	public GameObject Canvas;
	public GameObject cellContainer;
	public GameObject cell;
	
	public Font arial;
	#endregion UI

	#region colors
	public Color32 c_possibleMovementSpaceBlue;
	public Color32 c_friendlyBlue;
	public Color32 c_invalidSpaceRed;
	public Color32 c_enemyRed;
	public Color32 c_currentTurnWhite;
	public Color32 c_enemyInAttackRange;
	#endregion colors

	public void getUIElements()
	{
		Canvas = GameObject.Find ("Canvas");
		cellContainer = GameObject.Find ("Cell Container");
		cell = GameObject.Find ("Cell");

		c_possibleMovementSpaceBlue = new Color32(92,131,233,150);
		c_friendlyBlue = new Color32(130,249,244,150);
		c_invalidSpaceRed = new Color32(251,126,126,150);
		c_enemyRed = new Color32(255,19,19,150);
		c_currentTurnWhite = new Color32(255,255,255,150);
		c_enemyInAttackRange = new Color32(255,168,75,150);
	}

	void Update()
	{
		if(movePlayer)
		{
			moveAlongPath(cellsToMoveTo);
		}
	}

	IEnumerator clickMove(int row, int col)
	{
		assignCellPathScores(player.GetComponent<Lord>());
		movePlayer = true;
		cellsToMoveTo = GameObject.Find ("Cell"+row+","+col).GetComponent<Cell>().path;

		yield return new WaitForSeconds(0f);
	}

	void moveAlongPath(List<GameObject> path)
	{
		if(currentNode < path.Count)
		{
			move (player,path[currentNode],1f);

			xDistance = (int) (player.GetComponent<RectTransform>().localPosition.x - path[currentNode].GetComponent<RectTransform>().localPosition.x);
			yDistance = (int) (player.GetComponent<RectTransform>().localPosition.y - path[currentNode].GetComponent<RectTransform>().localPosition.y);

			print (xDistance + " " + yDistance);

			if(xDistance == 0 && yDistance == 0)
			{
				player.GetComponent<RectTransform>().localPosition = path[currentNode].GetComponent<RectTransform>().localPosition;
				player.GetComponent<Lord>().currentRow = path[currentNode].GetComponent<Cell>().row;
				player.GetComponent<Lord>().currentCol = path[currentNode].GetComponent<Cell>().col;
				player.rigidbody2D.velocity = new Vector2(0,0);
				currentNode++;
			}
		}
		else
		{
			currentNode = 1;
			movePlayer = false;
		}
	}

	void move(GameObject character, GameObject destination, float movementSpeed)
	{
		bool isHorizontalMovement;

		if(character.GetComponent<Lord>().currentRow == destination.GetComponent<Cell>().row)
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

			if(character.GetComponent<RectTransform>().localPosition.x > destination.GetComponent<RectTransform>().localPosition.x)
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

			if(character.GetComponent<RectTransform>().localPosition.y > destination.GetComponent<RectTransform>().localPosition.y)
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

	public void createGrid(string gridName)
	{
		string[,] level = _readCSV.readStringMultiDimArray(gridName,@".\Assets\Levels\");

		grid = new int[level.GetLength (0),level.GetLength (1)];

		for(int row = 0; row < level.GetLength (0); row++)
		{
			for(int col = 0; col < level.GetLength (1); col++)
			{
				grid[row,col] = -1;
				GameObject newSprite = new GameObject();
				newSprite.name = "Cell"+row+","+col;
				newSprite.AddComponent<CanvasRenderer>();
				newSprite.transform.parent = cellContainer.transform;
				
				SpriteRenderer newSpriteSpriteRenderer = newSprite.AddComponent<SpriteRenderer>();
				newSpriteSpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/"+level[row,col]);

				RectTransform newSpriteRectTransform = newSprite.AddComponent<RectTransform>();
				newSpriteRectTransform.localScale = new Vector3(_scale,_scale,1);

				RectTransform defaultCellRectTransform = cell.GetComponent<RectTransform>(); 
				newSpriteRectTransform.sizeDelta = new Vector3(16,16,1);
				newSpriteRectTransform.localPosition = 
				new Vector3(
					defaultCellRectTransform.localPosition.x + newSpriteRectTransform.sizeDelta.x*_scale*col
					,defaultCellRectTransform.localPosition.y - newSpriteRectTransform.sizeDelta.y*_scale*row
					,0);

				Cell newSpriteCell = newSprite.AddComponent<Cell>();
				newSpriteCell.path = new List<GameObject>();

				newSpriteCell.row = row;
				newSpriteCell.col = col;

				GameObject button = new GameObject();
				button.name = "Cell"+row+","+col+"_button";
				button.transform.parent = newSprite.transform;

				RectTransform buttonRectTransform = button.AddComponent<RectTransform>();
				buttonRectTransform.localScale = new Vector3(1,1,1);
				buttonRectTransform.sizeDelta = new Vector3(16,16,1);
				buttonRectTransform.localPosition = new Vector3(0,0,0);
				
				Image buttonImg = button.AddComponent<Image>();
				buttonImg.color = new Color32(255,255,255,50);

				Button buttonBtn = button.AddComponent<Button>();
				buttonBtn.targetGraphic = buttonImg;
				ColorBlock tempColorBlock = buttonBtn.colors;
				tempColorBlock.normalColor = new Color(255,255,255,0);
				buttonBtn.colors = tempColorBlock;

				buttonBtn.onClick.AddListener(delegate{StartCoroutine(clickMove(newSpriteCell.row,newSpriteCell.col));});
			}
		}
		{
			player = new GameObject();
			player.name = "human";
			player.transform.parent = cellContainer.transform;
			CanvasRenderer playerCanvasRenderer = player.AddComponent<CanvasRenderer>();
			SpriteRenderer playerSpriteRenderer = player.AddComponent<SpriteRenderer>();
			playerSpriteRenderer.sortingOrder = 1;

			player.AddComponent<Animator>();
			player.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human_spritesheet_walk_south") as RuntimeAnimatorController;

			RectTransform defaultCellRectTransform = cell.GetComponent<RectTransform>(); 

			RectTransform playerRectTransform = player.AddComponent<RectTransform>();
			playerRectTransform.localScale = new Vector3(_scale,_scale,1);
			playerRectTransform.localPosition = 
				new Vector3(
					defaultCellRectTransform.localPosition.x
					,defaultCellRectTransform.localPosition.y
					,0);

			Rigidbody2D playerRigidBody = player.AddComponent<Rigidbody2D>();
			playerRigidBody.gravityScale = 0;

			Lord playerLord = player.AddComponent<Lord>();
			playerLord.currentRow = 0;
			playerLord.currentCol = 0;
		}
	}

	public List<GameObject> assignCellPathScores(Lord lord)
	{
		List<GameObject> nodesToCheck = new List<GameObject>();
		List<GameObject> nodesInMovementRange = new List<GameObject>();
		List<GameObject> newNodesToCheck = new List<GameObject>();
		List<GameObject> nodesAlreadyChecked = new List<GameObject>();
		int movementRange = 50;
		
		GameObject origin = GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol));
		Cell originCell = origin.GetComponent<Cell>();
		
		originCell.pathScore = 0;
		//origin.GetComponent<Image>().color = c_currentTurnWhite;
		nodesAlreadyChecked.Add (origin);
		
		nodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow - 1) + "," + (lord.currentCol)));
		nodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow + 1) + "," + (lord.currentCol)));
		nodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol - 1)));
		nodesToCheck.Add (GameObject.Find ("Cell" + (lord.currentRow) + "," + (lord.currentCol + 1)));
		
		foreach(GameObject nodeToCheck in nodesToCheck)
		{
			if(nodeToCheck)
			{
				Cell nodeToCheckCell = nodeToCheck.GetComponent<Cell>();

				if(!nodeToCheckCell.isInvalidSpace && (!nodeToCheckCell.hasLord || grid[nodeToCheckCell.row,nodeToCheckCell.col] == grid[lord.currentRow,lord.currentCol]))
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
				
				if(!nodeToCheckCell.isInvalidSpace 
				   && (!nodeToCheckCell.hasLord || grid[nodeToCheckCell.row,nodeToCheckCell.col] == grid[lord.currentRow,lord.currentCol])
				   )
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


}
