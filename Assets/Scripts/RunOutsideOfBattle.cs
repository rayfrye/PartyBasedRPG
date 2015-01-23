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
	public int currentNodeInPath = 0;
	public List<GameObject> path = new List<GameObject>();
//a	public 
	
	// Update is called once per frame
	void Update () 
	{
		if(isMoving)
		{
			moveLord(getCurrentLord(),3,4);
		}
		else
		{
			directionalInput();
		}
	}

	public int[] directionalInput()
	{
		int[] dest = new int[2];

		if(Input.GetKey(KeyCode.W))
		{
			path.Add (GameObject.Find ("Cell" + 3 + "," + 4));
		}

		isMoving = true;

		return dest;
	}

	public void moveLord(Lord lord, int newRow, int newCol)
	{
		GameObject newCell = GameObject.Find ("Cell" + newRow + "," + newCol);
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
			GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol).GetComponent<Cell>().hasLord = false;
			getCurrentLord().currentRow = path[path.Count-1].GetComponent<Cell>().row;
			getCurrentLord().currentCol = path[path.Count-1].GetComponent<Cell>().col;
			GameObject.Find ("Cell"+getCurrentLord().currentRow+","+getCurrentLord().currentCol).GetComponent<Cell>().hasLord = true;
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
