using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using ChromosomeDefinition;
using GeneticAlgorithmSettingDefinition;
using NesScripts.Controls.PathFind;

public class TestFitnessFunction : MonoBehaviour {

	FitnessFunctions FitnessFunction = new FitnessFunctions();
	//012 //[0,0][0,1][0,2]
	//345 //[1,0][1,1][1,2]
	//678 //[2,0][2,1][2,2]
	private int[,] TestMapArray = {
	{0,1,1,1,1,1,1,1},//  00 01 02 03 04 05 06 07
	{0,0,0,0,1,1,1,1},//  08 09 10 11 12 13 14 15
	{0,1,1,0,1,1,1,1},//  16 17 18 19 20 21 22 23
	{0,1,1,0,1,1,1,1},//  24 25 26 27 28 29 30 31
	{0,1,1,0,0,0,0,1},//  32 33 34 35 36 37 38 39
	{0,0,0,0,0,0,0,1},//  40 41 42 43 44 45 46 47
	{1,1,1,1,1,1,0,1},//  48 49 50 51 52 53 54 55
	{1,1,1,1,1,1,0,1}};// 56 57 58 59 60 61 62 63

	private Chromosome TestMap = new Chromosome();
	private Chromosome TestMap2 = new Chromosome();

	int[] _numMinGameObject = new int[5] { 1, 1, 1, 1, 1 };
	int[] _numMaxGameObject = new int[5] { 1, 1, 3, 2 ,2 };

	private List<int> EmptyTiles = new List<int>();
	private Grid spaceGrid;

	void Start()
	{
		//EmptyTiles.Clear();
		//InitialTestMap(TestMapArray, TestMap);
		////InitialTestMap(TestMapArray, TestMap2);
		//float test1 = Fitness_CorridorQuality(TestMap, 8, 8);
		//float test2 = Fitness_RectangleQuality(TestMap, 8, 8);

		//TestMap.AddGameObjectInList(0, GeneGameObjectAttribute.entrance);
		//TestMap.AddGameObjectInList(62, GeneGameObjectAttribute.exit);
		//TestMap.AddGameObjectInList(44, GeneGameObjectAttribute.enemy);
		//TestMap.AddGameObjectInList(45, GeneGameObjectAttribute.enemy);
		//TestMap.AddGameObjectInList(38, GeneGameObjectAttribute.trap);
		//TestMap.AddGameObjectInList(54, GeneGameObjectAttribute.trap);
		//TestMap.AddGameObjectInList(46, GeneGameObjectAttribute.treasure);

		////TestMap2.AddGameObjectInList(40, GeneGameObjectAttribute.entrance);
		////TestMap2.AddGameObjectInList(62, GeneGameObjectAttribute.exit);
		////TestMap2.AddGameObjectInList(9, GeneGameObjectAttribute.enemy);
		////TestMap2.AddGameObjectInList(10, GeneGameObjectAttribute.enemy);
		////TestMap2.AddGameObjectInList(35, GeneGameObjectAttribute.enemy);
		////TestMap2.AddGameObjectInList(24, GeneGameObjectAttribute.treasure);
		////TestMap2.AddGameObjectInList(45, GeneGameObjectAttribute.treasure);

		//TestMap.settingGameObject();
		////TestMap2.settingGameObject();

		//for (int index = 0; index < TestMap2.genesList.Count; index++)
		//{
		//	if (TestMap2.genesList[index].type == GeneType.Empty)
		//	{
		//		EmptyTiles.Add(index);
		//	}
		//}
		//Fitness_MainPathQuality(TestMap, 8, 8, EmptyTiles.Count, spaceGrid);
		//Fitness_ConnectedQuality(TestMap, 8, 8);

		////Debug.Log("SpaceCount = " + TestMap.spaceList.Count);
		////for (int index = 0; index < TestMap.spaceList.Count; index++)
		////{
		////	Debug.Log("index = " + index);
		////	Debug.Log("SpaceAttribute = " + TestMap.spaceList[index].SpaceAttribute);
		////	Debug.Log("startPos = " + TestMap.spaceList[index].startPos);
		////	Debug.Log("length = " + TestMap.spaceList[index].length);
		////	Debug.Log("width = " + TestMap.spaceList[index].width);
		////}

		//Debug.Log(Fitness_Defense(TestMap, 8, 8));
	}

