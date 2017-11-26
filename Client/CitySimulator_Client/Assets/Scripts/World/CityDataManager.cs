using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Module: Point
/// Team: Client
/// Description: Coordinate Class model for the city data from the server
/// Author:
///  Name: Dongwon(Shawn) Kim    Date: 2017-11-25
/// Modified by:    
///  Name: N/A   Change: N/A         Date: N/A
/// Based on:  N/A
/// </summary>
[Serializable]
public class Point{

    // x coordinate 
    public int x;

    // z coordinate
    public int z;
}

/// <summary>
/// Module: NewBuilding
/// Team: Client
/// Description: Building Class model for the city data from the server
/// Author:
///  Name: Dongwon(Shawn) Kim    Date: 2017-11-25
/// Modified by:    
///  Name: N/A   Change: N/A         Date: N/A
/// Based on:  N/A
/// </summary>
[Serializable]
public class NewBuilding{

    // unique id for the building
    public string id;

    // name of the building
    public string Name;

    // location of the building
    public Point Point;

    // type of building
    public string Type;

    // rate of the building
    public int Rating;

    // I don't know what is this, not using on the client
    // but need to be here for deserialization
    public bool IsTall;
}

/// <summary>
/// Module: CityData
/// Team: Client
/// Description: Class model for the city data from the server
/// Author:
///  Name: Dongwon(Shawn) Kim    Date: 2017-11-25
/// Modified by:    
///  Name: N/A   Change: N/A         Date: N/A
/// Based on:  N/A
/// </summary>
[Serializable]
public class CityData{

    // length of the grid for the city
    public int GridLength;

    // width of the grid for the city
    public int GridWidth;
    
    // hour for the city
    public int netHours;

    // array of the points that has road
    public Point[] NewRoads;

    // array of buildings
    public NewBuilding[] NewBuildings;

}

/// <summary>
/// Module: CityDataManager
/// Team: Client
/// Description: Handle the all the data of the city, for now it has been used
/// Author:
///	 Name: Dongwon(Shawn) Kim    Date: 2017-09-11
/// Modified by:	
///	 Name: Dongwon(Shawn) Kim   Change:	Start to use		 	Date: 2017-10-17
///	 Name: Dongwon(Shawn) Kim   Change:	bug fix		 		 	Date: 2017-10-18
///  Name: Lancelei Herradura	Change: Adding walkable path 	Date: 2017-10-31
///	 Name: Dongwon(Shawn) Kim   Change:	gridforTest	 		 	Date: 2017-11-13
/// Based on:  N/A
/// </summary>
public class CityDataManager : MonoBehaviour {


	private Dictionary<int, GameObject> humans = new Dictionary<int, GameObject>();
	public Dictionary<int, GameObject> Humans {
		get {
			return humans;
		}

		set {
			humans = value;
		}
	}

