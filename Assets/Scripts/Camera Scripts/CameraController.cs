using UnityEngine;
using System.Collections;

public class CameraController: MonoBehaviour 
{
	/*
This camera smoothes out rotation around the y-axis and height.
Horizontal Distance to the target is always fixed.

There are many different ways to smooth the rotation but doing it this way gives you a lot of control over how the camera behaves.

For every of those smoothed values we calculate the wanted value and the current value.
Then we smooth it using the Lerp function.
Then we apply the smoothed values to the transform's position.
*/
	// The target we are following
	public Transform target;
	
	public Vector3 targetLocation;
	public GameObject[] players;
	public int numOfPlayers;
	
	float total_x;
	float total_y;
	float total_z;
	
	// The distance in the x-z plane to the target
	public float distance= 10.0f;
	public float changeYValue = 0;
	public float changeNoTargetYValue = 0;
	// the height we want the camera to be above the target
	public float height= 5.0f;
	// How much we 
	public float heightDamping= 2.0f;
	public float rotationDamping= 3.0f;
	public float originalHeight = 15f;

	float currentRotationAngle;
	
	Transform _transform;
	
	int Boundary = 10;
	int speed = 300;

	int theScreenWidth = Screen.width;
	int theScreenHeight = Screen.height;
	
	void Start()
	{
		_transform = this.GetComponent<Transform>();
		
		_transform.rotation = new Quaternion(.4f,0,0,.9f);

		float currentRotationAngle = 0;
	}

	void Update()
	{
		_transform.rotation = new Quaternion(.4f,0,0,.9f);

		if(target != null)
		{
			targetLocation = target.position;
			height = originalHeight+targetLocation.y;
		}

		//_transform.RotateAround(targetLocation,Vector3.right,(45*Input.GetAxis ("VerticalR")*Time.deltaTime)*-1);
	}

	void  LateUpdate ()
	{
		// Calculate the current rotation angles
		float wantedRotationAngle= 0;
		float wantedHeight= height;
		
//		float currentRotationAngle= 270;//transform.eulerAngles.y;
//		if(Input.GetAxis ("HorizontalR") != 0 && Mathf.Abs (currentRotationAngle)<90)
//		{
//			currentRotationAngle = currentRotationAngle+(90*Input.GetAxis ("HorizontalR")*Time.deltaTime/2)*-1;
//		}
//		else
//		{
//			currentRotationAngle=currentRotationAngle-(currentRotationAngle*Time.deltaTime);
//		}

		float currentHeight= transform.position.y;
		
		// Damp the rotation around the y-axis
//		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime)+changeYValue;
		
		// Convert the angle into a rotation - this will be the rotation around the target
		//Quaternion currentRotation= Quaternion.Euler (0, 300,0);//currentRotationAngle, 0);
//		Quaternion currentRotation = Quaternion.Euler (0, 0, 0);

		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
		//currentRotation.eulerAngles = targetRotation;
		
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		transform.position = targetLocation;
		transform.position -= currentRotation * Vector3.forward * distance;
		
		// Set the height of the camera
		transform.position = new Vector3(transform.position.x,currentHeight,transform.position.z);
		
		// Always look at the target
		transform.LookAt (target);
	}
}
