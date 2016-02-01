using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Maze : MonoBehaviour {
	[System.Serializable]
	public class Cell{
		public bool visited;
		public GameObject north;//1
		public GameObject east;//2
		public GameObject west;//3
		public GameObject south;//4
	}



	public GameObject wall;
	public float walllength = 10.0f;
	public int xSize = 5;
	public int ySize = 5;
	private Vector3 initialPos;
	private GameObject wallHolder;
	public Cell[] cells;
    public int currentCell=0;
    private int totalCells;
    private int visitedCells = 0;
    private bool startedBuilding=false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0;

	// Use this for initialization
	void Start () {
		CreateWalls ();
	}
	void CreateWalls(){
		wallHolder = new GameObject ();
		wallHolder.name = "Maze";
		initialPos = new Vector3 ((-xSize / 2) + walllength / 2.0f, (-ySize / 2) + walllength / 2.0f);
		Vector3 myPos = initialPos;
		GameObject tempWall;


		// for x axis
		for (int i = 0; i < ySize; i++) {
			for (int j = 0; j <= xSize; j++) {
				myPos = new Vector3(initialPos.x+(j*walllength)-walllength/2.0f,0.0f,initialPos.z+(i*walllength)-walllength/2);
				tempWall = Instantiate(wall,myPos,Quaternion.identity) as GameObject;
				tempWall.transform.parent = wallHolder.transform;
			}
		}
		
		//for y axis
		for (int i = 0; i <= ySize; i++) {
			for (int j = 0; j < xSize; j++) {
				myPos = new Vector3(initialPos.x+(j*walllength),0.0f,initialPos.z+(i*walllength)-walllength);
				tempWall=Instantiate(wall,myPos,Quaternion.Euler(0.0f,90.0f,0.0f)) as GameObject;
				tempWall.transform.parent = wallHolder.transform;
			}
		}
		CreateCells ();
	}
	void CreateCells(){
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * ySize;
        GameObject[] allWalls;
		int children = wallHolder.transform.childCount;
		allWalls = new GameObject[children];
		cells = new Cell[xSize * ySize];
		int eastWestProcess = 0;
		int childProcess = 0;
		int termCount = 0;

		// gets all children

		for (int i=0; i < children; i++) {
			allWalls [i] = wallHolder.transform.GetChild (i).gameObject;
		}

		//Assigns walls to cells
		for (int cellprocess = 0; cellprocess < cells.Length; cellprocess++) {
            if (termCount == xSize)
            {
                eastWestProcess++;
                termCount = 0;
            }
            cells[cellprocess] = new Cell ();
			cells[cellprocess].east =allWalls[eastWestProcess];
			cells[cellprocess].south =allWalls[childProcess+(xSize+1)*ySize];

            eastWestProcess++;
			
			termCount++;
			childProcess++;
			cells [cellprocess].west = allWalls [eastWestProcess];
			cells[cellprocess].north =allWalls[(childProcess+(xSize+1)*ySize)+xSize-1];
		}
        CreateMaze();
	}

    void CreateMaze()
    {
        while(visitedCells < totalCells)
        {
            if (startedBuilding)
            {
                GiveMeNegihbour();
                if(cells[currentNeighbour].visited== false && cells[currentCell].visited == true)
                {
                    BreakWall();
                    cells[currentNeighbour].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell);
                    currentCell = currentNeighbour;
                    if (lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                    
                }
                
            }
            else
            {
                currentCell = Random.Range(0,totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }
        }
        Debug.Log("Finished");
    }


    void BreakWall()
    {
        switch (wallToBreak)
        {
            case 1:Destroy(cells[currentCell].north);break;
            case 2:Destroy(cells[currentCell].east);break;
            case 3:Destroy(cells[currentCell].west);break;
            case 4:Destroy(cells[currentCell].south);break;
        }
    }

    void GiveMeNegihbour()
    {
        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;
        check = ((currentCell + 1) / xSize);
        check -= 1;
        check *= xSize;
        check += xSize;
        //West 

        if(currentCell +1< totalCells && (currentCell + 1) != check)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbours[length] = currentCell + 1;
                connectingWall[length] = 3;
                length++;
            }
        }

        //East 

        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[length] = currentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }

        //North 
          
        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[length] = currentCell + xSize;
                connectingWall[length] = 1;
                length++;
            }
        }

        //South 

        if (currentCell - xSize >=0)
        {
            if (cells[currentCell -xSize].visited == false)
            {
                neighbours[length] = currentCell - xSize;
                connectingWall[length] = 4;
                length++;
            }
        }

        if (length != 0)
        {
            int thechosenOne = Random.Range(0, length);
            currentNeighbour = neighbours[thechosenOne];
            wallToBreak = connectingWall[thechosenOne];
        }
        else
        {
            if (backingUp > 0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }

    }
	// Update is called once per frame
	void Update () {
	
	}
}