	// switch for testing
    // JSON dummy String for testing
    private static string jsonString = "{\"GridLength\":99,\"GridWidth\":58,\"netHours\":0,\"NewRoads\":[{\"x\":27,\"z\":49},{\"x\":27,\"z\":56},{\"x\":28,\"z\":49},{\"x\":28,\"z\":56},{\"x\":29,\"z\":49},{\"x\":29,\"z\":56},{\"x\":30,\"z\":49},{\"x\":30,\"z\":56},{\"x\":27,\"z\":50},{\"x\":30,\"z\":50},{\"x\":27,\"z\":51},{\"x\":30,\"z\":51},{\"x\":27,\"z\":52},{\"x\":30,\"z\":52},{\"x\":27,\"z\":53},{\"x\":30,\"z\":53},{\"x\":27,\"z\":54},{\"x\":30,\"z\":54},{\"x\":27,\"z\":55},{\"x\":30,\"z\":55},{\"x\":24,\"z\":56},{\"x\":24,\"z\":63},{\"x\":25,\"z\":56},{\"x\":25,\"z\":63},{\"x\":26,\"z\":56},{\"x\":26,\"z\":63},{\"x\":27,\"z\":63},{\"x\":24,\"z\":57},{\"x\":27,\"z\":57},{\"x\":24,\"z\":58},{\"x\":27,\"z\":58},{\"x\":24,\"z\":59},{\"x\":27,\"z\":59},{\"x\":24,\"z\":60},{\"x\":27,\"z\":60},{\"x\":24,\"z\":61},{\"x\":27,\"z\":61},{\"x\":24,\"z\":62},{\"x\":27,\"z\":62},{\"x\":21,\"z\":63},{\"x\":21,\"z\":70},{\"x\":22,\"z\":63},{\"x\":22,\"z\":70},{\"x\":23,\"z\":63},{\"x\":23,\"z\":70},{\"x\":24,\"z\":70},{\"x\":21,\"z\":64},{\"x\":24,\"z\":64},{\"x\":21,\"z\":65},{\"x\":24,\"z\":65},{\"x\":21,\"z\":66},{\"x\":24,\"z\":66},{\"x\":21,\"z\":67},{\"x\":24,\"z\":67},{\"x\":21,\"z\":68},{\"x\":24,\"z\":68},{\"x\":21,\"z\":69},{\"x\":24,\"z\":69}],\"NewBuildings\":[{\"id\":\"24132329-e85a-4072-b9c8-1dab463b8443\",\"Name\":\"Pacocha Inc\",\"Point\":{\"x\":28,\"z\":54},\"Type\":\"I\",\"Rating\":0,\"IsTall\":true},{\"id\":\"0a6a8518-fc33-4d7d-bf88-ef7464f72d5e\",\"Name\":\"Hilll, Kohler and Effertz\",\"Point\":{\"x\":25,\"z\":59},\"Type\":\"C\",\"Rating\":0,\"IsTall\":true},{\"id\":\"43018e9e-b03b-45d1-b214-ae7a623d5a8a\",\"Name\":\"Residence\",\"Point\":{\"x\":22,\"z\":69},\"Type\":\"H\",\"Rating\":0,\"IsTall\":true}]}";
    
    // deserialize json to object
    private CityData cityData = JsonUtility.FromJson<CityData>(jsonString);
	
    // switch for testing
	public bool turnOnTestGrid;
	// update trigger
	public bool updateTheCity;

	// simulator's TimeStamp for start: hours + minute/60 + second/60
	public double systemStartedTimeStamp;

	// simualtor's TimeStampe for current: hours + minute/60 + second/60
	public double systemCurrentTimeStamp;

	// timeInHour
	public double timeInHour;

	// population of the city
	private int population = 1000;

	/// <summary>
	/// Gets the population.
	/// </summary>
	/// <returns>The population.</returns>
	public int Population{
		get {
			return  population;
		}
		set {
			population = value;
		}
	}

	// x number of grids in horizontal
	private int size_x;

	/// <summary>
	/// Gets the size x.
	/// </summary>
	/// <returns>The size x.</returns>
	public int Size_x{
		get{
			return size_x;
		}
		set{
			size_x = value;
		}
	}

	// z number of grids in vertical
	private int size_z;

	/// <summary>
	/// Gets the size z.
	/// </summary>
	/// <returns>The size z.</returns>
	public int Size_z{
		get{
			return size_z;
		}
		set{
			size_z = value;
		}
	}

	// grid map contains the index of the zone
	private int[][] grid;

	/// <summary>
	/// Gets or sets the grid.
	/// </summary>
	/// <value>The grid.</value>
	public int[][] Grid{
		get{
			return grid;
		}
		set{
			grid = value;
		}
	}

	// grid map of the walkable path
	private bool[][] path;

	/// <summary>
	/// Gets or sets the path.
	/// </summary>
	/// <value>The path.</value>
	public bool [][] Path {
		get {
			return path;
		}
		set {
			path = value;
		}
	}

