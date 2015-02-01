using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SetupDialogues : MonoBehaviour 
{
	#region components
	public Transform _transform;
	public GameData _gameData;
	#endregion components

	public void setupDialogue
	(
		int id
		,string mainText
		,int numOptions
		,string option1Text
		,string option1Action
	)
	{
		Dialogue newDialogue = gameObject.AddComponent<Dialogue>();

		newDialogue.id = id;
		newDialogue.mainText = mainText;
		newDialogue.numOptions = numOptions;
		newDialogue.option1Text = option1Text;
		newDialogue.option1Action = option1Action;

		_gameData.dialogues.Add (id,newDialogue);
	}

	public void createDialogue()
	{
		setupDialogue
		(
			//id
			0
			//main text
			,"Im not supposed to be here"
			//num options
			,1
			//option1 text
			,""
			//option 1 action
			,"CloseDialogue|"
		);
	}
}
