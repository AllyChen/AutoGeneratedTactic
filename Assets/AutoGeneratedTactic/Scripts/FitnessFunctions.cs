﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChromosomeDefinition;
using GeneticAlgorithmSettingDefinition;

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
}
