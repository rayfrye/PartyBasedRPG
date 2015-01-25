using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class RunOutsideOfBattle : MonoBehaviour 
{
	public Transform _transform;
	public GameData _gameData;
	public SetupGrid _setupGrid;

	public GameObject targetLord;

	public bool isMoving = false;
	public bool isInteracting = false;
	public int currentNodeInPath = 0;
	public List<GameObject> path = new List<GameObject>();

	#region UI
	public GameObject Canvas;
	public GameObject DialgoueText;
	public GameObject Dialogue;


	public Font arial;
	#endregion UI

	public void getUIElements()
	{
		Canvas = GameObject.Find ("Canvas");
		Dialogue = GameObject.Find ("Dialogue");
		DialgoueText = GameObject.Find ("Dialogue Text");

		Dialogue.SetActive (false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(isInteracting)
		{
			interact();
		}
		
		if(!isMoving)
		{
			input();
		}

		if(path.Count > 0 && !isInteracting)
		{
			moveLord(getCurrentLord());
		}
	}

	public void input()
	{
		if(Input.GetKeyUp(KeyCode.E))
		{
			isInteracting = !isInteracting;

			if(isInteracting)
			{
				Dialogue.SetActive(true);
			}
			else
			{
				Dialogue.SetActive(false);
			}
		}

		if(!isInteracting)
		{
			if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
			{
				Lord tempLord = _gameData.lords[targetLord.GetComponent<LordAvatar>().lordID];
				
				int row = tempLord.currentRow;
				int col = tempLord.currentCol;

				GameObject node = GameObject.Find ("Cell" + (row) + "," + (col + 1));

				if(node)
				{
					Cell nodeCell = node.GetComponent<Cell>();

					if(!nodeCell.isInvalidSpace && !nodeCell.hasLord)
					{
						currentNodeInPath = 0;

						path.Add (node);
						isMoving = true;
					}
				}
			}

			if(!Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
			{
				Lord tempLord = _gameData.lords[targetLord.GetComponent<LordAvatar>().lordID];
				
				int row = tempLord.currentRow;
				int col = tempLord.currentCol;

				GameObject node = GameObject.Find ("Cell" + (row) + "," + (col - 1));

				if(node)
				{
					Cell nodeCell = node.GetComponent<Cell>();
					
					if(!nodeCell.isInvalidSpace && !nodeCell.hasLord)
					{
						currentNodeInPath = 0;
						
						path.Add (node);
						isMoving = true;
					}
				}
			}

			if(Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
			{
				Lord tempLord = _gameData.lords[targetLord.GetComponent<LordAvatar>().lordID];
				
				int row = tempLord.currentRow;
				int col = tempLord.currentCol;

				GameObject node = GameObject.Find ("Cell" + (row - 1) + "," + (col));

				if(node)
				{
					Cell nodeCell = node.GetComponent<Cell>();

					if(!nodeCell.isInvalidSpace && !nodeCell.hasLord)
					{
						currentNodeInPath = 0;
										
						path.Add (node);
						isMoving = true;
					}
				}
			}

			if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.D))
			{
				Lord tempLord = _gameData.lords[targetLord.GetComponent<LordAvatar>().lordID];
				
				int row = tempLord.currentRow;
				int col = tempLord.currentCol;

				GameObject node = GameObject.Find ("Cell" + (row + 1) + "," + (col));

				if(node)
				{
					Cell nodeCell = node.GetComponent<Cell>();

					if(!nodeCell.isInvalidSpace && !nodeCell.hasLord)
					{
						currentNodeInPath = 0;
						
						path.Add (node);
						isMoving = true;
					}
				}
			}
		}
	}

	public void interact()
	{
		//Dialogue.SetActive(false);
	}
	
	public void moveLord(Lord lord)
	{
		GameObject newCell = GameObject.Find ("Cell" + path[path.Count-1].GetComponent<Cell>().row + "," + path[path.Count-1].GetComponent<Cell>().col);
		GameObject oldCell = GameObject.Find ("Cell" + lord.currentRow + "," + lord.currentCol);
		
		if(newCell != oldCell)
		{
			moveAlongPath (GameObject.Find (lord.lordName), path);
			
			newCell.GetComponent<Cell>().lordID = lord.id;
			
			newCell.GetComponent<Cell>().hasLord = true;

			oldCell.GetComponent<Cell>().lordID = -1;
			oldCell.GetComponent<Cell>().hasLord = false;
		}
	}

	void moveAlongPath(GameObject player, List<GameObject> path)
	{
		if(currentNodeInPath < path.Count)
		{
			move (player,path[currentNodeInPath],2f);
			
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
			isMoving = false;
			GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol).GetComponent<Cell>().hasLord = false;
			getCurrentLord().currentRow = path[path.Count-1].GetComponent<Cell>().row;
			getCurrentLord().currentCol = path[path.Count-1].GetComponent<Cell>().col;
			GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol).GetComponent<Cell>().hasLord = true;
			path.Clear ();
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

	public Lord getCurrentLord()
	{
		return _gameData.lords[targetLord.GetComponent<LordAvatar>().lordID];
	}
}