	#region InitialTestMap
	public void InitialTestMap(int[,] MapArray, Chromosome Map)
	{
		// Create the genes in each chromosomes.
		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				Map.genesList.Add(new Gene());
				if (MapArray[x, y] == 0)
				{
					Map.genesList[8 * x + y].type = GeneType.Empty;
				}
				else if (MapArray[x, y] == 1)
				{
					Map.genesList[8 * x + y].type = GeneType.Forbidden;
				}
			}
		}
		// Initial the grid of space
		// create the tiles map
		bool[,] tilesmap = new bool[8, 8];
		// set values here....
		// true = walkable, false = blocking
		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				if (Map.genesList[x * 8 + y].type == GeneType.Empty)
				{
					tilesmap[x, y] = true;
				}
				if (Map.genesList[x * 8 + y].type == GeneType.Forbidden)
				{
					tilesmap[x, y] = false;
				}
			}
		}

		// create a grid
		spaceGrid = new Grid(tilesmap);
	}
	#endregion

	#region Fitness_MainPathQuality
	public float Fitness_MainPathQuality(Chromosome chromosome, int length, int width, int numEmpty, Grid spaceGrid)
	{
		float fitnessScore = 0.0f;

		FindMainPath(chromosome, length, width, spaceGrid);
		fitnessScore = (float)chromosome.mainPath.Count / numEmpty;

		return fitnessScore;
	}

	void FindMainPath(Chromosome chromosome, int length, int width, Grid spaceGrid)
	{
		// Initial
		int startPos_x = 0;
		int startPos_y = 0;
		int endPos_x = 0;
		int endPos_y = 0;
		chromosome.mainPath.Clear();

		foreach (var gameObject in chromosome.gameObjectList)
		{
			if (gameObject.GameObjectAttribute == GeneGameObjectAttribute.entrance)
			{
				startPos_x = (int)gameObject.Position / length;
				startPos_y = (int)gameObject.Position % length;
			}
			if (gameObject.GameObjectAttribute == GeneGameObjectAttribute.exit)
			{
				endPos_x = (int)gameObject.Position / length;
				endPos_y = (int)gameObject.Position % length;
			}
		}

		// create source and target points
		Point _from = new Point(startPos_x, startPos_y);
		Point _to = new Point(endPos_x, endPos_y);

		// for Manhattan distance (4 directions)
		List<Point> pathSearch = Pathfinding.FindPath(spaceGrid, _from, _to, Pathfinding.DistanceType.Manhattan);

		chromosome.mainPath.Add(startPos_x * length + startPos_y); // Add Start point
		foreach (var item in pathSearch)
		{
			chromosome.mainPath.Add(item.x * length + item.y);
		}
	}
	#endregion

	#region Fitness_ConnectedQuality
	public float Fitness_ConnectedQuality(Chromosome chromosome, int length, int width)
	{
		detectedSpaceAttribute(chromosome);

		float fitnessScore = 0.0f;

		if (chromosome.spaceList.Count != 0)
		{
			fitnessScore = isConnected(chromosome, length, width);
		}
		else
		{
			fitnessScore = 0.0f;
		}
		return fitnessScore;
	}

	void detectedSpaceAttribute(Chromosome _chromosome)
	{
		int indexGene = 0;
		foreach (Gene gene in _chromosome.genesList)
		{
			if (gene.SpaceAttribute == GeneSpaceAttribute.None && gene.type == GeneType.Empty)
			{
				// Save the information of None
				_chromosome.spaceList.Add(new SpaceInfo());
				_chromosome.spaceList[_chromosome.spaceList.Count - 1].startPos = indexGene;
				_chromosome.spaceList[_chromosome.spaceList.Count - 1].length = 1;
				_chromosome.spaceList[_chromosome.spaceList.Count - 1].width = 1;
				_chromosome.spaceList[_chromosome.spaceList.Count - 1].SpaceAttribute = GeneSpaceAttribute.None;
			}
			indexGene++;
		}
	}

	float isConnected(Chromosome _chromosome, int length, int width)
	{
		List<SpaceConnected_Root> RootList = new List<SpaceConnected_Root>();
		int[] beVisited = new int[_chromosome.spaceList.Count];// 紀錄有無拜訪過
		int index_checking = 0;

		// Finding the space which haven't be visited.
		while (index_checking < _chromosome.spaceList.Count)
		{
			// The index of space we find be set a root.
			RootList.Add(new SpaceConnected_Root());
			RootList[RootList.Count - 1].spaceIndex = index_checking;
			// Start to find the leaf of root.
			beVisited = findRootLeaf(_chromosome, length, width, RootList[RootList.Count - 1], beVisited);
			index_checking++;
		}
		// -----End of Searching Root & Leaf-----

		int spaceCount = _chromosome.spaceList.Count;
		int[] checkSpaceConnected = new int[spaceCount];// 紀錄是否連通
		int indexRoot = 0;
		float connectedCount = 0.0f;
		float connectedRatio = 0.0f;

		visitedConnectedSpace(indexRoot, RootList, checkSpaceConnected);

		for (int i = 0; i < checkSpaceConnected.Length; i++)
		{
			if (checkSpaceConnected[i] == 1)
			{
				connectedCount++;
			}
		}

		connectedRatio = connectedCount / checkSpaceConnected.Length;

		return connectedRatio;
	}

	int[] findRootLeaf(Chromosome _chromosome, int length, int width, SpaceConnected_Root _Root, int[] _beVisited)
	{
		int[] _beVisitedFinished = _beVisited;
		// Get the information of root space.
		int rootSpace_startPos_x = (int)_chromosome.spaceList[_Root.spaceIndex].startPos / length;
		int rootSpace_startPos_y = (int)_chromosome.spaceList[_Root.spaceIndex].startPos % length;
		int rootSpace_length = _chromosome.spaceList[_Root.spaceIndex].length;
		int rootSpace_width = _chromosome.spaceList[_Root.spaceIndex].width;
		GeneSpaceAttribute rootSpaceAttribute = _chromosome.spaceList[_Root.spaceIndex].SpaceAttribute;

		#region Top row
		// Top row
		if (rootSpace_startPos_x - 1 >= 0)
		{
			for (int y = rootSpace_startPos_y; y < ( rootSpace_startPos_y + rootSpace_length ); y++)
			{
				if (_chromosome.genesList[( rootSpace_startPos_x - 1 ) * length + y].type == GeneType.Empty)
				{
					// Find the index of the space that contain the empty tile.
					int indexNeighborSpace = checkNeighbor(_chromosome, length, width, ( rootSpace_startPos_x - 1 ), y);
					_beVisited[indexNeighborSpace] = 1;
					// Save in root.
					_Root.connectedLeaf.Add(new SpaceConnected_Leaf());
					_Root.connectedLeaf[_Root.connectedLeaf.Count - 1].spaceIndex = indexNeighborSpace;
				}
			}
		}
		#endregion

		#region Right column
		// Right column
		if (rootSpace_startPos_y + rootSpace_length < length)
		{
			for (int x = rootSpace_startPos_x; x < ( rootSpace_startPos_x + rootSpace_width ); x++)
			{
				if (_chromosome.genesList[x * length + ( rootSpace_startPos_y + rootSpace_length )].type == GeneType.Empty)
				{
					// Find the index of the space that contain the empty tile.
					int indexNeighborSpace = checkNeighbor(_chromosome, length, width, x, ( rootSpace_startPos_y + rootSpace_length ));
					_beVisited[indexNeighborSpace] = 1;
					// Save in root.
					_Root.connectedLeaf.Add(new SpaceConnected_Leaf());
					_Root.connectedLeaf[_Root.connectedLeaf.Count - 1].spaceIndex = indexNeighborSpace;
				}
			}
		}
		#endregion

		#region Bottom row
		// Bottom row
		if (rootSpace_startPos_x + rootSpace_width < width)
		{
			for (int y = rootSpace_startPos_y; y < ( rootSpace_startPos_y + rootSpace_length ); y++)
			{
				if (_chromosome.genesList[( rootSpace_startPos_x + rootSpace_width ) * length + y].type == GeneType.Empty)
				{
					// Find the index of the space that contain the empty tile.
					int indexNeighborSpace = checkNeighbor(_chromosome, length, width, ( rootSpace_startPos_x + rootSpace_width ), y);
					_beVisited[indexNeighborSpace] = 1;
					// Save in root.
					_Root.connectedLeaf.Add(new SpaceConnected_Leaf());
					_Root.connectedLeaf[_Root.connectedLeaf.Count - 1].spaceIndex = indexNeighborSpace;
				}
			}
		}
		#endregion

		#region Left column
		// Left column
		if (rootSpace_startPos_y - 1 >= 0)
		{
			for (int x = rootSpace_startPos_x; x < ( rootSpace_startPos_x + rootSpace_width ); x++)
			{
				if (_chromosome.genesList[x * length + ( rootSpace_startPos_y - 1 )].type == GeneType.Empty)
				{
					// Find the index of the space that contain the empty tile.
					int indexNeighborSpace = checkNeighbor(_chromosome, length, width, x, ( rootSpace_startPos_y - 1 ));
					_beVisited[indexNeighborSpace] = 1;
					// Save in root.
					_Root.connectedLeaf.Add(new SpaceConnected_Leaf());
					_Root.connectedLeaf[_Root.connectedLeaf.Count - 1].spaceIndex = indexNeighborSpace;
				}
			}
		}
		#endregion

		return _beVisitedFinished;
	}
	// Checking the index of Neighbor. 
	int checkNeighbor(Chromosome _chromosome, int length, int width, int neighbor_x, int neighbor_y)
	{
		GeneSpaceAttribute neighborSpaceAttribute = _chromosome.genesList[neighbor_x * length + neighbor_y].SpaceAttribute;
		int indexNeighborSpace = 0;
		bool isNeighbor = false;

		for (int index = 0; index < _chromosome.spaceList.Count; index++)
		{
			if (isNeighbor == false && _chromosome.spaceList[index].SpaceAttribute == neighborSpaceAttribute)
			{
				// Get the information of space in spaceList.
				int spaceStartPos_x = _chromosome.spaceList[index].startPos / length;
				int spaceStartPos_y = _chromosome.spaceList[index].startPos % length;
				int spaceLength = _chromosome.spaceList[index].length;
				int spaceWidth = _chromosome.spaceList[index].width;

				// Find the empty tile is belonged to which space.
				if (neighbor_x >= spaceStartPos_x
					&& neighbor_x < ( spaceStartPos_x + spaceWidth )
					&& neighbor_y >= spaceStartPos_y
					&& neighbor_y < ( spaceStartPos_y + spaceLength ))
				{
					isNeighbor = true;
					indexNeighborSpace = index;
				}
				else
				{
					isNeighbor = false;
				}
			}
		}
		return indexNeighborSpace;
	}

	void visitedConnectedSpace(int indexRoot, List<SpaceConnected_Root> _RootList, int[] _checkSpaceConnected)
	{
		// Connected itself!!		
		_checkSpaceConnected[indexRoot] = 1;

		for (int indexLeaf = 0; indexLeaf < _RootList[indexRoot].connectedLeaf.Count; indexLeaf++)
		{
			if (_checkSpaceConnected[_RootList[indexRoot].connectedLeaf[indexLeaf].spaceIndex] == 0)
			{
				// Connect the leaf!!
				_checkSpaceConnected[_RootList[indexRoot].connectedLeaf[indexLeaf].spaceIndex] = 1;
				// visit the leafs of leaf.
				visitedConnectedSpace(_RootList[indexRoot].connectedLeaf[indexLeaf].spaceIndex, _RootList, _checkSpaceConnected);
			}
		}
	}
	#endregion

	#region Fitness_CorridorQuality
	public float Fitness_CorridorQuality(Chromosome chromosome, int length, int width)
	{
		float fitnessScore = 0.0f;
		int numEmpty = 0;
		int numCorridorGene = 0;
		int numCorridor = calculateCorridorNumber(chromosome, length, width);

		numEmpty = ( from gene in chromosome.genesList
					 where gene.type == GeneType.Empty
					 select gene ).Count();

		numCorridorGene = ( from gene in chromosome.genesList
							 where gene.SpaceAttribute == GeneSpaceAttribute.Corridor
							 select gene ).Count();

		fitnessScore = (float)numCorridorGene / numEmpty;

		return fitnessScore;
	}

	int calculateCorridorNumber(Chromosome _chromosome, int length, int width)
	{
		int numCorridor = 0;
		int numGene = _chromosome.genesList.Count;
		bool isFindCorridor = true;
		int corridorLength = 2;
		bool isRowCorridor = true;

		for (int indexGene = 0; indexGene < numGene; indexGene++)
		{
			// Find the start point of Rectangle
			if (_chromosome.genesList[indexGene].type == GeneType.Empty && _chromosome.genesList[indexGene].SpaceAttribute == GeneSpaceAttribute.None)
			{
				// Find the row corridor first
				isRowCorridor = true;
				corridorLength = 2;
				// Find row corridor
				isFindCorridor = findCorridor(_chromosome, length, width, indexGene, corridorLength, isRowCorridor);
				// Can't find row corridor
				if (isFindCorridor == false)
				{
					isRowCorridor = false;
					// Find column corridor
					isFindCorridor = findCorridor(_chromosome, length, width, indexGene, corridorLength, isRowCorridor);
				}

				if (isFindCorridor == true)
				{
					// There is a corridor and length of corridor is 2 at least.
					// Start to find whole corridor
					isFindCorridor = findCorridor(_chromosome, length, width, indexGene, ( corridorLength + 1 ), isRowCorridor);
					while (isFindCorridor)
					{
						corridorLength++;
						isFindCorridor = findCorridor(_chromosome, length, width, indexGene, ( corridorLength + 1 ), isRowCorridor);
					}

					// Set Rectangle
					setCorridor(_chromosome, length, width, indexGene, corridorLength, isRowCorridor);
					//Debug.Log("StartPoint = " + indexGene + ", corridorLength = " + corridorLength + ", isRowCorridor = " + isRowCorridor);
					// Get one Corridor
					numCorridor++;
				}				
			}
		}
		return numCorridor;
	}

	bool findCorridor(Chromosome _chromosome, int length, int width, int startPos, int corridorlength, bool isRow)
	{
		bool isCorridor = true;
		int startPos_x = (int)startPos / length;
		int startPos_y = (int)startPos % length;
		bool upForbidden = false;
		bool downForbidden = false;
		bool rightForbidden = false;
		bool leftForbidden = false;


		if (isRow == true)
		{
			// Out of range
			if (( startPos_y + corridorlength ) > length)
			{
				isCorridor = false;
			}
			else
			{
				#region Find Row
				// Find Row
				for (int y = startPos_y; y < ( startPos_y + corridorlength ); y++)
				{
					// The previous tile is Corridor.
					if (isCorridor == true)
					{
						if (_chromosome.genesList[startPos_x * length + y].type == GeneType.Empty
						&& _chromosome.genesList[startPos_x * length + y].SpaceAttribute == GeneSpaceAttribute.None)
						{
							// Check the up forbidden
							if (( startPos_x - 1 ) < 0) // boundary
							{
								upForbidden = true;
							}
							else
							{
								upForbidden = false;
								if (_chromosome.genesList[( startPos_x - 1 ) * length + y].type == GeneType.Forbidden)
								{
									upForbidden = true;
								}
								else
								{
									upForbidden = false;
								}
							}

							// Check the down forbidden
							if (( startPos_x + 1 ) == width) // boundary
							{
								downForbidden = true;
							}
							else
							{
								downForbidden = false;
								if (_chromosome.genesList[( startPos_x + 1 ) * length + y].type == GeneType.Forbidden)
								{
									downForbidden = true;
								}
								else
								{
									downForbidden = false;
								}
							}
							if (upForbidden == true && downForbidden == true)
							{
								isCorridor = true;
							}
							else
							{
								isCorridor = false;
							}
						}
						else
						{
							isCorridor = false;
						}
					}
				}
				#endregion
			}
		}
		else
		{
			// Out of range
			if (( startPos_x + corridorlength ) > width)
			{
				isCorridor = false;
			}
			else
			{
				#region Find Column
				// Find Column
				for (int x = startPos_x; x < ( startPos_x + corridorlength ); x++)
				{
					// The previous tile is Corridor.
					if (isCorridor == true)
					{
						if (_chromosome.genesList[x * length + startPos_y].type == GeneType.Empty
						&& _chromosome.genesList[x * length + startPos_y].SpaceAttribute == GeneSpaceAttribute.None)
						{
							// Check the left forbidden
							if (( startPos_y - 1 ) < 0) // boundary
							{
								leftForbidden = true;
							}
							else
							{
								leftForbidden = false;
								if (_chromosome.genesList[x * length + startPos_y - 1].type == GeneType.Forbidden)
								{
									leftForbidden = true;
								}
								else
								{
									leftForbidden = false;
								}
							}

							// Check the right forbidden
							if (( startPos_y + 1 ) == length) // boundary
							{
								rightForbidden = true;
							}
							else
							{
								rightForbidden = false;
								if (_chromosome.genesList[x * length + startPos_y + 1].type == GeneType.Forbidden)
								{
									rightForbidden = true;
								}
								else
								{
									rightForbidden = false;
								}
							}
							if (leftForbidden == true && rightForbidden == true)
							{
								isCorridor = true;
							}
							else
							{
								isCorridor = false;
							}
						}
						else
						{
							isCorridor = false;
						}
					}
				}
				#endregion
			}
		}
		return isCorridor;
	}

	void setCorridor(Chromosome _chromosome, int length, int width, int startPos, int corridorlength, bool isRow)
	{
		int startPos_x = (int)startPos / length;
		int startPos_y = (int)startPos % length;

		if (isRow == true)
		{
			for (int y = startPos_y; y < ( startPos_y + corridorlength ); y++)
			{
				_chromosome.genesList[startPos_x * length + y].SpaceAttribute = GeneSpaceAttribute.Corridor;
			}
		}
		else
		{
			for (int x = startPos_x; x < ( startPos_x + corridorlength ); x++)
			{
				_chromosome.genesList[x * length + startPos_y].SpaceAttribute = GeneSpaceAttribute.Corridor;
			}
		}

		// Save the information of Corridor
		_chromosome.spaceList.Add(new SpaceInfo());
		_chromosome.spaceList[_chromosome.spaceList.Count - 1].startPos = startPos;
		if (isRow == true)
		{
			_chromosome.spaceList[_chromosome.spaceList.Count - 1].length = corridorlength;
			_chromosome.spaceList[_chromosome.spaceList.Count - 1].width = 1;
		}
		else
		{
			_chromosome.spaceList[_chromosome.spaceList.Count - 1].length = 1;
			_chromosome.spaceList[_chromosome.spaceList.Count - 1].width = corridorlength;
		}		
		_chromosome.spaceList[_chromosome.spaceList.Count - 1].SpaceAttribute = GeneSpaceAttribute.Corridor;
	}
	#endregion

	#region Fitness_RectangleQuality
	public float Fitness_RectangleQuality(Chromosome chromosome, int length, int width)
	{
		float fitnessScore = 0.0f;
		int numEmpty = 0;
		int numRectangleGene = 0;
		int numRectangle = calculateRectangleNumber(chromosome, length, width);

		numEmpty = ( from gene in chromosome.genesList
					 where gene.type == GeneType.Empty
					 select gene ).Count();

		numRectangleGene = ( from gene in chromosome.genesList
							 where gene.SpaceAttribute == GeneSpaceAttribute.Rectangle
							 select gene ).Count();

		fitnessScore = (float)numRectangleGene / numEmpty;

		return fitnessScore;
	}

	int calculateRectangleNumber(Chromosome _chromosome, int length, int width)
	{
		int numRectangle = 0;
		int numGene = _chromosome.genesList.Count;
		bool isFindRectangle;
		int rectangleLength = 2;
		int rectangleWidth = 2;

		for (int indexGene = 0; indexGene < numGene; indexGene++)
		{
			// Find the start point of Rectangle
			if (_chromosome.genesList[indexGene].type == GeneType.Empty && _chromosome.genesList[indexGene].SpaceAttribute == GeneSpaceAttribute.None)
			{
				rectangleLength = 2;
				rectangleWidth = 2;
				// Find 2*2
				isFindRectangle = findRectangle(_chromosome, length, width, indexGene, rectangleLength, rectangleWidth);

				if (isFindRectangle == true)
				{
					// Extend the length
					isFindRectangle = findRectangle(_chromosome, length, width, indexGene, rectangleLength + 1, rectangleWidth);
					while (isFindRectangle)
					{
						rectangleLength++;
						isFindRectangle = findRectangle(_chromosome, length, width, indexGene, rectangleLength + 1, rectangleWidth);
					}

					// Extend the width
					isFindRectangle = findRectangle(_chromosome, length, width, indexGene, rectangleLength, rectangleWidth + 1);
					while (isFindRectangle)
					{
						rectangleWidth++;
						isFindRectangle = findRectangle(_chromosome, length, width, indexGene, rectangleLength, rectangleWidth + 1);
					}

					// Set Rectangle
					setRectangle(_chromosome, length, width, indexGene, rectangleLength, rectangleWidth);

					// Get one Rectangle
					numRectangle++;
				}
			}
		}
		return numRectangle;
	}

	bool findRectangle(Chromosome _chromosome, int length, int width, int startPos, int rectangleLength, int rectangleWidth)
	{
		bool isRectangle = true;
		int startPos_x = (int)startPos / length;
		int startPos_y = (int)startPos % length;

		for (int x = startPos_x; x < ( startPos_x + rectangleWidth ); x++)
		{
			for (int y = startPos_y; y < ( startPos_y + rectangleLength ); y++)
			{
				// Out of range
				if (x >= width || y >= length)
				{
					isRectangle = false;
				}
				else
				{
					if (_chromosome.genesList[x * length + y].type != GeneType.Empty
					|| _chromosome.genesList[x * length + y].SpaceAttribute != GeneSpaceAttribute.None)
					{
						isRectangle = false;
					}
				}
			}
		}
		return isRectangle;
	}

	void setRectangle(Chromosome _chromosome, int length, int width, int startPos, int rectangleLength, int rectangleWidth)
	{
		int startPos_x = (int)startPos / length;
		int startPos_y = (int)startPos % length;

		for (int x = startPos_x; x < ( startPos_x + rectangleWidth ); x++)
		{
			for (int y = startPos_y; y < ( startPos_y + rectangleLength ); y++)
			{
				_chromosome.genesList[x * length + y].SpaceAttribute = GeneSpaceAttribute.Rectangle;
			}
		}
		// Save the information of Rectangle
		_chromosome.spaceList.Add(new SpaceInfo());
		_chromosome.spaceList[_chromosome.spaceList.Count - 1].startPos = startPos;
		_chromosome.spaceList[_chromosome.spaceList.Count - 1].length = rectangleLength;
		_chromosome.spaceList[_chromosome.spaceList.Count - 1].width = rectangleWidth;
		_chromosome.spaceList[_chromosome.spaceList.Count - 1].SpaceAttribute = GeneSpaceAttribute.Rectangle;
	}
	#endregion
	
	#region Fitness_ImpassableDensity
	public float Fitness_ImpassableDensity(Chromosome chromosome, int length, int width)
	{
		float fitnessScore = 0.0f;

		int[,] beVisited = new int[width, length];// 紀錄有無拜訪過
		List<float> subImpassableDensity = new List<float>();// 紀錄各子Impassable的Density

		calculateImpassableDensity(chromosome, length, width, beVisited, subImpassableDensity);

		for (int i = 0; i < subImpassableDensity.Count; i++)
		{
			Debug.Log(i + " : " + subImpassableDensity[i]);
			fitnessScore = fitnessScore + subImpassableDensity[i];
		}

		fitnessScore = (float)fitnessScore / subImpassableDensity.Count;

		return fitnessScore;
	}

	void calculateImpassableDensity(Chromosome _chromosome, int _length, int _width, int[,] _beVisited, List<float> subDensity)
	{
		int visitID = 0; // 被拜訪的 Tile 在第幾個位置
		int visit_posX = 0; // 被拜訪的 X 位置
		int visit_posY = 0; // 被拜訪的 Y 位置

		// 拜訪每個 Tile
		for (visitID = 0; visitID < _length * _width; visitID++)
		{
			visit_posX = (int)visitID / _length;
			visit_posY = (int)visitID % _length;

			if (_beVisited[visit_posX, visit_posY] == 0)
			{
				// Here is passable.
				if (_chromosome.genesList[visitID].type == GeneType.Empty)
				{
					// Be visited.
					_beVisited[visit_posX, visit_posY] = 1;
				}
				// Here is impassable.
				else if (_chromosome.genesList[visitID].type == GeneType.Forbidden)
				{
					// calculate the Impassable Density.
					subDensity.Add(startCalculateSubImpassable(visit_posX, visit_posY, _length, _width, _chromosome, _beVisited));
				}
			}
		}
	}

	float startCalculateSubImpassable(int startPos_X, int startPos_Y, int _length, int _width, Chromosome _chromosome, int[,] _beVisited)
	{
		// Set the range of max rectangle.
		// minX, minY, maxX, maxY
		int[] rangePosition = new int[6] { startPos_X, startPos_Y, startPos_X, startPos_Y, _length, _width };

		// Search the area of impassable tiles.
		DFS(startPos_X, startPos_Y, rangePosition, _chromosome, _beVisited);

		// Get the range
		int minX = rangePosition[0];
		int minY = rangePosition[1];
		int maxX = rangePosition[2];
		int maxY = rangePosition[3];
		// Number of tiles in the range
		int num_rangeTiles = 0;
		// Number of impassable tiles
		int num_impassableTile = 0;
		// Density
		float subDensity = 0.0f;

		// Calculate the number of tiles in the range.
		num_rangeTiles = ( maxX - minX + 1 ) * ( maxY - minY + 1 );
		// Calculate the number of impassable tiles.
		for (int y = minY; y <= maxY; y++)
		{
			for (int x = minX; x <= maxX; x++)
			{
				if (_chromosome.genesList[_length * x + y].type == GeneType.Forbidden)
					num_impassableTile++;
			}
		}
		// Calculate the density of impassable area.
		subDensity = (float)num_impassableTile / num_rangeTiles;

		//Debug.Log("minX: " + rangePosition[0] + "minY: " + rangePosition[1] + "maxX: " + rangePosition[2]
		//	+ "maxY: " + rangePosition[3] + "length: " + rangePosition[4] + "width: " + rangePosition[5]);
		//Debug.Log("num_rangeTiles: " + num_rangeTiles);
		//Debug.Log("num_impassableTile: " + num_impassableTile);
		//Debug.Log("subDensity: " + subDensity);
		//for (int i = 0; i < _width; i++)
		//{
		//	Debug.Log(_beVisited[i, 0]
		//		+ ", " + _beVisited[i, 1]
		//		+ ", " + _beVisited[i, 2]
		//		+ ", " + _beVisited[i, 3]
		//		+ ", " + _beVisited[i, 4]
		//		+ ", " + _beVisited[i, 5]
		//		+ ", " + _beVisited[i, 6]
		//		+ ", " + _beVisited[i, 7]);
		//}
		return subDensity;
	}

	void DFS(int startPos_X, int startPos_Y, int[] _rangePosition, Chromosome _chromosome, int[,] _beVisited)
	{
		int tileMap_length = _rangePosition[4];
		int tileMap_width = _rangePosition[5];

		// Visit the start position.
		_beVisited[startPos_X, startPos_Y]++;
		//Debug.Log(startPos_X + ", " + startPos_Y);

		/// 判斷右下左上是否有尚未拜訪過的ImpassableTile
		#region 判斷右下左上是否有尚未拜訪過的ImpassableTile
		// Right
		if (startPos_Y + 1 < tileMap_length)
		{
			if (_beVisited[startPos_X, startPos_Y + 1] == 0
			   && _chromosome.genesList[tileMap_length * startPos_X + ( startPos_Y + 1 )].type == GeneType.Forbidden)
			{
				// Find the maxY of position.
				if (_rangePosition[3] < startPos_Y + 1)
					_rangePosition[3] = startPos_Y + 1;
				// Search!!
				DFS(startPos_X, startPos_Y + 1, _rangePosition, _chromosome, _beVisited);
			}
		}
		// Down
		if (startPos_X + 1 < tileMap_width)
		{
			if (_beVisited[startPos_X + 1, startPos_Y] == 0
				&& _chromosome.genesList[tileMap_length * ( startPos_X + 1 ) + startPos_Y].type == GeneType.Forbidden)
			{
				// Find the maxX of position.
				if (_rangePosition[2] < startPos_X + 1)
					_rangePosition[2] = startPos_X + 1;
				// Search!!
				DFS(startPos_X + 1, startPos_Y, _rangePosition, _chromosome, _beVisited);
			}
		}
		// Left
		if (startPos_Y - 1 >= 0)
		{
			if (_beVisited[startPos_X, startPos_Y - 1] == 0
			&& _chromosome.genesList[tileMap_length * startPos_X + ( startPos_Y - 1 )].type == GeneType.Forbidden)
			{
				// Find the minY of position.
				if (_rangePosition[1] > startPos_Y - 1)
					_rangePosition[1] = startPos_Y - 1;
				// Search!!
				DFS(startPos_X, startPos_Y - 1, _rangePosition, _chromosome, _beVisited);
			}
		}
		// Up
		if (startPos_X - 1 >= 0)
		{
			if (_beVisited[startPos_X - 1, startPos_Y] == 0
			&& _chromosome.genesList[tileMap_length * ( startPos_X - 1 ) + startPos_Y].type == GeneType.Forbidden)
			{
				// Find the minX of position.
				if (_rangePosition[0] > startPos_X - 1)
					_rangePosition[0] = startPos_X - 1;
				// Search!!
				DFS(startPos_X - 1, startPos_Y, _rangePosition, _chromosome, _beVisited);
			}
		}
		#endregion

		/// 判斷右下左上是否皆為拜訪過的ImpassableTile或passableTile
		#region 判斷右下左上是否皆為拜訪過的ImpassableTile或passableTile
		// 判斷四周拜訪過的ImpassableTile或passableTile的總數
		int visitedNeighbor = 0;
		// Right
		if (startPos_Y + 1 < tileMap_length)
		{
			if (_beVisited[startPos_X, startPos_Y + 1] > 0
			|| _chromosome.genesList[tileMap_length * startPos_X + ( startPos_Y + 1 )].type == GeneType.Empty)
			{
				visitedNeighbor++;
			}
		}
		else
		{
			visitedNeighbor++;
		}
		// Down
		if (startPos_X + 1 < tileMap_width)
		{
			if (_beVisited[startPos_X + 1, startPos_Y] > 0
				|| _chromosome.genesList[tileMap_length * ( startPos_X + 1 ) + startPos_Y].type == GeneType.Empty)
			{
				visitedNeighbor++;
			}
		}
		else
		{
			visitedNeighbor++;
		}
		// Left
		if (startPos_Y - 1 >= 0)
		{
			if (_beVisited[startPos_X, startPos_Y - 1] > 0
			|| _chromosome.genesList[tileMap_length * startPos_X + ( startPos_Y - 1 )].type == GeneType.Empty)
			{
				visitedNeighbor++;
			}
		}
		else
		{
			visitedNeighbor++;
		}
		// Up
		if (startPos_X - 1 >= 0)
		{
			if (_beVisited[startPos_X - 1, startPos_Y] > 0
			|| _chromosome.genesList[tileMap_length * ( startPos_X - 1 ) + startPos_Y].type == GeneType.Empty)
			{
				visitedNeighbor++;
			}
		}
		else
		{
			visitedNeighbor++;
		}

		if (visitedNeighbor == 4)
		{
			/// 原地踏步(拜訪)
			_beVisited[startPos_X, startPos_Y]++;
			//Debug.Log(startPos_X + ", " + startPos_Y);
		}
		#endregion

		/// 往回走
		#region 往回走
		// Right
		if (startPos_Y + 1 < tileMap_length)
		{
			if (_beVisited[startPos_X, startPos_Y + 1] == 1
				&& _chromosome.genesList[tileMap_length * startPos_X + ( startPos_Y + 1 )].type == GeneType.Forbidden)
			{
				// Search!!
				DFS(startPos_X, startPos_Y + 1, _rangePosition, _chromosome, _beVisited);
			}
		}
		// Down
		if (startPos_X + 1 < tileMap_width)
		{
			if (_beVisited[startPos_X + 1, startPos_Y] == 1
			&& _chromosome.genesList[tileMap_length * ( startPos_X + 1 ) + startPos_Y].type == GeneType.Forbidden)
			{
				// Search!!
				DFS(startPos_X + 1, startPos_Y, _rangePosition, _chromosome, _beVisited);
			}
		}
		// Left
		if (startPos_Y - 1 >= 0)
		{
			if (_beVisited[startPos_X, startPos_Y - 1] == 1
				&& _chromosome.genesList[tileMap_length * startPos_X + ( startPos_Y - 1 )].type == GeneType.Forbidden)
			{
				// Search!!
				DFS(startPos_X, startPos_Y - 1, _rangePosition, _chromosome, _beVisited);
			}
		}
		// Up
		if (startPos_X - 1 >= 0)
		{
			if (_beVisited[startPos_X - 1, startPos_Y] == 1
				&& _chromosome.genesList[tileMap_length * ( startPos_X - 1 ) + startPos_Y].type == GeneType.Forbidden)
			{
				// Search!!
				DFS(startPos_X - 1, startPos_Y, _rangePosition, _chromosome, _beVisited);
			}
		}
		#endregion
	}
	#endregion

	#region Test_GameObject_Crossover

	List<List<GameObjectInfo>> _childsGameObjectListPopulation = new List<List<GameObjectInfo>>();

	// Determine which type of gameobject is the first one to crossover.
	int[] RandomGameObjectType()
	{
		// Initial the array.
		int[] indexGameObjectTypeArray = new int[(int)GeneGameObjectAttribute.NumberOfGeneSpaceAttribute - 1];
		for (int index = 0; index < (int)GeneGameObjectAttribute.NumberOfGeneSpaceAttribute - 1; index++)
		{
			indexGameObjectTypeArray[index] = index + 1;
		}

		// Random the index array.
		for (int index = indexGameObjectTypeArray.Length - 1; index > 0; index--)
		{
			int randomArrayIndex = Random.Range(0, index);
			int tempValue = indexGameObjectTypeArray[randomArrayIndex];
			// Swap the value with the last one.
			indexGameObjectTypeArray[randomArrayIndex] = indexGameObjectTypeArray[index];
			indexGameObjectTypeArray[index] = tempValue;
		}
		return indexGameObjectTypeArray;
	}
	// Search each kind of Game Object in Parent
	List<List<GameObjectInfo>> SearchGameObject(List<GameObjectInfo> GameObjectList)
	{
		List<List<GameObjectInfo>> GameObjectPositionList = new List<List<GameObjectInfo>>();
		// Initial
		for (int num = 0; num < (int)GeneGameObjectAttribute.NumberOfGeneSpaceAttribute - 1; num++)
		{
			GameObjectPositionList.Add(new List<GameObjectInfo>());
		}
		foreach (var gameObject in GameObjectList)
		{
			switch (gameObject.GameObjectAttribute)
			{
				case GeneGameObjectAttribute.entrance:
					GameObjectPositionList[0].Add(new GameObjectInfo());
					GameObjectPositionList[0][GameObjectPositionList[0].Count - 1].Position = gameObject.Position;
					GameObjectPositionList[0][GameObjectPositionList[0].Count - 1].GameObjectAttribute = gameObject.GameObjectAttribute;
					break;
				case GeneGameObjectAttribute.exit:
					GameObjectPositionList[1].Add(new GameObjectInfo());
					GameObjectPositionList[1][GameObjectPositionList[1].Count - 1].Position = gameObject.Position;
					GameObjectPositionList[1][GameObjectPositionList[1].Count - 1].GameObjectAttribute = gameObject.GameObjectAttribute;
					break;
				case GeneGameObjectAttribute.enemy:
					GameObjectPositionList[2].Add(new GameObjectInfo());
					GameObjectPositionList[2][GameObjectPositionList[2].Count - 1].Position = gameObject.Position;
					GameObjectPositionList[2][GameObjectPositionList[2].Count - 1].GameObjectAttribute = gameObject.GameObjectAttribute;
					break;
				case GeneGameObjectAttribute.trap:
					GameObjectPositionList[3].Add(new GameObjectInfo());
					GameObjectPositionList[3][GameObjectPositionList[3].Count - 1].Position = gameObject.Position;
					GameObjectPositionList[3][GameObjectPositionList[3].Count - 1].GameObjectAttribute = gameObject.GameObjectAttribute;
					break;
				case GeneGameObjectAttribute.treasure:
					GameObjectPositionList[4].Add(new GameObjectInfo());
					GameObjectPositionList[4][GameObjectPositionList[4].Count - 1].Position = gameObject.Position;
					GameObjectPositionList[4][GameObjectPositionList[4].Count - 1].GameObjectAttribute = gameObject.GameObjectAttribute;
					break;
			}
		}
		return GameObjectPositionList;
	}

	List<GameObjectInfo> gameObjectListCopy(List<List<GameObjectInfo>> originalgGameObjectList)
	{
		var GameObjectListClone = new List<GameObjectInfo>();
		foreach (var type in originalgGameObjectList)
		{
			foreach (var item in type)
			{
				GameObjectListClone.Add(new GameObjectInfo());
				GameObjectListClone[GameObjectListClone.Count - 1].Position = item.Position;
				GameObjectListClone[GameObjectListClone.Count - 1].GameObjectAttribute = item.GameObjectAttribute;
			}				
		}
		return GameObjectListClone;
	}

	void CrossoverMethod(List<GameObjectInfo> parent_1, List<GameObjectInfo> parent_2)
	{
		List<List<GameObjectInfo>> child_1_GameObjectPositionList = SearchGameObject(parent_1);
		List<List<GameObjectInfo>> child_2_GameObjectPositionList = SearchGameObject(parent_2);

		// Save the position of Game Object
		List<List<GameObjectInfo>> Parent_1_GameObjectPositionList = SearchGameObject(parent_1);
		List<List<GameObjectInfo>> Parent_2_GameObjectPositionList = SearchGameObject(parent_2);
		
		//// Random select the type of game object which need to crossocver
		//int[] gameObjectTypeArray = RandomGameObjectType();
		//int numCrossoverGameObject = Random.Range(1, (int)GeneGameObjectAttribute.NumberOfGeneSpaceAttribute); // The number of GameObject type need to crossover.
		
		//// Set the value to know that this game object will crossover or not.
		//int[] isCrossoverArray = new int[(int)GeneGameObjectAttribute.NumberOfGeneSpaceAttribute - 1];
		//for (int typeCrossover = 0; typeCrossover < numCrossoverGameObject; typeCrossover++)
		//{
		//	isCrossoverArray[gameObjectTypeArray[typeCrossover] - 1] = 1;
		//}

		//for (int i = 0; i < isCrossoverArray.Length; i++)
		//{
		//	Debug.Log(isCrossoverArray[i]);
		//}

		//Debug.Log("==================");
		// Start to Crossover
		#region Start to Crossover
		for (int typeGameObject = 0; typeGameObject < (int)GeneGameObjectAttribute.NumberOfGeneSpaceAttribute - 1; typeGameObject++)
		{
			// If parent_1 has this type of game object
			if (Parent_1_GameObjectPositionList[typeGameObject].Count > 0)
			{
				// Start to swap the content of gameObjectList from Parent_2
				for (int indexParent_1 = 0; indexParent_1 < Parent_1_GameObjectPositionList[typeGameObject].Count; indexParent_1++)
				{
					for (int indexParent_2 = 0; indexParent_2 < Parent_2_GameObjectPositionList[typeGameObject].Count; indexParent_2++)
					{
						bool isNone = true;
						foreach (var type in child_1_GameObjectPositionList)
						{
							foreach (var item in type)
							{
								if (item.Position == Parent_2_GameObjectPositionList[typeGameObject][indexParent_2].Position)
								{
									isNone = false;
								}
							}
						}

						if (isNone == true)
						{
							child_1_GameObjectPositionList[typeGameObject][indexParent_1].Position = Parent_2_GameObjectPositionList[typeGameObject][indexParent_2].Position;
							break;
						}
					}
				}
			}
			// If parent_2 has this type of game object
			if (Parent_2_GameObjectPositionList[typeGameObject].Count > 0)
			{
				// Start to swap the content of gameObjectList from Parent_1
				for (int indexParent_2 = 0; indexParent_2 < Parent_2_GameObjectPositionList[typeGameObject].Count; indexParent_2++)
				{
					for (int indexParent_1 = 0; indexParent_1 < Parent_1_GameObjectPositionList[typeGameObject].Count; indexParent_1++)
					{
						bool isNone = true;
						foreach (var type in child_2_GameObjectPositionList)
						{
							foreach (var item in type)
							{
								if (item.Position == Parent_1_GameObjectPositionList[typeGameObject][indexParent_1].Position)
								{
									isNone = false;
								}
							}
						}

						if (isNone == true)
						{
							child_2_GameObjectPositionList[typeGameObject][indexParent_2].Position = Parent_1_GameObjectPositionList[typeGameObject][indexParent_1].Position;
							break;
						}
					}
				}
			}
		}
		#endregion

		_childsGameObjectListPopulation.Add(gameObjectListCopy(child_1_GameObjectPositionList));
		_childsGameObjectListPopulation.Add(gameObjectListCopy(child_2_GameObjectPositionList));
	}

	#endregion

	#region Test_GameObject_Mutation
	int[] mutateGameObjectTypeArray;

	void RandomMutateGameObjectType()
	{
		// The count of GameObjectType
		int count_GameObjectType = (int)GeneGameObjectAttribute.NumberOfGeneSpaceAttribute - 1;

		// Initial the array.
		mutateGameObjectTypeArray = new int[count_GameObjectType];
		for (int index = 0; index < count_GameObjectType; index++)
		{
			mutateGameObjectTypeArray[index] = index + 1;
		}

		// Random the index array.
		for (int index = count_GameObjectType - 1; index > 0; index--)
		{
			int randomArrayIndex = Random.Range(0, index);
			int tempValue = mutateGameObjectTypeArray[randomArrayIndex];
			// Swap the value with the last one.
			mutateGameObjectTypeArray[randomArrayIndex] = mutateGameObjectTypeArray[index];
			mutateGameObjectTypeArray[index] = tempValue;
		}
	}

	void MutationMethod(List<GameObjectInfo> originalGameObjectList)
	{
		RandomMutateGameObjectType();

		int count_currentGameObject = 0;
		int startIndexGameObject = 0;
		for (int index = 0; index < mutateGameObjectTypeArray.Length; index++)
		{
			// To calculate the number of gameObject which be mutation
			count_currentGameObject = 0;
			// Find the start index of this game object.
			startIndexGameObject = 0;
			for (int indexGameObject = 0; indexGameObject < originalGameObjectList.Count; indexGameObject++)
			{
				if (originalGameObjectList[indexGameObject].GameObjectAttribute == (GeneGameObjectAttribute)mutateGameObjectTypeArray[index])
				{
					count_currentGameObject++;
					startIndexGameObject = indexGameObject;
				}				
			}
			if (count_currentGameObject != 0)
			{
				startIndexGameObject = startIndexGameObject - count_currentGameObject + 1;
			}
			else
			{
				startIndexGameObject = 0;
			}
			// Randomly determine Add or delete the same game object.
			int randomDetermine = Random.Range(0, 2);
			// Add the same game object
			if (randomDetermine == 0)
			{
				// Add
				if (count_currentGameObject < _numMaxGameObject[mutateGameObjectTypeArray[index] - 1])
				{
					Debug.Log("ADD => " + (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);
					AddDeleteGameObject(originalGameObjectList, EmptyTiles, true, count_currentGameObject, startIndexGameObject, (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);
					break;
				}
				else
				{
					// Turn to delete 
					if (count_currentGameObject > _numMinGameObject[mutateGameObjectTypeArray[index] - 1])
					{
						Debug.Log("DELETE => " + (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);
						AddDeleteGameObject(originalGameObjectList, EmptyTiles, false, count_currentGameObject, startIndexGameObject, (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);
						break;
					}
				}
			}
			// Delete the same game object
			else
			{
				// Delete
				if (count_currentGameObject > _numMinGameObject[mutateGameObjectTypeArray[index] - 1])
				{
					Debug.Log("DELETE => " + (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);				
					AddDeleteGameObject(originalGameObjectList, EmptyTiles, false, count_currentGameObject, startIndexGameObject, (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);
					break;
				}
				else
				{
					// Turn to add
					if (count_currentGameObject < _numMaxGameObject[mutateGameObjectTypeArray[index] - 1])
					{
						Debug.Log("ADD => " + (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);
						AddDeleteGameObject(originalGameObjectList, EmptyTiles, true, count_currentGameObject, startIndexGameObject, (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);
						break;
					}
				}
			}
		}

	}

	void AddDeleteGameObject(List<GameObjectInfo> originalGameObjectList, List<int> EmptyTiles, bool isAdd, int num_originalGameObject, int startIndexGameObject, GeneGameObjectAttribute gameObjectType)
	{
		if (isAdd == true)
		{
			// Find the position which will be add the game object 
			int newPosition = 0;
			bool isFindPosition = false;
			while (isFindPosition != true)
			{
				newPosition = EmptyTiles[Random.Range(0, EmptyTiles.Count)];
				isFindPosition = true;
				// Check the position is empty!!
				foreach (var originalGameObject in originalGameObjectList)
				{
					if (originalGameObject.Position == newPosition)
					{
						isFindPosition = false;
					}
				}
			}

			GameObjectInfo newGameObject = new GameObjectInfo();
			newGameObject.Position = newPosition;
			newGameObject.GameObjectAttribute = gameObjectType;
			// Add!!
			originalGameObjectList.Insert(( startIndexGameObject + num_originalGameObject), newGameObject);
		}
		else
		{
			// Find the position which will be delete the game object.
			int index_deletePosition = Random.Range(0, num_originalGameObject);
			originalGameObjectList.RemoveAt(startIndexGameObject + index_deletePosition);
		}
	}
	#endregion

	#region Fitness_Defense
	public float Fitness_Defense(Chromosome chromosome, int length, int width)
	{
		float fitnessScore = 0.0f;
		List<int> posEnemy = new List<int>();
		List<int> posTrap = new List<int>();
		List<int> posExit = new List<int>();
		List<int> posTreasure = new List<int>();
		// Calculate the number of game object
		foreach (var item in chromosome.gameObjectList)
		{
			if (item.GameObjectAttribute == GeneGameObjectAttribute.enemy)
			{
				posEnemy.Add(item.Position);
			}
			if (item.GameObjectAttribute == GeneGameObjectAttribute.trap)
			{
				posTrap.Add(item.Position);
			}
			if (item.GameObjectAttribute == GeneGameObjectAttribute.exit)
			{
				posExit.Add(item.Position);
			}
			if (item.GameObjectAttribute == GeneGameObjectAttribute.treasure)
			{
				posTreasure.Add(item.Position);
			}
		}

		// Fitness how will the space impact on Defense pattern!!
		float fitness_space = 0.0f;
		// Fitness how will the game object impact on Defense pattern!!
		float fitness_GameObject = 0.0f;
		List<int> enemySpaceIndex = findGuardSpaceIndex(chromosome, length, width, posEnemy);
		List<int> trapSpaceIndex = findGuardSpaceIndex(chromosome, length, width, posTrap);
		List<int> exitNeighborSpaceIndex;
		List<int> treasureNeighborSpaceIndex;
		foreach (var exit in posExit)
		{
			exitNeighborSpaceIndex = findNeighborSpaceIndex(chromosome, length, width, exit);
			fitness_space = fitness_space + valueSaveSpace(chromosome, exitNeighborSpaceIndex);
			fitness_GameObject = fitness_GameObject + valueBeProtected(chromosome, exitNeighborSpaceIndex, enemySpaceIndex, GeneGameObjectAttribute.enemy);
			fitness_GameObject = fitness_GameObject + valueBeProtected(chromosome, exitNeighborSpaceIndex, trapSpaceIndex, GeneGameObjectAttribute.trap);
		}
		foreach (var treasure in posTreasure)
		{
			treasureNeighborSpaceIndex = findNeighborSpaceIndex(chromosome, length, width, treasure);
			fitness_space = fitness_space + valueSaveSpace(chromosome, treasureNeighborSpaceIndex);
			fitness_GameObject = fitness_GameObject + valueBeProtected(chromosome, treasureNeighborSpaceIndex, enemySpaceIndex, GeneGameObjectAttribute.enemy);
			fitness_GameObject = fitness_GameObject + valueBeProtected(chromosome, treasureNeighborSpaceIndex, trapSpaceIndex, GeneGameObjectAttribute.trap);
		}

		fitness_GameObject = fitness_GameObject / ( ( posEnemy.Count * 1.0f + posTrap.Count * 0.5f ) * ( posExit.Count + posTreasure.Count ) );
		fitness_space = fitness_space / ( posExit.Count + posTreasure.Count );

		fitnessScore = ( fitness_space + fitness_GameObject ) / 2.0f;

		return fitnessScore;
	}

	float valueSaveSpace(Chromosome chromosome, List<int> neighborSpaceIndex)
	{
		float value = 0.0f;
		float num_Corridor = 0;

		foreach (var neighbor in neighborSpaceIndex)
		{
			if (chromosome.spaceList[neighbor].SpaceAttribute == GeneSpaceAttribute.Corridor)
			{
				num_Corridor++;
			}
		}

		value = num_Corridor / (float)neighborSpaceIndex.Count;
		return value;
	}

	float valueBeProtected(Chromosome chromosome, List<int> neighborSpaceIndex, List<int> guardSpaceIndex, GeneGameObjectAttribute guardAttribute)
	{
		float value = 0.0f;
		float num_GuardInNeightbor = 0.0f;
		foreach (var neighbor in neighborSpaceIndex)
		{
			foreach (var guard in guardSpaceIndex)
			{
				if (neighbor == guard)
				{
					num_GuardInNeightbor++;
				}
			}
		}
		if (guardAttribute == GeneGameObjectAttribute.enemy)
		{
			value = num_GuardInNeightbor * 1.0f;
		}
		else if (guardAttribute == GeneGameObjectAttribute.trap)
		{
			value = num_GuardInNeightbor * 0.5f;
		}
		return value;
	}

	List<int> findGuardSpaceIndex(Chromosome chromosome, int length, int width, List<int> posGuard)
	{
		List<int> guardSpaceIndex = new List<int>(); // The neighbor in which index of space

		foreach (var item in posGuard)
		{
			guardSpaceIndex.Add(checkNeighbor(chromosome, length, width, item / length, item % length));
		}
		return guardSpaceIndex;
	}

	List<int> findNeighborSpaceIndex(Chromosome chromosome, int length, int width, int posProtectedObject)
	{
		List<int> protectedObjectNeighbor = findprotectedObjectNeighbor(chromosome, length, width, posProtectedObject); // Neighbor of protected object which can go to protected object
		List<int> protectedObjectNeighborSpaceIndex = new List<int>(); // The neighbor in which index of space

		int index;
		foreach (var item in protectedObjectNeighbor)
		{
			index = checkNeighbor(chromosome, length, width, item / length, item % length);
			int numSameIndex = 0;
			foreach (var saveIndex in protectedObjectNeighborSpaceIndex)
			{
				if (saveIndex == index)
				{
					numSameIndex++;
				}
			}
			if (numSameIndex == 0)
			{
				protectedObjectNeighborSpaceIndex.Add(index);
			}
		}
		return protectedObjectNeighborSpaceIndex;
	}

	List<int> findprotectedObjectNeighbor(Chromosome chromosome, int length, int width, int posProtectedObject)
	{
		int posTop = posProtectedObject - length;
		int posBottom = posProtectedObject + length;
		int posLeft = posProtectedObject - 1;
		int posRight = posProtectedObject + 1;
		int posTopLeft = posTop - 1;
		int posTopRight = posTop + 1;
		int posBottomLeft = posBottom - 1;
		int posBottomRight = posBottom + 1;
		int posCenter_x = posProtectedObject / length;
		int posCenter_y = posProtectedObject % length;
		List<int> _protectedObjectNeighbor = new List<int>();

		if (posCenter_x - 1 >= 0)
		{
			if (posCenter_y - 1 >= 0)
			{
				// TopLeft
				if (chromosome.genesList[posTopLeft].type == GeneType.Empty)
				{
					if (chromosome.genesList[posTop].type != GeneType.Forbidden || chromosome.genesList[posLeft].type != GeneType.Forbidden)
					{
						_protectedObjectNeighbor.Add(posTopLeft);
					}
				}
			}
			// Top
			if (chromosome.genesList[posTop].type == GeneType.Empty)
			{
				_protectedObjectNeighbor.Add(posTop);
			}
			if (posCenter_y + 1 < length)
			{
				// TopRight
				if (chromosome.genesList[posTopRight].type == GeneType.Empty)
				{
					if (chromosome.genesList[posTop].type != GeneType.Forbidden || chromosome.genesList[posRight].type != GeneType.Forbidden)
					{
						_protectedObjectNeighbor.Add(posTopRight);
					}
				}
			}
		}
		if (posCenter_y - 1 >= 0)
		{
			// Left
			if (chromosome.genesList[posLeft].type == GeneType.Empty)
			{
				_protectedObjectNeighbor.Add(posLeft);
			}
		}
		if (posCenter_y + 1 < length)
		{
			// Right
			if (chromosome.genesList[posRight].type == GeneType.Empty)
			{
				_protectedObjectNeighbor.Add(posRight);
			}
		}
		if (posCenter_x + 1 < width)
		{
			if (posCenter_y - 1 >= 0)
			{
				// BottomLeft
				if (chromosome.genesList[posBottomLeft].type == GeneType.Empty)
				{
					if (chromosome.genesList[posBottom].type != GeneType.Forbidden || chromosome.genesList[posLeft].type != GeneType.Forbidden)
					{
						_protectedObjectNeighbor.Add(posBottomLeft);
					}
				}
			}
			// Bottom
			if (chromosome.genesList[posBottom].type == GeneType.Empty)
			{
				_protectedObjectNeighbor.Add(posBottom);
			}
			if (posCenter_y + 1 < length)
			{
				// BottomRight
				if (chromosome.genesList[posBottomRight].type == GeneType.Empty)
				{
					if (chromosome.genesList[posBottom].type != GeneType.Forbidden || chromosome.genesList[posRight].type != GeneType.Forbidden)
					{
						_protectedObjectNeighbor.Add(posBottomRight);
					}
				}
			}
		}
		return _protectedObjectNeighbor;
	}
	#endregion

}
