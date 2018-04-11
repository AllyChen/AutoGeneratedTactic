using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

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
	{1,1,1,0,1,0,0,1},//  48 49 50 51 52 53 54 55
	{1,1,1,0,0,1,0,1}};// 56 57 58 59 60 61 62 63

	private int _mapLength = 8;
	private int _mapWidth = 8;

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
		//TestMap.AddGameObjectInList(0, GeneGameObjectAttribute.entrance);
		//TestMap.AddGameObjectInList(62, GeneGameObjectAttribute.exit);
		//TestMap.AddGameObjectInList(36, GeneGameObjectAttribute.enemy);
		//TestMap.AddGameObjectInList(38, GeneGameObjectAttribute.enemy);
		////TestMap.AddGameObjectInList(38, GeneGameObjectAttribute.trap);
		////TestMap.AddGameObjectInList(54, GeneGameObjectAttribute.trap);
		//TestMap.AddGameObjectInList(46, GeneGameObjectAttribute.treasure);
		//// Calculate the number of empty tiles
		//for (int indexGene = 0; indexGene < TestMap.genesList.Count; indexGene++)
		//{
		//	if (TestMap.genesList[indexGene].type == GeneType.Empty)
		//	{
		//		EmptyTiles.Add(indexGene);
		//	}
		//}

		//// Fitness function
		//float weight_RectangleQuality = 0.0f;
		//float weight_CorridorQuality = 1.0f;
		//float weight_ConnectedQuality = 1.0f;
		//TestMap.FitnessScore[FitnessFunctionName.RectangleQuality] = FitnessFunction.Fitness_RectangleQuality(TestMap, _mapLength, _mapWidth);
		//TestMap.FitnessScore[FitnessFunctionName.CorridorQuality] = FitnessFunction.Fitness_CorridorQuality(TestMap, _mapLength, _mapWidth);
		//TestMap.FitnessScore[FitnessFunctionName.ConnectedQuality] = FitnessFunction.Fitness_ConnectedQuality(TestMap, _mapLength, _mapWidth);
		//TestMap.FitnessScore[FitnessFunctionName.SumOfFitnessScore] = ( TestMap.FitnessScore[FitnessFunctionName.RectangleQuality] * weight_RectangleQuality
		//																	+ TestMap.FitnessScore[FitnessFunctionName.CorridorQuality] * weight_CorridorQuality
		//																	+ TestMap.FitnessScore[FitnessFunctionName.ConnectedQuality] * weight_ConnectedQuality ) / 3.0f;
		//float weight_MainPathQuality = 1.0f;
		//float weight_Fitness_Defense = 1.0f;
		//TestMap.FitnessScore[FitnessFunctionName.MainPathQuality] = FitnessFunction.Fitness_MainPathQuality(TestMap, _mapLength, _mapWidth, EmptyTiles.Count, spaceGrid);
		//TestMap.FitnessScore[FitnessFunctionName.Fitness_Defense] = FitnessFunction.Fitness_Defense(TestMap, _mapLength, _mapWidth, _numMaxGameObject);
		//TestMap.FitnessScore[FitnessFunctionName.SumOfFitnessScore] = ( TestMap.FitnessScore[FitnessFunctionName.MainPathQuality] * weight_MainPathQuality
		//																	+ TestMap.FitnessScore[FitnessFunctionName.Fitness_Defense] * weight_Fitness_Defense ) / 2.0f;

		//Debug.Log(qualityNeighborForbidden(TestMap, _mapLength, _mapWidth, 0) + qualityNeighborMainPath(TestMap, _mapLength, _mapWidth, 0));

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
	bool[] emptyNeightbors = { true, true, true, true, true, true, true, true }; // 0:TopLeft 1:Top 2:TopRight 3:Left 4:Right 5:ButtomLeft 6:Buttom 7:ButtomRight

	float qualityNeighborForbidden(Chromosome chromosome, int length, int width, int posTreasure)
	{
		float value = 0.0f;
		int pos_x = posTreasure / length;
		int pos_y = posTreasure % length;
		int posTop = posTreasure - length;
		int posBottom = posTreasure + length;
		int posLeft = posTreasure - 1;
		int posRight = posTreasure + 1;
		int posTopLeft = posTop - 1;
		int posTopRight = posTop + 1;
		int posBottomLeft = posBottom - 1;
		int posBottomRight = posBottom + 1;
		int numForbidden = 0;

		if (pos_x - 1 >= 0)
		{
			if (pos_y - 1 >= 0)
			{
				// TopLeft
				if (chromosome.genesList[posTop].type == GeneType.Empty || chromosome.genesList[posLeft].type == GeneType.Empty)
				{
					if (chromosome.genesList[posTopLeft].type == GeneType.Forbidden)
					{
						emptyNeightbors[0] = false;
					}
				}
			}
			// Top
			if (chromosome.genesList[posTop].type == GeneType.Forbidden)
			{
				emptyNeightbors[1] = false;
			}
			if (pos_y + 1 < length)
			{
				// TopRight
				if (chromosome.genesList[posTop].type == GeneType.Empty || chromosome.genesList[posRight].type == GeneType.Empty)
				{
					if (chromosome.genesList[posTopRight].type == GeneType.Forbidden)
					{
						emptyNeightbors[2] = false;
					}
				}
			}
		}
		else
		{
			emptyNeightbors[0] = false;
			emptyNeightbors[1] = false;
			emptyNeightbors[2] = false;
		}
		if (pos_y - 1 >= 0)
		{
			// Left
			if (chromosome.genesList[posLeft].type == GeneType.Forbidden)
			{
				emptyNeightbors[3] = false;
			}
		}
		else
		{
			emptyNeightbors[0] = false;
			emptyNeightbors[3] = false;
			emptyNeightbors[5] = false;
		}
		if (pos_y + 1 < length)
		{
			// Right
			if (chromosome.genesList[posRight].type == GeneType.Forbidden)
			{
				emptyNeightbors[4] = false;
			}
		}
		else
		{
			emptyNeightbors[2] = false;
			emptyNeightbors[4] = false;
			emptyNeightbors[7] = false;
		}
		if (pos_x + 1 < width)
		{
			if (pos_y - 1 >= 0)
			{
				// BottomLeft
				if (chromosome.genesList[posBottom].type == GeneType.Empty || chromosome.genesList[posLeft].type == GeneType.Empty)
				{
					if (chromosome.genesList[posBottomLeft].type == GeneType.Forbidden)
					{
						emptyNeightbors[5] = false;
					}
				}
			}
			// Bottom
			if (chromosome.genesList[posBottom].type == GeneType.Forbidden)
			{
				emptyNeightbors[6] = false;
			}
			if (pos_y + 1 < length)
			{
				// BottomRight
				if (chromosome.genesList[posBottom].type == GeneType.Empty || chromosome.genesList[posRight].type == GeneType.Empty)
				{
					if (chromosome.genesList[posBottomRight].type == GeneType.Forbidden)
					{
						emptyNeightbors[7] = false;
					}
				}
			}
		}
		else
		{
			emptyNeightbors[5] = false;
			emptyNeightbors[6] = false;
			emptyNeightbors[7] = false;
		}

		// Double Check Four corners
		// TopLeft
		if (emptyNeightbors[1] == false && emptyNeightbors[3] == false)
		{
			emptyNeightbors[0] = false;
		}
		// TopRight
		if (emptyNeightbors[1] == false && emptyNeightbors[4] == false)
		{
			emptyNeightbors[2] = false;
		}
		// ButtomLeft
		if (emptyNeightbors[3] == false && emptyNeightbors[6] == false)
		{
			emptyNeightbors[5] = false;
		}
		// ButtomRight
		if (emptyNeightbors[4] == false && emptyNeightbors[6] == false)
		{
			emptyNeightbors[7] = false;
		}

		for (int index = 0; index < 8; index++)
		{
			if (emptyNeightbors[index] == false)
			{
				Debug.Log(index);
				numForbidden++;
			}
		}

		if (numForbidden == 8 || numForbidden == 0)
		{
			value = 0;
		}
		else
		{
			value = numForbidden / 8.0f;
		}

		Debug.Log("numForbidden = " + numForbidden);

		return value;
	}

	float qualityNeighborMainPath(Chromosome chromosome, int length, int width, int posTreasure)
	{
		float value = 0.0f;
		int numEmpty = 0;
		int numMainPath = 0; // The number of tiles which is on main path.

		// The Neightbor which is empty.
		for (int index = 0; index < 8; index++)
		{
			if (emptyNeightbors[index] == true)
			{
				numEmpty++;
				Debug.Log("Empty "+index);
				// Checking the tile which on the main path.
				switch (index)
				{
					case 0:
						if (chromosome.genesList[posTreasure - length - 1].isMainPath == true)
						{
							numMainPath++;
						}
						break;
					case 1:
						if (chromosome.genesList[posTreasure - length].isMainPath == true)
						{
							numMainPath++;
						}
						break;
					case 2:
						if (chromosome.genesList[posTreasure - length + 1].isMainPath == true)
						{
							numMainPath++;
						}
						break;
					case 3:
						if (chromosome.genesList[posTreasure - 1].isMainPath == true)
						{
							numMainPath++;
						}
						break;
					case 4:
						if (chromosome.genesList[posTreasure + 1].isMainPath == true)
						{
							numMainPath++;
						}
						break;
					case 5:
						if (chromosome.genesList[posTreasure + length - 1].isMainPath == true)
						{
							numMainPath++;
						}
						break;
					case 6:
						if (chromosome.genesList[posTreasure + length].isMainPath == true)
						{
							numMainPath++;
						}
						break;
					case 7:
						if (chromosome.genesList[posTreasure + length + 1].isMainPath == true)
						{
							numMainPath++;
						}
						break;
				}
			}
		}
		// The position of Treasure is on empty tile.
		numEmpty++;
		// Checking the position of treasure
		if (chromosome.genesList[posTreasure].isMainPath == true)
		{
			numMainPath++;
		}
		Debug.Log("numEmpty = " + numEmpty);
		Debug.Log("numMainPath = " + numMainPath);
		value = (float)( numEmpty - numMainPath ) / numEmpty;
		return value;
	}
}
