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
	public GenerateGameData _generateGameData;

	public GameObject targetLord;

	public bool isMoving = false;
	public bool isInteracting = false;
	public int currentNodeInPath = 0;
	public List<GameObject> path = new List<GameObject>();
	public bool disabledTiles = false;
	public int cellRowRangeToDraw = 15;
	public int cellColRangeToDraw = 25;

	#region UI
	public GameObject Canvas;
	public Text DialogueText;
	public GameObject Dialogue;


	public Font arial;
	#endregion UI

	public void getUIElements()
	{
		Canvas = GameObject.Find ("Canvas");
		Dialogue = GameObject.Find ("Dialogue");
		DialogueText = GameObject.Find ("Dialogue Text").GetComponent<Text>();

		Dialogue.SetActive (false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(!isMoving && !isInteracting)
		{
			input();
		}

		if(path.Count > 0 && !isInteracting)
		{
			moveLord(getCurrentLord(),getCurrentLordAvatar());
		}

		if(isInteracting)
		{
			interact();
		}

		if(!disabledTiles)
		{
			disableOutOfViewTiles();
		}
	}

	public void disableOutOfViewTiles()
	{
		List<GameObject> nodes = _gameData.tiles;
		
		foreach(GameObject node in nodes)
		{
			Cell nodeCell = node.GetComponent<Cell>();
			LordAvatar lordAvatar = targetLord.GetComponent<LordAvatar>();

			if(
				nodeCell.row >= lordAvatar.row + cellRowRangeToDraw 
				|| nodeCell.row <= lordAvatar.row - cellRowRangeToDraw
				|| nodeCell.col >= lordAvatar.col + cellColRangeToDraw 
				|| nodeCell.col <= lordAvatar.col - cellColRangeToDraw
			)
			{
				node.SetActive (false);
			}
			else
			{
				node.SetActive (true);
			}
		}

		disabledTiles = true;
	}

	public void input()
	{
		if(Input.GetKeyUp(KeyCode.E))
		{
			isInteracting = true;
		}

		if(!isInteracting)
		{
			if(!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))
			{
				LordAvatar tempAvatar = targetLord.GetComponent<LordAvatar>();
				setFacingDir(tempAvatar,"east");
				Lord tempLord = _gameData.lords[tempAvatar.lordID];
				
				int row = tempAvatar.row;
				int col = tempAvatar.col;

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
				LordAvatar tempAvatar = targetLord.GetComponent<LordAvatar>();
				setFacingDir(tempAvatar,"west");
				Lord tempLord = _gameData.lords[tempAvatar.lordID];

				int row = tempAvatar.row;
				int col = tempAvatar.col;

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
				LordAvatar tempAvatar = targetLord.GetComponent<LordAvatar>();
				setFacingDir(tempAvatar,"north");
				Lord tempLord = _gameData.lords[tempAvatar.lordID];

				int row = tempAvatar.row;
				int col = tempAvatar.col;

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
				LordAvatar tempAvatar = targetLord.GetComponent<LordAvatar>();
				setFacingDir(tempAvatar,"south");
				Lord tempLord = _gameData.lords[tempAvatar.lordID];

				int row = tempAvatar.row;
				int col = tempAvatar.col;

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

	public void setFacingDir(LordAvatar lordAvatar, string dir)
	{
		lordAvatar.facingDir = dir;
		lordAvatar.transform.FindChild(lordAvatar.name + "_body").transform.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + dir + "/anims/human_body_walk_" + dir + "_cont") as RuntimeAnimatorController;
		lordAvatar.transform.FindChild(lordAvatar.name + "_skin").transform.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + dir + "/anims/human_skin_walk_" + dir + "_cont") as RuntimeAnimatorController;
		lordAvatar.transform.FindChild(lordAvatar.name + "_hair").transform.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + dir + "/anims/human_hair_walk_" + dir + "_cont") as RuntimeAnimatorController;
		lordAvatar.transform.FindChild(lordAvatar.name + "_eyes").transform.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + dir + "/anims/human_eyes_walk_" + dir + "_cont") as RuntimeAnimatorController;
		lordAvatar.transform.FindChild(lordAvatar.name + "_shirt").transform.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + dir + "/anims/human_shirt_walk_" + dir + "_cont") as RuntimeAnimatorController;
		lordAvatar.transform.FindChild(lordAvatar.name + "_pants").transform.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + dir + "/anims/human_pants_walk_" + dir + "_cont") as RuntimeAnimatorController;
		lordAvatar.transform.FindChild(lordAvatar.name + "_shoes").transform.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + dir + "/anims/human_shoes_walk_" + dir + "_cont") as RuntimeAnimatorController;
		
	}

	public void closeDialogue()
	{
		Dialogue.SetActive(false);
		isInteracting = false;
	}

	public void interact()
	{
		LordAvatar tempAvatar = targetLord.GetComponent<LordAvatar>();
		int row = tempAvatar.row;
		int col = tempAvatar.col;

		switch(tempAvatar.facingDir)
		{
		case "north":
		{
			GameObject node = GameObject.Find ("Cell" + (row - 1) + "," + (col));

			if(node)
			{
				Cell nodeCell = node.GetComponent<Cell>();

				interactWithCell (nodeCell, "north", "south");
			}

			break;
		}
		case "west":
		{
			GameObject node = GameObject.Find ("Cell" + (row) + "," + (col - 1));
			
			if(node)
			{
				Cell nodeCell = node.GetComponent<Cell>();
				
				interactWithCell (nodeCell, "west", "east");
			}
			
			break;
		}
		case "south":
		{
			GameObject node = GameObject.Find ("Cell" + (row + 1) + "," + (col));
			
			if(node)
			{
				Cell nodeCell = node.GetComponent<Cell>();
				
				interactWithCell (nodeCell, "south", "north");
			}
			
			break;
		}
		case "east":
		{
			GameObject node = GameObject.Find ("Cell" + (row) + "," + (col + 1));
			
			if(node)
			{
				Cell nodeCell = node.GetComponent<Cell>();
				
				interactWithCell (nodeCell, "east", "west");
			}
			
			break;
		}
		default:
		{

			break;
		}
		}
	}

	public void interactWithCell(Cell nodeCell, string dir, string oppdir)
	{
		if(!nodeCell.isInvalidSpace && !nodeCell.hasLord)
		{
			closeDialogue();
		}

		if(nodeCell.isInvalidSpace)
		{
			if(nodeCell.descText.Length > 0)
			{
				DialogueText.text = nodeCell.descText;
				Dialogue.SetActive(true);
				if(Input.GetKeyUp (KeyCode.R))
				{
					closeDialogue();
				}
			}
			else
			{
				closeDialogue();
			}
		}

		if(nodeCell.hasLord)
		{
			LordAvatar lordInteractingWith = GameObject.Find (nodeCell.lordID.ToString ()).GetComponent<LordAvatar>();
			setFacingDir(lordInteractingWith,oppdir);
			Dialogue d = _gameData.dialogues[_gameData.lords[nodeCell.lordID].dialogueID];

			DialogueText.text = d.mainText;
			Dialogue.SetActive(true);

			if(Input.GetKeyUp(KeyCode.R))
			{
				dialogueAction (d.option1Action);
			}
		}
	}

	public void dialogueAction(string action)
	{
		if(action.Contains("CloseDialogue"))
		{
			closeDialogue();
		}
	}

	public void moveLord(Lord lord, LordAvatar lordAvatar)
	{
		GameObject newCell = GameObject.Find ("Cell" + path[path.Count-1].GetComponent<Cell>().row + "," + path[path.Count-1].GetComponent<Cell>().col);
		GameObject oldCell = GameObject.Find ("Cell" + lordAvatar.row + "," + lordAvatar.col);
		
		if(newCell != oldCell)
		{
			moveAlongPath (GameObject.Find (lord.id.ToString ()), path, getCurrentLordAvatar());
			
			newCell.GetComponent<Cell>().lordID = lord.id;
			
			newCell.GetComponent<Cell>().hasLord = true;

			oldCell.GetComponent<Cell>().lordID = -1;
			oldCell.GetComponent<Cell>().hasLord = false;
		}
	}

	public string getString(string s, string prefix)
	{
		string value = "";
		
		if(s.Length > 0 && s.Contains (prefix))
		{
			int prefixIndexStart = s.IndexOf(prefix)+prefix.Length + 1;
			
			string temp = s.Substring(prefixIndexStart);
			
			int prefixIndexEnd = temp.IndexOf("|");
			
			value = temp.Substring(0,prefixIndexEnd);
		}
		
		return value;
	}

	void moveAlongPath(GameObject player, List<GameObject> path, LordAvatar lordAvatar)
	{
		if(currentNodeInPath < path.Count)
		{
			move (player,path[currentNodeInPath],5f);
			
			float xDistance = Mathf.RoundToInt(Mathf.Abs(player.GetComponent<RectTransform>().localPosition.x - path[currentNodeInPath].GetComponent<RectTransform>().localPosition.x));
			float yDistance = Mathf.RoundToInt(Mathf.Abs(player.GetComponent<RectTransform>().localPosition.y - path[currentNodeInPath].GetComponent<RectTransform>().localPosition.y));

			if(xDistance <= 1 && yDistance <= 1)
			{
				if(path[currentNodeInPath].GetComponent<Cell>().isDoor)
				{
					_generateGameData.levelToLoad = path[currentNodeInPath].GetComponent<Cell>().doorDest+".csv";
					_generateGameData.doorNum = path[currentNodeInPath].GetComponent<Cell>().doorDestID;
					Application.LoadLevel("Test Town");
					_generateGameData.loadLevel = true;
				}

				player.GetComponent<RectTransform>().localPosition = path[currentNodeInPath].GetComponent<RectTransform>().localPosition;
				lordAvatar.row = path[currentNodeInPath].GetComponent<Cell>().row;
				lordAvatar.col = path[currentNodeInPath].GetComponent<Cell>().col;
				player.rigidbody2D.velocity = new Vector2(0,0);
				currentNodeInPath++;
			}
		}
		
		if(currentNodeInPath == path.Count)
		{
			isMoving = false;
			GameObject.Find ("Cell"+lordAvatar.row+","+lordAvatar.col).GetComponent<Cell>().hasLord = false;
			lordAvatar.row = path[path.Count-1].GetComponent<Cell>().row;
			lordAvatar.col = path[path.Count-1].GetComponent<Cell>().col;
			GameObject.Find ("Cell"+lordAvatar.row+","+lordAvatar.col).GetComponent<Cell>().hasLord = true;
			path.Clear ();
			disabledTiles = false;
		}
	}

	void move(GameObject player, GameObject destination, float movementSpeed)
	{
		bool isHorizontalMovement;
		Lord playerLord = getCurrentLord();
		LordAvatar lordAvatar = getCurrentLordAvatar();
		
		if(lordAvatar.row == destination.GetComponent<Cell>().row)
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

			float distance = Mathf.Abs (player.GetComponent<RectTransform>().localPosition.x - destination.GetComponent<RectTransform>().localPosition.x);
			float speed = Mathf.Min (movementSpeed,movementSpeed * (distance/8));
			
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
				player.rigidbody2D.velocity = new Vector2(-speed,0);
			}
			else
			{
				player.rigidbody2D.velocity = new Vector2(speed,0);
			}
		}
		else
		{
			bool isUp;

			float distance = Mathf.Abs (player.GetComponent<RectTransform>().localPosition.y - destination.GetComponent<RectTransform>().localPosition.y);
			float speed = Mathf.Min (movementSpeed,movementSpeed * (distance/8));

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
				player.rigidbody2D.velocity = new Vector2(0,speed);
			}
			else
			{
				player.rigidbody2D.velocity = new Vector2(0,-speed);
			}
		}
	}
	
	public Lord getCurrentLord()
	{
		return _gameData.lords[targetLord.GetComponent<LordAvatar>().lordID];
	}

	public LordAvatar getCurrentLordAvatar()
	{
		return targetLord.GetComponent<LordAvatar>();
	}
}
