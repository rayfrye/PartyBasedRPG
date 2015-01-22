using UnityEngine;
using System.Collections;

public class movement : MonoBehaviour 
{

	void move(GameObject character, GameObject destination)
	{
		bool isHorizontalMovement;
		
		if(character.GetComponent<RectTransform>().localPosition.x == destination.GetComponent<RectTransform>().localPosition.x)
		{
			isHorizontalMovement = false;
		}
		else
		{
			isHorizontalMovement = true;
		}
		
		if(isHorizontalMovement)
		{
			bool isLeft;
			
			if(character.GetComponent<RectTransform>().localPosition.y > destination.GetComponent<RectTransform>().localPosition.y)
			{
				isLeft = true;
			}
			else
			{
				isLeft = false;
			}
			
			if(isLeft)
			{
				character.transform.Translate (-1 * 2f * Time.deltaTime,0,0);
			}
			else
			{
				character.transform.Translate (1 * 2f * Time.deltaTime,0,0);
			}
		}
		else
		{
			bool isUp;
			
			if(character.GetComponent<RectTransform>().localPosition.x > destination.GetComponent<RectTransform>().localPosition.x)
			{
				isUp = false;
			}
			else
			{
				isUp = true;
			}
			
			if(isUp)
			{
				character.transform.Translate (0,-1 * 2f * Time.deltaTime,0);
			}
			else
			{
				character.transform.Translate (0,1 * 2f * Time.deltaTime,0);
			}
		}
	}
}
