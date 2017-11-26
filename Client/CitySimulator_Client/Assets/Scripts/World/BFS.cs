using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Module: BFS
/// Team: Client
/// Description: Finds shortest path using BFS.
/// Author: 
///	 Name: Lancelei Herradura   Date: 2017-11-13
/// Based on:  How to Do PATHFINDING: The Basics (Graphs, BFS, and DFS in Unity)
/// https://www.youtube.com/watch?v=WvR9voi0y2I
/// GitHub Username: anneomcl
/// https://github.com/anneomcl/PathfindingBasics
/// </summary>
public class BFS {

	// x axis of destination
	private int x_dest;
	//z axis of destination
	private int z_dest;
	// Parents of each node visited
	private IDictionary<GameObject, GameObject> nodeParents = new Dictionary<GameObject, GameObject>();
	// City grid
	private static GameObject[] planes;
	// Shortest path found
	private IList<GameObject> path;
	// Index of current move being used for move
	private int pathIndex;
	// Current plane being used for move
	private GameObject currentPlane;
	// Start plane
	private GameObject originalPlane;
	// Destination plane
	private GameObject goal;
	// Going to location
	private bool to;
	// solution has been found
	private bool valid;

	/// <summary>
	/// Start this instance.
	/// </summary>
	public void Start () {
		planes = GameObject.FindGameObjectsWithTag("plane");
		goal = findPlane(x_dest, z_dest);
		path = getShortestPath (originalPlane, goal);
		pathIndex = path.Count - 1;
		to = true;
		valid = pathIndex >= 0 ? true : false;

	}

	/// <summary>
	/// Gets or sets the x DES.
	/// </summary>
	/// <value>The x DES.</value>
	public int X_DEST {
		get {
			return x_dest;
		}
		set {
			x_dest = value;
		}
	}

	/// <summary>
	/// Gets or sets the z DES.
	/// </summary>
	/// <value>The z DES.</value>
	public int Z_DEST {
		get {
			return z_dest;
		}

		set {
			z_dest = value;
		}
	}

	/// <summary>
	/// Gets or sets the original plane.
	/// </summary>
	/// <value>The original plane.</value>
	public GameObject OriginalPlane {
		get {
			return originalPlane;
		}
		set {
			originalPlane = value;
		}
	}

	/// <summary>
	/// Gets or sets the current plane.
	/// </summary>
	/// <value>The current plane.</value>
	public GameObject CurrentPlane {
		get {
			return currentPlane;
		}

		set {
			currentPlane = value;
		}
	}

	/// <summary>
	/// Gets or sets the goal.
	/// </summary>
	/// <value>The goal.</value>
	public  GameObject Goal {
		get {
			return goal;
		}

		set {
			goal = value;
		}
	}

	/// <summary>
	/// Gets or sets the path.
	/// </summary>
	/// <value>The path.</value>
	public IList<GameObject> Path {
		get {
			return path;
		}
		set {
			path = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="BFS"/> is to.
	/// </summary>
	/// <value><c>true</c> if to; otherwise, <c>false</c>.</value>
	public bool To {
		get {
			return to;
		}
		set {
			to = value;
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether solution is found.
	/// </summary>
	/// <value><c>true</c> if valid; otherwise, <c>false</c>.</value>
	public bool Valid {
		get {
			return valid;
		}
		set {
			valid = value;
		}
	}

	/// <summary>
	/// Gets or sets the index of the path.
	/// </summary>
	/// <value>The index of the path.</value>
	public int PathIndex {
		get {
			return pathIndex;
		}
		set {
			pathIndex = value;
		}
	}
		
	/// <summary>
	/// Move this instance.
	/// </summary>
	public void Move() {
		currentPlane = path [pathIndex];
	}

	/// <summary>
	/// Trace back steps once destination has been found.
	/// </summary>
	/// <returns>The shortest path.</returns>
	/// <param name="start">Start.</param>
	/// <param name="destination">Destination.</param>
	IList<GameObject> getShortestPath(GameObject start, GameObject destination) {
		IList<GameObject> pathway = new List<GameObject> ();
		GameObject goal = findShortestPathBFS (start, destination);

		GameObject curr = goal;

		while (!curr.Equals(start)) {
			pathway.Add (curr);
			curr = nodeParents [curr];
		}

		return pathway;

	}

	/// <summary>
	/// Finds the shortest path using BFS.
	/// </summary>
	/// <returns>The shortest path BF.</returns>
	/// <param name="startPos">Start position.</param>
	/// <param name="destPos">Destination position.</param>
	GameObject findShortestPathBFS(GameObject startPos, GameObject destPos) {
		Queue<GameObject> queue = new Queue<GameObject> ();
		HashSet<GameObject> exploredNodes = new HashSet<GameObject> ();
		queue.Enqueue (startPos);

		while (queue.Count != 0) {
			GameObject currentNode = queue.Dequeue ();
			if (currentNode.Equals (destPos)) {
				return currentNode;
			}

			IList<GameObject> nodes = GetWalkableNodes (currentNode);

			// Add to nodeParents to retrace back to start later
			foreach (GameObject node in nodes) {
				if (!exploredNodes.Contains (node)) {
					exploredNodes.Add (node);
					nodeParents.Add (node, currentNode);
					queue.Enqueue (node);
				}
			}

		}

		return startPos;
	}

	/// <summary>
	/// Gets the walkable nodes.
	/// </summary>
	/// <returns>The walkable nodes.</returns>
	/// <param name="current">Current.</param>
	IList<GameObject> GetWalkableNodes(GameObject current) {
		IList<GameObject> walkableNodes = new List<GameObject> ();
		IList<GameObject> possibleNodes = new List<GameObject> ();
		GameObject up, down, left, right;
		string textMesh = current.transform.GetChild(1).GetComponent<TextMesh> ().text;
		IList<int> points = findNumber (textMesh);
		int x = points [0];
		int z = points [1];

		up = findPlane (x, z + 1);
		down = findPlane (x, z - 1);
		left = findPlane (x - 1, z);
		right = findPlane (x + 1, z);

		possibleNodes.Add (up);
		possibleNodes.Add (down);
		possibleNodes.Add (left);
		possibleNodes.Add (right);

		// Only get nodes that are roads
		foreach (GameObject node in possibleNodes) {
			if(node != null)
			if (node.transform.GetChild (0).GetComponent<TextMesh> ().text == "0")
				walkableNodes.Add (node);

		}

		return walkableNodes;

	}

	/// <summary>
	/// Finds the axis inside plane textmesh.
	/// </summary>
	/// <returns>The number.</returns>
	/// <param name="textMesh">Text mesh.</param>
	public IList<int> findNumber(string textMesh) {
		IList<int> points = new List<int> ();
		char[] delimiterChars = { '{', ',', ' ' , '(', ')' };

		string[] words = textMesh.Split (delimiterChars);
		int temp;

		// Format is (x, z)
		foreach (string s in words) {
			if (int.TryParse (s, out temp))
				points.Add (temp);

		}

		return points;

	}

	/// <summary>
	/// Finds the plane that has certain x and z axis.
	/// </summary>
	/// <returns>The plane.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	GameObject findPlane(int x, int z) {
		string goalPlaneText = "(" + x + ", " + z + ")";
		foreach (GameObject plane in planes) {
			Transform grid = plane.transform;
			string gridText = grid.GetChild (1).GetComponent<TextMesh> ().text;
			if (gridText.Equals (goalPlaneText)) {
				return plane;
			}
		}

		return null;
	}

}
