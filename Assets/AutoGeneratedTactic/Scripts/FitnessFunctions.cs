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
		float fitnessScore = 0.0f;

		if (isConnected(chromosome.genesList, length, width) == true)
		{
			fitnessScore = 1.0f;
		}
		else
		{
			fitnessScore = 0.0f;
		}

		return fitnessScore;
	}

	bool isConnected(List<Gene> chromosome, int length, int width)
	{
		// Is the empty tiles are connected?
		bool isConnected = true;
		// create the tiles map
		bool[,] tilesmap = new bool[width, length];
		// set values here....
		// true = walkable, false = blocking
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < length; y++)
			{
				if (chromosome[x * length + y].type == GeneType.Empty)
				{
					tilesmap[x, y] = true;
				}
				else if (chromosome[x * length + y].type == GeneType.Forbidden)
				{
					tilesmap[x, y] = false;
				}
			}
		}
		// create a grid
		Grid grid = new Grid(tilesmap);

		// Find the first empty tile
		bool isFirstEmptyTile = false;
		int indexFirstEmptyTile = 0;
		while (isFirstEmptyTile == false)
		{
			if (chromosome[indexFirstEmptyTile].type == GeneType.Empty)
			{
				isFirstEmptyTile = true;
			}
			else
			{
				indexFirstEmptyTile++;
			}
		}

		// Start to check the map is connected.
		// create source and target points
		Point _from = new Point(indexFirstEmptyTile / length, indexFirstEmptyTile % length);
		Point _to = new Point(0, 0);

		// for Manhattan distance (4 directions)
		List<Point> pathSearch = new List<Point>();

		for (int index = indexFirstEmptyTile + 1; index < chromosome.Count; index++)
		{
			pathSearch.Clear();

			if (chromosome[index].type == GeneType.Empty)
			{
				// Initial the goal.
				_to.x = index / length;
				_to.y = index % length;
				// for Manhattan distance (4 directions)
				pathSearch = Pathfinding.FindPath(grid, _from, _to, Pathfinding.DistanceType.Manhattan);

				// NOT connect
				if (pathSearch.Count == 0)
				{
					isConnected = false;
				}
			}
		}
		return isConnected;
	}
	#endregion
}
