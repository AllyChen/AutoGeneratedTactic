using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


using ChromosomeDefinition;
using GeneticAlgorithmSettingDefinition;
using NesScripts.Controls.PathFind;

public class FitnessFunctions {

	public float Fitness_Impassable(Chromosome chromosome, int numGene)
	{
		float fitnessScore = 0.0f;
		int numPassable = 0;
		int numImpassable = 0;

		for (int x = 0; x < numGene; x++)
		{
			if (chromosome.genesList[x].type == GeneType.Forbidden)
			{
				numImpassable++;
			}
			else if(chromosome.genesList[x].type == GeneType.Empty)
			{
				numPassable++;
			}
		}

		fitnessScore = numImpassable;

		return fitnessScore;
	}

	/// <summary>
	/// 計算Impassable的聚集程度。
	/// </summary>
	/// <param name="chromosome"></param>
	/// <param name="numGene"></param>
	/// <param name="length"></param>
	/// <param name="width"></param>
	/// <returns></returns>
	#region Fitness_ImpassableDensity
	public float Fitness_ImpassableDensity(Chromosome chromosome, int numGene, int length, int width)
	{
		float fitnessScore = 0.0f;

		int[,] beVisited = new int[width, length];// 紀錄有無拜訪過
		List<float> subImpassableDensity = new List<float>();// 紀錄各子Impassable的Density

		calculateImpassableDensity(chromosome, length, width, beVisited, subImpassableDensity);

		for (int i = 0; i < subImpassableDensity.Count; i++)
		{
			fitnessScore = fitnessScore + subImpassableDensity[i];
		}

		if (subImpassableDensity.Count != 0)
		{
			fitnessScore = (float)fitnessScore / subImpassableDensity.Count;
		}
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
		return subDensity;
	}

