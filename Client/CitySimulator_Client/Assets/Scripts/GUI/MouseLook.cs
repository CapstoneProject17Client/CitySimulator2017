using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Module: Mouse look.
/// Team: Client
/// Description: Controls the camera's direction using Mouse coordinates.
/// Author:
///	 Name: Lancelei Herradura    Date: 01.31.17.
/// Modified by:	
///	 Name: Dongwon(Shawn) Kim   Change:	addin multi key for roatiting  Date: 2017-10-17
/// Based on:
///  http://answers.unity3d.com/questions/29741/mouse-look-script.html
/// </summary>
public class MouseLook : MonoBehaviour {

	//sensitivity of mouse
	public float mouseSensitivity = 60.0f;
	//clamp angle for viewport
	public float clampAngle = 80.0f;

 	// rotation around the up/y axis
	private float rotY = 0.0f;
	// rotation around the right/x axis
	private float rotX = 0.0f;

	// C clicked switch
	public bool cClicked;

	// Text Object for messge
	public Text msgForLock;

	/// <summary>
	/// Starts this instance
	/// </summary>
	void Start () {

		//unlock the mouse(camera) look
		cClicked = true;
		Vector3 rot = transform.localRotation.eulerAngles;
		rotY = rot.y;
		rotX = rot.x;

		// getting GUI Text msg for lock
//		msgForLock = GameObject.Find("CameraLockMsg").GetComponent<Text>();
		// disable GUI Text msg for lock
//		msgForLock.enabled = false;
	}

	/// <summary>
	/// Plaies the game button.
	/// </summary>
	void Update () {

		//check the input c or C gets from keyboard 
		if(Input.GetKey(KeyCode.C)){
			//switch statement
			if(cClicked){
				cClicked = false;
				// enable GUI Text msg for lock
				msgForLock.enabled = true;
			} else {
				cClicked = true;
				// disable GUI Text msg for lock
				msgForLock.enabled = false;
			}
		} 

		// if c has been tunred on apply following rotation
		if(cClicked) {
			float mouseX =  Input.GetAxis("Mouse X");
			float mouseY = -Input.GetAxis("Mouse Y");

			rotY += mouseX * mouseSensitivity * Time.deltaTime;
			rotX += mouseY * mouseSensitivity * Time.deltaTime;

			rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

			Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
			transform.rotation = localRotation;
		}
	}
}
