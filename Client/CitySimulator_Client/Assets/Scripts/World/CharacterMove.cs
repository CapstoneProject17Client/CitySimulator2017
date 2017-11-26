using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Module: CharacterMove
/// Team: Client
/// Description: Make human move to location and back
/// Author: 
///	 Name: Lancelei Herradura   Date: 2017-11-13
/// Modified by:
///  Name: Lancelei Herradura	Date: 2017-11-25 
/// Based on:  How to Do PATHFINDING: The Basics (Graphs, BFS, and DFS in Unity) 
/// https://www.youtube.com/watch?v=WvR9voi0y2I
/// GitHub Username: anneomcl
/// https://github.com/anneomcl/PathfindingBasics
/// </summary>
public class CharacterMove : MonoBehaviour {

	// X axis of destination
	private static int x_dest;
	public int X_Dest {
		get {
			return x_dest;
		}
		set {
			x_dest = value;
			bfs.X_DEST = x_dest;
		}
	}
	// Z axis of destination
	private static int z_dest;
	public int Z_Dest {
		get {
			return z_dest;
		}
		set {
			z_dest = value;
			bfs.Z_DEST = z_dest;
		}
	}

	private int id;
	public int ID {
		get {
			return id;
		}
		set {
			id = value;
		}
	}

	// Reference for the CityDataManager class
	public CityDataManager cityDataManager;

	// Plane on which character is on
	private GameObject originalPlane;
	// Movement speed
	private static float speed = 30f;
	// access all BFS functions
	private BFS bfs = new BFS();

	/// <summary>
	/// Start this instance.
	/// For now, retrieve information at Start.
	/// </summary>
	void Start() {

		// Rotate, until we get the new humans
		transform.rotation = Quaternion.AngleAxis(-90, Vector3.right);

		// Until we get the new humans
		// transform.rotation = Quaternion.AngleAxis(-90, Vector3.right);

		bfs.OriginalPlane = findCurrentPlane();
		bfs.Start ();

	}


	/// <summary>
	/// Update this instance.
	/// Character moves to destination one plane at a time.
	/// Move to the location back and forth.
	/// </summary>
	void Update() {
		
		move ();
	}

	void move() {
		// use once referencing dictionary from manager is fixed
//		changedLocation ();

		if (bfs.To && bfs.Valid) {
			bfs.Move ();
			float step = Time.deltaTime * speed;
			transform.position = Vector3.MoveTowards (transform.position, bfs.CurrentPlane.transform.position, step);
			if (transform.position.Equals (bfs.CurrentPlane.transform.position) && bfs.PathIndex >= 0) {
				bfs.PathIndex--;
			} 
			if (bfs.PathIndex < 0) {
				bfs.To = false;
				bfs.PathIndex = 0;
			}
		} else if(!bfs.To && bfs.Valid) {
			bfs.Move ();
			float step = Time.deltaTime * speed;
			transform.position = Vector3.MoveTowards (transform.position, bfs.CurrentPlane.transform.position, step);
			if (transform.position.Equals (bfs.CurrentPlane.transform.position) && bfs.PathIndex >= 0) {
				bfs.PathIndex++;
			} 
			if (bfs.PathIndex >= bfs.Path.Count) {
				bfs.To = true;
				bfs.PathIndex = bfs.Path.Count - 1;
			}
		}
	}

	// Use when CityDataManager is object
	void changedLocation() {
		string currentDest = "(" + x_dest + ", " + z_dest + ")";
		string possibleDest;
		IList<int> points = new List<int>();
		cityDataManager = this.GetComponent<CityDataManager> ();
		Dictionary<int, GameObject> test = new Dictionary<int, GameObject>();
		test = cityDataManager.Humans;
		GameObject check = test[id];
		possibleDest = check.transform.GetChild(0).gameObject.GetComponent<TextMesh>().text;

		if (!possibleDest.Equals (currentDest)) {
			points = bfs.findNumber (possibleDest);
			x_dest = points [0];
			z_dest = points [1];

		}
			
	}

	/// <summary>
	/// Finds the current plane.
	/// </summary>
	/// <returns>The current plane.</returns>
	GameObject findCurrentPlane() {
		GameObject curr = new GameObject ();
		GameObject[] planes = GameObject.FindGameObjectsWithTag("plane");

		foreach (GameObject plane in planes) {
			if (plane.transform.position.Equals (this.transform.position))
				curr = plane;
		}

		return curr;

	}

}