	void DFS(int startPos_X, int startPos_Y, int[] _rangePosition, Chromosome _chromosome, int[,] _beVisited)
	{
		int tileMap_length = _rangePosition[4];
		int tileMap_width = _rangePosition[5];

		// Visit the start position.
		_beVisited[startPos_X, startPos_Y]++;

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

	/// <summary>
	/// 計算Rectangle Quality。
	/// </summary>
	/// <param name="chromosome"></param>
	/// <param name="length"></param>
	/// <param name="width"></param>
	/// <returns></returns>
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

	/// <summary>
	/// 計算Corridor Quality。
	/// </summary>
	/// <param name="chromosome"></param>
	/// <param name="length"></param>
	/// <param name="width"></param>
	/// <returns></returns>
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
					// Set Corridor
					setCorridor(_chromosome, length, width, indexGene, corridorLength, isRowCorridor);
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

	/// <summary>
	/// 計算Empty tiles 是否彼此 Connect。
	/// </summary>
	/// <param name="chromosome"></param>
	/// <param name="length"></param>
	/// <param name="width"></param>
	/// <returns></returns>
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

	/// <summary>
	/// 計算MainPath在EmptyTiles中的比例。
	/// </summary>
	/// <param name="chromosome"></param>
	/// <param name="length"></param>
	/// <param name="width"></param>
	/// <param name="numEmpty"></param>
	/// <returns></returns>
	#region Fitness_MainPathQuality
	public float Fitness_MainPathQuality(Chromosome chromosome, int length, int width, int numEmpty, Grid spaceGrid)
	{
		float fitnessScore = 0.0f;

		FindMainPath(chromosome, length, width, spaceGrid);
		chromosome.settingMainPath();
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
			if(gameObject.GameObjectAttribute == GeneGameObjectAttribute.entrance)
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

	/// <summary>
	/// Fitness_Defense
	/// </summary>
	/// <param name="chromosome"></param>
	/// <param name="length"></param>
	/// <param name="width"></param>
	/// <returns></returns>
	#region Fitness_Defense
	public float Fitness_Defense(Chromosome chromosome, int length, int width, int[]MaxNumObject)
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
		//foreach (var exit in posExit)
		//{
		//	exitNeighborSpaceIndex = findNeighborSpaceIndex(chromosome, length, width, exit);
		//	fitness_space = fitness_space + valueSaveSpace(chromosome, exitNeighborSpaceIndex);
		//	fitness_GameObject = fitness_GameObject + valueBeProtected(chromosome, exitNeighborSpaceIndex, enemySpaceIndex, GeneGameObjectAttribute.enemy);
		//	fitness_GameObject = fitness_GameObject + valueBeProtected(chromosome, exitNeighborSpaceIndex, trapSpaceIndex, GeneGameObjectAttribute.trap);
		//}
		foreach (var treasure in posTreasure)
		{
			treasureNeighborSpaceIndex = findNeighborSpaceIndex(chromosome, length, width, treasure);
			fitness_space = fitness_space + qualityNeighborForbidden(chromosome, length, width, treasure); //valueSaveSpace(chromosome, treasureNeighborSpaceIndex);
			fitness_GameObject = fitness_GameObject + valueBeProtected(chromosome, treasureNeighborSpaceIndex, enemySpaceIndex, GeneGameObjectAttribute.enemy);
			fitness_GameObject = fitness_GameObject + valueBeProtected(chromosome, treasureNeighborSpaceIndex, trapSpaceIndex, GeneGameObjectAttribute.trap);
		}

		//fitness_GameObject = fitness_GameObject / ( ( posEnemy.Count * 1.0f + posTrap.Count * 0.5f ) * ( posExit.Count + posTreasure.Count ) );
		//fitness_space = fitness_space / ( posExit.Count + posTreasure.Count );
		if (MaxNumObject[(int)GeneGameObjectAttribute.treasure - 1] == 0 || ( posEnemy.Count + posTrap.Count ) == 0)
		{
			fitness_GameObject = 0;
		}
		else
		{
			fitness_GameObject = fitness_GameObject / ( ( posEnemy.Count * 1.0f + posTrap.Count * 0.5f ) * ( MaxNumObject[(int)GeneGameObjectAttribute.treasure - 1] ) );
		}
		if (MaxNumObject[(int)GeneGameObjectAttribute.treasure - 1] == 0)
		{
			fitness_space = 0;
		}
		else
		{
			fitness_space = fitness_space / ( MaxNumObject[(int)GeneGameObjectAttribute.treasure - 1] );
		}

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

	// Calculate the quality of treasure 如果周遭為牆壁，代表很安全。
	float qualityNeighborForbidden(Chromosome chromosome, int length, int width, int posTreasure)
	{
		float value = 0.0f;
		int pos_x = posTreasure / length;
		int pos_y = posTreasure % length;
		int numForbidden = 0;

		// Top
		if (pos_x - 1 >= 0)
		{
			if (chromosome.genesList[( pos_x - 1 ) * length + pos_y].type == GeneType.Forbidden)
			{
				numForbidden++;
			}
		}
		// Buttom
		if (pos_x + 1 < width)
		{
			if (chromosome.genesList[( pos_x + 1 ) * length + pos_y].type == GeneType.Forbidden)
			{
				numForbidden++;
			}
		}
		// Left
		if (pos_y - 1 >= 0)
		{
			if (chromosome.genesList[pos_x * length + ( pos_y - 1 )].type == GeneType.Forbidden)
			{
				numForbidden++;
			}
		}
		// Right
		if (pos_y + 1 < length)
		{
			if (chromosome.genesList[pos_x * length + ( pos_y + 1 )].type == GeneType.Forbidden)
			{
				numForbidden++;
			}
		}

		if (numForbidden == 4 || numForbidden == 0)
		{
			value = 0;
		}
		else
		{
			value = numForbidden / 4.0f;
		}

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

	/// <summary>
	/// Fitness_OnMainPath
	/// </summary>
	/// <param name="chromosome"></param>
	/// <param name="length"></param>
	/// <param name="width"></param>
	/// <param name="gameObjectAttribute"></param>
	/// <returns></returns>
	#region Fitness_OnMainPath
	public float Fitness_OnMainPath(Chromosome chromosome, int length, int width, GeneGameObjectAttribute gameObjectAttribute, int[] MaxNumObject)
	{
		float fitnessScore = 0.0f;
		float numOnMainPath = 0.0f; // Number of game object on the main path.

		foreach (var Object in chromosome.gameObjectList)
		{
			if (Object.GameObjectAttribute == gameObjectAttribute)
			{
				if (chromosome.genesList[Object.Position].isMainPath == true)
				{
					numOnMainPath++;
				}
			}
		}
		if (MaxNumObject[(int)gameObjectAttribute - 1] == 0)
		{
			fitnessScore = 0.0f;
		}
		else
		{
			fitnessScore = numOnMainPath / MaxNumObject[(int)gameObjectAttribute - 1];
		}
		return fitnessScore;
	}
	#endregion

	/// <summary>
	/// Fitness_BesideMainPath
	/// </summary>
	/// <param name="chromosome"></param>
	/// <param name="length"></param>
	/// <param name="width"></param>
	/// <param name="gameObjectAttribute"></param>
	/// <returns></returns>
	#region Fitness_BesideMainPath
	public float Fitness_BesideMainPath(Chromosome chromosome, int length, int width, GeneGameObjectAttribute gameObjectAttribute, int[] MaxNumObject)
	{
		float fitnessScore = 0.0f;
		List<int> posGameObject = new List<int>();

		foreach (var Object in chromosome.gameObjectList)
		{
			if (Object.GameObjectAttribute == gameObjectAttribute)
			{
				posGameObject.Add(Object.Position);
			}
		}
		if (MaxNumObject[(int)gameObjectAttribute - 1] == 0 || posGameObject.Count == 0)
		{
			fitnessScore = 0.0f;
		}
		else
		{
			foreach (var position in posGameObject)
			{
				fitnessScore = fitnessScore + gameObjectBesideMainPath(chromosome, length, width, position);
			}

			fitnessScore = fitnessScore / MaxNumObject[(int)gameObjectAttribute - 1];
		}
		return fitnessScore;
	}

	float gameObjectBesideMainPath(Chromosome chromosome, int length, int width, int posGameObject)
	{
		float value = 0.0f;
		int numEmpty = 0;
		int numNeighborMainPath = 0;
		int posTop = posGameObject - length;
		int posBottom = posGameObject + length;
		int posLeft = posGameObject - 1;
		int posRight = posGameObject + 1;
		int posTopLeft = posTop - 1;
		int posTopRight = posTop + 1;
		int posBottomLeft = posBottom - 1;
		int posBottomRight = posBottom + 1;
		int posCenter_x = posGameObject / length;
		int posCenter_y = posGameObject % length;
		List<int> _emptyNeighbor = new List<int>();

		if (chromosome.genesList[posGameObject].isMainPath == true)
		{
			value = 0.0f;
		}
		else
		{
			#region findNeighborWhichIsEmpty
			if (posCenter_x - 1 >= 0)
			{
				if (posCenter_y - 1 >= 0)
				{
					// TopLeft
					if (chromosome.genesList[posTopLeft].type == GeneType.Empty)
					{
						if (chromosome.genesList[posTop].type != GeneType.Forbidden || chromosome.genesList[posLeft].type != GeneType.Forbidden)
						{
							numEmpty++;
							if (chromosome.genesList[posTopLeft].isMainPath == true)
							{
								numNeighborMainPath++;
							}
						}
					}
				}
				// Top
				if (chromosome.genesList[posTop].type == GeneType.Empty)
				{
					numEmpty++;
					if (chromosome.genesList[posTop].isMainPath == true)
					{
						numNeighborMainPath++;
					}
				}
				if (posCenter_y + 1 < length)
				{
					// TopRight
					if (chromosome.genesList[posTopRight].type == GeneType.Empty)
					{
						if (chromosome.genesList[posTop].type != GeneType.Forbidden || chromosome.genesList[posRight].type != GeneType.Forbidden)
						{
							numEmpty++;
							if (chromosome.genesList[posTopRight].isMainPath == true)
							{
								numNeighborMainPath++;
							}
						}
					}
				}
			}
			if (posCenter_y - 1 >= 0)
			{
				// Left
				if (chromosome.genesList[posLeft].type == GeneType.Empty)
				{
					numEmpty++;
					if (chromosome.genesList[posLeft].isMainPath == true)
					{
						numNeighborMainPath++;
					}
				}
			}
			if (posCenter_y + 1 < length)
			{
				// Right
				if (chromosome.genesList[posRight].type == GeneType.Empty)
				{
					numEmpty++;
					if (chromosome.genesList[posRight].isMainPath == true)
					{
						numNeighborMainPath++;
					}
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
							numEmpty++;
							if (chromosome.genesList[posBottomLeft].isMainPath == true)
							{
								numNeighborMainPath++;
							}
						}
					}
				}
				// Bottom
				if (chromosome.genesList[posBottom].type == GeneType.Empty)
				{
					numEmpty++;
					if (chromosome.genesList[posBottom].isMainPath == true)
					{
						numNeighborMainPath++;
					}
				}
				if (posCenter_y + 1 < length)
				{
					// BottomRight
					if (chromosome.genesList[posBottomRight].type == GeneType.Empty)
					{
						if (chromosome.genesList[posBottom].type != GeneType.Forbidden || chromosome.genesList[posRight].type != GeneType.Forbidden)
						{
							numEmpty++;
							if (chromosome.genesList[posBottomRight].isMainPath == true)
							{
								numNeighborMainPath++;
							}
						}
					}
				}
			}
			#endregion

			value = (float)numNeighborMainPath / (float)numEmpty;
		}
		return value;
	}
	#endregion
}
