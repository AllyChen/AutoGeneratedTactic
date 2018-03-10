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
	{0,0,0,0,0,0,1,1},//  00 01 02 03 04 05 06 07
	{0,0,0,0,1,1,1,1},//  08 09 10 11 12 13 14 15
	{0,1,1,0,1,1,1,1},//  16 17 18 19 20 21 22 23
	{0,1,1,0,0,0,1,1},//  24 25 26 27 28 29 30 31
	{0,0,0,0,0,1,1,1},//  32 33 34 35 36 37 38 39
	{1,1,1,0,0,1,1,1},//  40 41 42 43 44 45 46 47
	{0,0,0,0,0,0,1,1},//  48 49 50 51 52 53 54 55
	{1,1,1,1,1,1,0,0}};// 56 57 58 59 60 61 62 63

	private Chromosome TestMap = new Chromosome();

	void Start()
	{
		//InitialTestMap();
		//float test1 = Fitness_RectangleQuality(TestMap, 8, 8);
		//float test2 = Fitness_CorridorQuality(TestMap, 8, 8);
		////detectedSpaceAttribute(TestMap);
		////for (int i = 0; i < TestMap.spaceList.Count; i++)
		////{
		////	Debug.Log("==Index_" + i + "==");
		////	Debug.Log("startPos = " + TestMap.spaceList[i].startPos);
		////	Debug.Log("length = " + TestMap.spaceList[i].length);
		////	Debug.Log("width = " + TestMap.spaceList[i].width);
		////	Debug.Log("SpaceAttribute = " + TestMap.spaceList[i].SpaceAttribute);
		////}
		//Debug.Log(Fitness_ConnectedQuality(TestMap, 8, 8));
	}
	#region InitialTestMap
	public void InitialTestMap()
	{
		// Create the genes in each chromosomes.
		for (int x = 0; x < 8; x++)
		{
			for (int y = 0; y < 8; y++)
			{
				TestMap.genesList.Add(new Gene());
				if (TestMapArray[x, y] == 0)
				{
					TestMap.genesList[8 * x + y].type = GeneType.Empty;
				}
				else if (TestMapArray[x, y] == 1)
				{
					TestMap.genesList[8 * x + y].type = GeneType.Forbidden;
				}
			}
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
			if (isConnected(chromosome, length, width) == true)
			{
				fitnessScore = 1.0f;
			}
			else
			{
				fitnessScore = 0.0f;
			}
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

	bool isConnected(Chromosome _chromosome, int length, int width)
	{
		#region A-star
		//// create the tiles map
		//bool[,] tilesmap = new bool[8, 8];
		//// set values here....
		//// true = walkable, false = blocking
		//for (int x = 0; x < width; x++)
		//{
		//	for (int y = 0; y < length; y++)
		//	{
		//		if (chromosome[x * length + y].type == GeneType.Empty)
		//		{
		//			tilesmap[x, y] = true;
		//		}
		//		else if (chromosome[x * length + y].type == GeneType.Forbidden)
		//		{
		//			tilesmap[x, y] = false;
		//		}
		//	}
		//}
		//// create a grid
		//Grid grid = new Grid(tilesmap);

		//// Find the first empty tile
		//bool isFirstEmptyTile = false;
		//int indexFirstEmptyTile = 0;
		//while (isFirstEmptyTile == false)
		//{
		//	if (chromosome[indexFirstEmptyTile].type == GeneType.Empty)
		//	{
		//		isFirstEmptyTile = true;
		//	}
		//	else
		//	{
		//		indexFirstEmptyTile++;
		//	}
		//}

		//// Start to check the map is connected.
		//// create source and target points
		//Point _from = new Point(indexFirstEmptyTile / length, indexFirstEmptyTile % length);
		//Point _to = new Point(0, 0);

		//// for Manhattan distance (4 directions)
		//List<Point> pathSearch = new List<Point>();

		//for (int index = indexFirstEmptyTile + 1; index < chromosome.Count; index++)
		//{
		//	pathSearch.Clear();

		//	if (chromosome[index].type == GeneType.Empty)
		//	{
		//		// Initial the goal.
		//		_to.x = index / length;
		//		_to.y = index % length;
		//		// for Manhattan distance (4 directions)
		//		pathSearch = Pathfinding.FindPath(grid, _from, _to, Pathfinding.DistanceType.Manhattan);

		//		// NOT connect
		//		if (pathSearch.Count == 0)
		//		{
		//			isConnected = false;
		//		}
		//	}
		//}
		#endregion
		// Is the empty tiles are connected?
		bool isConnected = true;
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

		visitedConnectedSpace(indexRoot, RootList, checkSpaceConnected);

		for (int i = 0; i < checkSpaceConnected.Length; i++)
		{
			if (checkSpaceConnected[i] == 0)
			{
				isConnected = false;
			}
		}
		return isConnected;
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



}
