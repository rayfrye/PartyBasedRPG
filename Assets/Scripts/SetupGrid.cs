using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SetupGrid : MonoBehaviour 
{
	public Transform _transform;
	public GameData _gameData;
	public ReadCSV _readCSV;
	public RunBattle _runBattle;

	public int diceSides;

	public int factionIDSide1;
	public int factionIDSide2;

	public int objectScale;
	public int cellSize;

	public GameObject targetLord;

	public int[,] grid = new int[0,0];

	#region UI
	public GameObject Canvas;
	public GameObject cell;
	public GameObject cellContainer;
	public GameObject lordContainer;
	public GameObject endTurnButton;

	
	
	public Font arial;
	#endregion UI

	#region Colors
	public Color32[] colors;
	#endregion Colors

	public void getUIElements()
	{
		Canvas = GameObject.Find ("Canvas");
		cell = GameObject.Find ("Cell");
		cellContainer = GameObject.Find ("Cell Container");
		lordContainer = GameObject.Find ("Lord Container");
		endTurnButton = GameObject.Find ("End Turn Button");
	}

	public void makeGrid(int targetLordID, int doorID, string gridName)
	{
		_gameData.tiles.Clear ();

		string[,] level = _readCSV.readStringMultiDimArray(gridName,@".\Assets\Levels\");
		
		grid = new int[level.GetLength (0),level.GetLength (1)];
		
		for(int row = 0; row < level.GetLength (0); row++)
		{
			for(int col = 0; col < level.GetLength (1); col++)
			{
				grid[row,col] = -1;
				GameObject newSprite = new GameObject();
				newSprite.name = "Cell"+row+","+col;
				newSprite.tag = "Cell";
				newSprite.AddComponent<CanvasRenderer>();
				newSprite.transform.parent = cellContainer.transform;

				SpriteRenderer newSpriteSpriteRenderer = newSprite.AddComponent<SpriteRenderer>();
				newSpriteSpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/floors/"+getCellValue(level[row,col],"bg"));
				//newSpriteSpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/floors/floor_wood");
				newSpriteSpriteRenderer.sortingOrder = -10;

				RectTransform newSpriteRectTransform = newSprite.AddComponent<RectTransform>();
				newSpriteRectTransform.localScale = new Vector3(objectScale,objectScale,1);
				
				RectTransform defaultCellRectTransform = cell.GetComponent<RectTransform>(); 
				newSpriteRectTransform.sizeDelta = new Vector3(cellSize,cellSize,1);
				newSpriteRectTransform.localPosition = 
					new Vector3(
						defaultCellRectTransform.localPosition.x + newSpriteRectTransform.sizeDelta.x*objectScale*col
						,defaultCellRectTransform.localPosition.y - newSpriteRectTransform.sizeDelta.y*objectScale*row
						,0);

				Cell newSpriteCell = newSprite.AddComponent<Cell>();
				newSpriteCell.path = new List<GameObject>();

				if(getCellValue(level[row,col],"isInvalidSpace") == "true")
				{
					newSpriteCell.isInvalidSpace = true;
				}

				if(level[row,col].Contains ("text"))
				{
					newSpriteCell.descText = getCellValue(level[row,col],"text");
				}
				else
				{
					newSpriteCell.descText = "";
				}

				if(level[row,col].Contains ("doorDest"))
				{
					newSpriteCell.doorDest = getCellValue(level[row,col],"doorDest");
					newSpriteCell.isDoor = true;

					if(level[row,col].Contains ("doorDestID"))
					{
						newSpriteCell.doorDestID = int.Parse (getCellValue(level[row,col],"doorDestID"));
					}
					else
					{
						newSpriteCell.doorDestID = 1;
					}
				}
				else
				{
					newSpriteCell.doorDest = "";
					newSpriteCell.isDoor = false;
				}

				if(level[row,col].Contains ("obj1"))
				{
					GameObject obj = new GameObject();
					obj.transform.parent = newSprite.transform;
					SpriteRenderer objSpriteRenderer = obj.AddComponent<SpriteRenderer>();
					objSpriteRenderer.sortingOrder = -9;

					if(level[row,col].Contains ("objIndex1"))
					{
						int index = int.Parse (getCellValue(level[row,col],"objIndex1"));
						objSpriteRenderer.sprite = Resources.LoadAll<Sprite>("Sprites/floors/"+getCellValue(level[row,col],"obj1"))[index];
						obj.name = getCellValue(level[row,col],"obj1") + "_" + index;

						if(level[row,col].Contains ("obj1FillColor"))
						{
							int colorID = int.Parse (getCellValue(level[row,col],"obj1FillColor"));
							objSpriteRenderer.color = colors[colorID];
						}
					}
					else
					{
						objSpriteRenderer.sprite = Resources.Load<Sprite>("Sprites/floors/"+getCellValue(level[row,col],"obj1"));
						obj.name = getCellValue(level[row,col],"obj1");

						if(level[row,col].Contains ("obj1FillColor"))
						{
							int colorID = int.Parse (getCellValue(level[row,col],"obj1FillColor"));
							objSpriteRenderer.color = colors[colorID];
						}
					}

					RectTransform objRectTransform = obj.AddComponent<RectTransform>();
					objRectTransform.localScale = new Vector3(1,1,1);
					objRectTransform.sizeDelta = new Vector3(cellSize,cellSize,1);
					objRectTransform.localPosition = new Vector3(0,0,0);
				}

				if(level[row,col].Contains ("spawn"))
				{
					if(doorID == int.Parse (getCellValue(level[row,col],"spawn")))
					{
						targetLord = putLordOnGrid(targetLordID,row,col,"south",true);
					}
				}

				if(level[row,col].Contains ("lord"))
				{
					int lordID = int.Parse (getCellValue(level[row,col],"lord"));
					putLordOnGrid(lordID,row,col,"south",false);

					if(level[row,col].Contains ("lord"))
					{
						_gameData.lords[lordID].dialogueID = int.Parse (getCellValue(level[row,col],"dialogue"));
					}
				}
				else
				{
					newSpriteCell.lordID = -1;
				}

				newSpriteCell.row = row;
				newSpriteCell.col = col;

				GameObject button = new GameObject();
				button.name = "Cell"+row+","+col+"_button";
				button.transform.parent = newSprite.transform;
				
				RectTransform buttonRectTransform = button.AddComponent<RectTransform>();
				buttonRectTransform.localScale = new Vector3(1,1,1);
				buttonRectTransform.sizeDelta = new Vector3(cellSize,cellSize,1);
				buttonRectTransform.localPosition = new Vector3(0,0,0);
				
				Image buttonImg = button.AddComponent<Image>();
				buttonImg.color = new Color32(255,255,255,0);
				//buttonImg.color = new Color32(255,255,255,50);
				
				Button buttonBtn = button.AddComponent<Button>();
				buttonBtn.targetGraphic = buttonImg;
				ColorBlock tempColorBlock = buttonBtn.colors;
				tempColorBlock.normalColor = new Color(255,255,255,0);
				buttonBtn.colors = tempColorBlock;
				
				buttonBtn.onClick.AddListener(delegate{StartCoroutine(handleClick(newSpriteCell.row,newSpriteCell.col));});

				_gameData.tiles.Add (newSprite);
			}
		}

		GameObject.Destroy (cell);
	}

	public string getCellValue(string cellString, string cellType)
	{
		string s = "";

		if(cellString.Length > 0 && cellString.Contains (cellType))
		{
			int prefixIndexStart = cellString.IndexOf(cellType)+cellType.Length + 1;

			string temp = cellString.Substring(prefixIndexStart);

			int prefixIndexEnd = temp.IndexOf("|");

			s = temp.Substring(0,prefixIndexEnd);
		}

		return s;
	}

	public void putFactionsOnGrid(int factionID1, int factionID2, int rows, int cols)
	{
		factionIDSide1 = factionID1;
		factionIDSide2 = factionID2;

		grid[4,2] = _gameData.factions[factionID1].positions[3];
		grid[5,2] = _gameData.factions[factionID1].positions[4];
		grid[6,2] = _gameData.factions[factionID1].positions[5];
		grid[4,3] = _gameData.factions[factionID1].positions[0];
		grid[5,3] = _gameData.factions[factionID1].positions[1];
		grid[6,3] = _gameData.factions[factionID1].positions[2];
		
		grid[4,9] = _gameData.factions[factionID2].positions[3];
		grid[5,9] = _gameData.factions[factionID2].positions[4];
		grid[6,9] = _gameData.factions[factionID2].positions[5];
		grid[4,8] = _gameData.factions[factionID2].positions[0];
		grid[5,8] = _gameData.factions[factionID2].positions[1];
		grid[6,8] = _gameData.factions[factionID2].positions[2];
		
		for(int col = 0; col < grid.GetLength (1); col++)
		{
			for(int row = 0; row < grid.GetLength (0); row++)
			{
				if(grid[row,col] != -1)
				{
					GameObject newPlayerBase = new GameObject();
					GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");

					newPlayerBase.transform.parent = cellContainer.transform;
					newPlayerBase.name = _gameData.lords[grid[row,col]].id.ToString ();

					camera.transform.parent = newPlayerBase.transform;
					camera.GetComponent<RectTransform>().localPosition = new Vector3(0,0,-10);
					camera.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);

					LordAvatar newPlayerBaseLordAvatar = newPlayerBase.AddComponent<LordAvatar>();
					newPlayerBaseLordAvatar.lordID = grid[row,col];


					RectTransform defaultCellRectTransform = GameObject.Find ("Cell"+row+","+col).GetComponent<RectTransform>(); 

					RectTransform playerRectTransform = newPlayerBase.AddComponent<RectTransform>();
					playerRectTransform.localScale = new Vector3(1,1,1);
					playerRectTransform.localPosition = 
						new Vector3(
							defaultCellRectTransform.localPosition.x
							,defaultCellRectTransform.localPosition.y
							,0);

					Rigidbody2D playerRigidBody = newPlayerBase.AddComponent<Rigidbody2D>();
					playerRigidBody.gravityScale = 0;

					newPlayerBaseLordAvatar.row = row;
					newPlayerBaseLordAvatar.col = col;

					GameObject.Find ("Cell" + row + "," + col).GetComponent<Cell>().hasLord = true;
					GameObject.Find ("Cell" + row + "," + col).GetComponent<Cell>().lordID = grid[row,col];


					GameObject newPlayerBody = new GameObject();
					newPlayerBody.name = newPlayerBase.name + "_body";
					newPlayerBody.transform.parent = newPlayerBase.transform;
					
					CanvasRenderer _canvasRenderer = newPlayerBody.AddComponent<CanvasRenderer>();
					SpriteRenderer _spriteRenderer = newPlayerBody.AddComponent<SpriteRenderer>();
					_spriteRenderer.sortingOrder = 1;
					_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
					
					newPlayerBody.AddComponent<Animator>();
					newPlayerBody.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/east/anims/human_body_walk_east_cont") as RuntimeAnimatorController;
					
					playerRectTransform = newPlayerBody.AddComponent<RectTransform>();
					playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
					playerRectTransform.localPosition = new Vector3(0,0,0);


					GameObject newPlayerSkin = new GameObject();
					newPlayerSkin.name = newPlayerBase.name + "_skin";
					newPlayerSkin.transform.parent = newPlayerBase.transform;
					
					_canvasRenderer = newPlayerSkin.AddComponent<CanvasRenderer>();
					_spriteRenderer = newPlayerSkin.AddComponent<SpriteRenderer>();
					_spriteRenderer.sortingOrder = 1;
					_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];

					newPlayerSkin.AddComponent<Animator>();
					newPlayerSkin.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/east/anims/human_skin_walk_east_cont") as RuntimeAnimatorController;

					playerRectTransform = newPlayerSkin.AddComponent<RectTransform>();
					playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
					playerRectTransform.localPosition = new Vector3(0,0,0);


					GameObject newPlayerHair = new GameObject();
					newPlayerHair.name = newPlayerBase.name + "_hair";
					newPlayerHair.transform.parent = newPlayerBase.transform;
					
					_canvasRenderer = newPlayerHair.AddComponent<CanvasRenderer>();
					_spriteRenderer = newPlayerHair.AddComponent<SpriteRenderer>();
					_spriteRenderer.sortingOrder = 3;
					_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];

					newPlayerHair.AddComponent<Animator>();
					newPlayerHair.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/east/anims/human_hair_walk_east_cont") as RuntimeAnimatorController;

					playerRectTransform = newPlayerHair.AddComponent<RectTransform>();
					playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
					playerRectTransform.localPosition = new Vector3(0,0,0);


					GameObject newPlayerEyes = new GameObject();
					newPlayerEyes.name = newPlayerBase.name + "_eyes";
					newPlayerEyes.transform.parent = newPlayerBase.transform;
					
					_canvasRenderer = newPlayerEyes.AddComponent<CanvasRenderer>();
					_spriteRenderer = newPlayerEyes.AddComponent<SpriteRenderer>();
					_spriteRenderer.sortingOrder = 2;
					_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];

					newPlayerEyes.AddComponent<Animator>();
					newPlayerEyes.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/east/anims/human_eyes_walk_east_cont") as RuntimeAnimatorController;

					playerRectTransform = newPlayerEyes.AddComponent<RectTransform>();
					playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
					playerRectTransform.localPosition = new Vector3(0,0,0);


					GameObject newPlayerShirt = new GameObject();
					newPlayerShirt.name = newPlayerBase.name + "_shirt";
					newPlayerShirt.transform.parent = newPlayerBase.transform;
					
					_canvasRenderer = newPlayerShirt.AddComponent<CanvasRenderer>();
					_spriteRenderer = newPlayerShirt.AddComponent<SpriteRenderer>();
					_spriteRenderer.sortingOrder = 2;
					_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
					
					newPlayerShirt.AddComponent<Animator>();
					newPlayerShirt.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/east/anims/human_shirt_walk_east_cont") as RuntimeAnimatorController;
					
					playerRectTransform = newPlayerShirt.AddComponent<RectTransform>();
					playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
					playerRectTransform.localPosition = new Vector3(0,0,0);


					GameObject newPlayerPants = new GameObject();
					newPlayerPants.name = newPlayerBase.name + "_pants";
					newPlayerPants.transform.parent = newPlayerBase.transform;
					
					_canvasRenderer = newPlayerPants.AddComponent<CanvasRenderer>();
					_spriteRenderer = newPlayerPants.AddComponent<SpriteRenderer>();
					_spriteRenderer.sortingOrder = 2;
					_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
					
					newPlayerPants.AddComponent<Animator>();
					newPlayerPants.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/east/anims/human_pants_walk_east_cont") as RuntimeAnimatorController;



					playerRectTransform = newPlayerPants.AddComponent<RectTransform>();
					playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
					playerRectTransform.localPosition = new Vector3(0,0,0);
					
					
					GameObject newPlayerShoes = new GameObject();
					newPlayerShoes.name = newPlayerBase.name + "_shoes";
					newPlayerShoes.transform.parent = newPlayerBase.transform;
					
					_canvasRenderer = newPlayerShoes.AddComponent<CanvasRenderer>();
					_spriteRenderer = newPlayerShoes.AddComponent<SpriteRenderer>();
					_spriteRenderer.sortingOrder = 2;
					_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
					
					newPlayerShoes.AddComponent<Animator>();
					newPlayerShoes.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/east/anims/human_shoes_walk_east_cont") as RuntimeAnimatorController;

					playerRectTransform = newPlayerShoes.AddComponent<RectTransform>();
					playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
					playerRectTransform.localPosition = new Vector3(0,0,0);
				}
			}
		}
	}

	public GameObject putLordOnGrid(int lordID, int row, int col, string facingDir, bool isTarget)
	{
		GameObject newPlayerBase = new GameObject();
//		camera.transform.SetParent(newPlayerBase.transform);
//		camera.transform.parent = newPlayerBase.transform;
//		camera.GetComponent<RectTransform>().localPosition = new Vector3(0,0,-10);
//		camera.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);

		newPlayerBase.transform.parent = lordContainer.transform;
		newPlayerBase.name = _gameData.lords[lordID].id.ToString ();
		LordAvatar newPlayerBaseLordAvatar = newPlayerBase.AddComponent<LordAvatar>();
		newPlayerBaseLordAvatar.lordID = lordID;
		newPlayerBaseLordAvatar.facingDir = facingDir;
		GameObject.Find ("Cell"+row+","+col).GetComponent<Cell>().hasLord = true;
		GameObject.Find ("Cell"+row+","+col).GetComponent<Cell>().lordID = lordID;
		
		RectTransform defaultCellRectTransform = GameObject.Find ("Cell"+row+","+col).GetComponent<RectTransform>(); 
		
		RectTransform playerRectTransform = newPlayerBase.AddComponent<RectTransform>();
		playerRectTransform.localScale = new Vector3(1,1,1);
		playerRectTransform.localPosition = 
			new Vector3(
				defaultCellRectTransform.localPosition.x
				,defaultCellRectTransform.localPosition.y
				,0);
		
		Rigidbody2D playerRigidBody = newPlayerBase.AddComponent<Rigidbody2D>();
		playerRigidBody.gravityScale = 0;
		
		newPlayerBaseLordAvatar.row = row;
		newPlayerBaseLordAvatar.col = col;
		
		GameObject.Find ("Cell" + row + "," + col).GetComponent<Cell>().hasLord = true;
		GameObject.Find ("Cell" + row + "," + col).GetComponent<Cell>().lordID = lordID;
		
		
		GameObject newPlayerBody = new GameObject();
		newPlayerBody.name = newPlayerBase.name + "_body";
		newPlayerBody.transform.parent = newPlayerBase.transform;
		
		CanvasRenderer _canvasRenderer = newPlayerBody.AddComponent<CanvasRenderer>();
		SpriteRenderer _spriteRenderer = newPlayerBody.AddComponent<SpriteRenderer>();
		_spriteRenderer.sortingOrder = -5;
		_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
		
		newPlayerBody.AddComponent<Animator>();
		newPlayerBody.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + facingDir + "/anims/human_body_walk_" + facingDir + "_cont") as RuntimeAnimatorController;
		
		playerRectTransform = newPlayerBody.AddComponent<RectTransform>();
		playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
		playerRectTransform.localPosition = new Vector3(0,0,0);
		
		
		GameObject newPlayerSkin = new GameObject();
		newPlayerSkin.name = newPlayerBase.name + "_skin";
		newPlayerSkin.transform.parent = newPlayerBase.transform;
		
		_canvasRenderer = newPlayerSkin.AddComponent<CanvasRenderer>();
		_spriteRenderer = newPlayerSkin.AddComponent<SpriteRenderer>();
		_spriteRenderer.sortingOrder = 1;
		_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
		
		newPlayerSkin.AddComponent<Animator>();
		newPlayerSkin.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + facingDir + "/anims/human_skin_walk_" + facingDir + "_cont") as RuntimeAnimatorController;
		
		playerRectTransform = newPlayerSkin.AddComponent<RectTransform>();
		playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
		playerRectTransform.localPosition = new Vector3(0,0,0);
		
		
		GameObject newPlayerHair = new GameObject();
		newPlayerHair.name = newPlayerBase.name + "_hair";
		newPlayerHair.transform.parent = newPlayerBase.transform;
		
		_canvasRenderer = newPlayerHair.AddComponent<CanvasRenderer>();
		_spriteRenderer = newPlayerHair.AddComponent<SpriteRenderer>();
		_spriteRenderer.sortingOrder = 3;
		_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
		
		newPlayerHair.AddComponent<Animator>();
		newPlayerHair.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + facingDir + "/anims/human_hair_walk_" + facingDir + "_cont") as RuntimeAnimatorController;
		
		playerRectTransform = newPlayerHair.AddComponent<RectTransform>();
		playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
		playerRectTransform.localPosition = new Vector3(0,0,0);
		
		
		GameObject newPlayerEyes = new GameObject();
		newPlayerEyes.name = newPlayerBase.name + "_eyes";
		newPlayerEyes.transform.parent = newPlayerBase.transform;
		
		_canvasRenderer = newPlayerEyes.AddComponent<CanvasRenderer>();
		_spriteRenderer = newPlayerEyes.AddComponent<SpriteRenderer>();
		_spriteRenderer.sortingOrder = 2;
		_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
		
		newPlayerEyes.AddComponent<Animator>();
		newPlayerEyes.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + facingDir + "/anims/human_eyes_walk_" + facingDir + "_cont") as RuntimeAnimatorController;
		
		playerRectTransform = newPlayerEyes.AddComponent<RectTransform>();
		playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
		playerRectTransform.localPosition = new Vector3(0,0,0);
		
		
		GameObject newPlayerShirt = new GameObject();
		newPlayerShirt.name = newPlayerBase.name + "_shirt";
		newPlayerShirt.transform.parent = newPlayerBase.transform;
		
		_canvasRenderer = newPlayerShirt.AddComponent<CanvasRenderer>();
		_spriteRenderer = newPlayerShirt.AddComponent<SpriteRenderer>();
		_spriteRenderer.sortingOrder = 2;
		_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
		
		newPlayerShirt.AddComponent<Animator>();
		newPlayerShirt.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + facingDir + "/anims/human_shirt_walk_" + facingDir + "_cont") as RuntimeAnimatorController;
		
		playerRectTransform = newPlayerShirt.AddComponent<RectTransform>();
		playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
		playerRectTransform.localPosition = new Vector3(0,0,0);
		
		
		GameObject newPlayerPants = new GameObject();
		newPlayerPants.name = newPlayerBase.name + "_pants";
		newPlayerPants.transform.parent = newPlayerBase.transform;
		
		_canvasRenderer = newPlayerPants.AddComponent<CanvasRenderer>();
		_spriteRenderer = newPlayerPants.AddComponent<SpriteRenderer>();
		_spriteRenderer.sortingOrder = 2;
		_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
		
		newPlayerPants.AddComponent<Animator>();
		newPlayerPants.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + facingDir + "/anims/human_pants_walk_" + facingDir + "_cont") as RuntimeAnimatorController;
		
		
		
		playerRectTransform = newPlayerPants.AddComponent<RectTransform>();
		playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
		playerRectTransform.localPosition = new Vector3(0,0,0);
		
		
		GameObject newPlayerShoes = new GameObject();
		newPlayerShoes.name = newPlayerBase.name + "_shoes";
		newPlayerShoes.transform.parent = newPlayerBase.transform;
		
		_canvasRenderer = newPlayerShoes.AddComponent<CanvasRenderer>();
		_spriteRenderer = newPlayerShoes.AddComponent<SpriteRenderer>();
		_spriteRenderer.sortingOrder = 2;
		_spriteRenderer.color = colors[UnityEngine.Random.Range (0,colors.Length)];
		
		newPlayerShoes.AddComponent<Animator>();
		newPlayerShoes.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Sprites/human/" + facingDir + "/anims/human_shoes_walk_" + facingDir + "_cont") as RuntimeAnimatorController;
		
		playerRectTransform = newPlayerShoes.AddComponent<RectTransform>();
		playerRectTransform.localScale = new Vector3(objectScale,objectScale,1);
		playerRectTransform.localPosition = new Vector3(0,0,0);

		if(isTarget)
		{
			GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
			camera.GetComponent<CameraController>().target = newPlayerBase.transform;
		}

		return newPlayerBase;
	}
	
	public IEnumerator handleClick(int row, int col)
	{
		float waitTime = 0f;

		if(_runBattle)
		{
			StartCoroutine(_runBattle.handleClick(row,col));
		}

		yield return new WaitForSeconds(waitTime);
	}
	
	public List<int> getBattleOrder(int factionID1, int factionID2)
	{
		List<int> l = new List<int>();
		List<int> tempLordList = new List<int>();
		Dictionary<int,int> unsortedLordIDs = new Dictionary<int, int>();
		
		Dictionary<int, int> temprolls = possibleRolls ();
		
		for(int i = 0; i < _gameData.factions[factionID1].positions.Length; i++)
		{
			tempLordList.Add (_gameData.factions[factionID1].positions[i]);
		}
		for(int i = 0; i < _gameData.factions[factionID2].positions.Length; i++)
		{
			tempLordList.Add (_gameData.factions[factionID2].positions[i]);
		}
		
		foreach(int lordID in tempLordList)
		{
			int roll = UnityEngine.Random.Range (0,temprolls.Count);

			unsortedLordIDs.Add (lordID,roll);
			temprolls = updatePossibleRolls(temprolls,roll);
		}
		
		IEnumerable<KeyValuePair<int, int>> sortedLordIDs = unsortedLordIDs.OrderByDescending(i => i.Value);
		
		foreach(KeyValuePair<int, int> sortedLordID in sortedLordIDs)
		{
			l.Add (sortedLordID.Key);
		}

		return l;
	}
	
	Dictionary<int, int> updatePossibleRolls(Dictionary<int, int> r, int id)
	{
		Dictionary<int, int> t = new Dictionary<int, int>();
		
		r.Remove (id);
		List<int> l = r.Values.ToList ();
		
		for(int i = 0; i < l.Count; i++)
		{
			t.Add (i,l[i]);
		}
		
		return t;
	}
	
	Dictionary<int,int> possibleRolls()
	{
		Dictionary<int, int> newRolls = new Dictionary<int, int>();
		
		for(int i = 0; i < diceSides; i++)
		{
			newRolls.Add (i,i);
		}
		
		return newRolls;
	}
}