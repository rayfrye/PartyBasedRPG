using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LoadScripts : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		GameObject scripts = GameObject.Find ("Scripts");

		if(!scripts)
		{
			GameObject newScripts = new GameObject();

			newScripts.AddComponent<GameData>();
			newScripts.AddComponent<GenerateGameData>();
			SetupGrid _setupGrid = newScripts.AddComponent<SetupGrid>();
			newScripts.name = "Scripts";

			_setupGrid.colors = new Color32[10];

			//red
			_setupGrid.colors[0] = new Color32(231,42,42,255);
			//blue
			_setupGrid.colors[1] = new Color32(30,68,253,255);
			//yellow
			_setupGrid.colors[2] = new Color32(253,206,9,255);
			//purple
			_setupGrid.colors[3] = new Color32(194,45,210,255);
			//green
			_setupGrid.colors[4] = new Color32(15,223,88,255);
			//light blue
			_setupGrid.colors[5] = new Color32(28,166,234,255);
			//pink
			_setupGrid.colors[6] = new Color32(189,146,146,255);
			//dark red
			_setupGrid.colors[7] = new Color32(180,28,28,255);
			//dark green
			_setupGrid.colors[8] = new Color32(61,163,108,255);
			//white
			_setupGrid.colors[9] = new Color32(255,255,255,255);
		}
	}
}