	/// <summary>
	// Awake this instance.
	/// </summary>
	void Awake () {
        initiateCityData();

		systemStartedTimeStamp =  System.DateTime.Now.Minute;
		updateTheCity = false;
		if(turnOnTestGrid){ // if turned on for test, initiate test grid
			initiateGridForTest();
		}
        // else {  // else general grid
		//	initiateGrid ();
		//}
    
        Debug.Log(cityData.GridLength);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update(){
		/// Will be used in the future
		systemCurrentTimeStamp = System.DateTime.Now.Minute;

		if(updateTheCity){

			//call the updates

			updateTheCity = false;
		}

		// Debug.Log(systemStartedTimeStamp);
		// Debug.Log(systemCurrentTimeStamp);
	}

    public void initiateCityData(){
        size_z = cityData.GridLength;
        size_x = cityData.GridWidth;

        grid = new int[size_x][];

        // initalize 2d array
        for (int x = 0; x < grid.Length; x++) {
            grid [x] = new int[size_z]; 
        }

        // iterates grid and assign the zone
        for(int x = 0; x < size_x; x++){
            for (int z = size_z -1; z >= 0; z--) {
                grid[x][z] = -1;
            }
        }

        // assign road to grid        
        foreach(Point point in cityData.NewRoads){
            grid[point.x][point.z] = 0;
        }

        // assign building to grid
        foreach(NewBuilding building in cityData.NewBuildings){

            // Debug.Log(building);
            // Debug.Log((string)building.Type + " "
            //         + (string)building.Name + " "
            //         + building.Point.x + " "
            //         + building.Point.z + " "
            //         + building.Rating + " "
            //         + building.IsTall);

            int type = -1;
            switch(building.Type[0]){
                case 'H':
                    type = 1;
                    // Debug.Log("Res assigned: " + building.id + " " + building.Point.x + ", " + building.Point.z);
                break;

                case 'C':
                    type = 2;
                    // Debug.Log("Comm assigned: " + building.id + " " + building.Point.x + ", " + building.Point.z);
                break;

                case 'I':
                    type = 3;
                    // Debug.Log("Indst assigned: " + building.id + " " + building.Point.x + ", " + building.Point.z);
                break;

                default:
                    type = -1;
                break;
            }

            grid[building.Point.x][building.Point.z] = type;
        }
    }

	/// <summary>
	/// Initials the grid.
	/// </summary>
	public void initiateGrid() {
		grid = new int[size_x][];

		// initalize 2d array
		for (int x = 0; x < grid.Length; x++) {
			grid [x] = new int[size_z]; 
		}

		// iterates grid and assign the zone
		for(int x = 0; x < size_x; x++){
			for (int z = size_z -1; z >= 0; z--) {
				grid [x][z] = UnityEngine.Random.Range (0, 4);

				if(grid[x][z] == 2)
					grid[x][z] = 0;
			}
		}
	}


	/// <summary>
	/// Initiate grid for test
	/// </summary>
	public void initiateGridForTest(){		

		// initializing and assigning arrays
		grid = new[] {
			new int [] { 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 3, 3, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 3, 3, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 1, 1, 1, 1, 0, 3, 3, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 3, 3, 3, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 3, 3, 3, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 3, 3, 3, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 3, 3, 3, 3, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 3, 3, 3, 1, 1},
			new int [] { 0, 1, 1, 1, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 3, 3, 3, 1, 1}
		};

		// set the size to fit into array of
		size_x = grid.GetLength(0);
		size_z = grid[0].GetLength(0);
	}

	/// <summary>
	/// Gets the index of X.
	/// </summary>
	/// <returns>The index of X.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="z">The z coordinate.</param>
	public int getIndexOfXZ(int x, int z){

		if (x > size_x && z > size_z) {
			return -1;
		}

		return grid[x][z];
	}

	/// <summary>
	/// Turns on updateTheCity to update city 
	/// </summary>
	public void noticeUpdate(){
		updateTheCity = true;
	}
}
