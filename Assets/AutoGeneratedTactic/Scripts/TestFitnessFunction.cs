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
	{1,1,1,0,0,0,0,0},//  00 01 02 03 04 05 06 07
	{1,1,1,0,0,0,0,1},//  08 09 10 11 12 13 14 15
	{1,1,1,0,0,0,0,1},//  16 17 18 19 20 21 22 23
	{1,1,1,0,0,0,0,1},//  24 25 26 27 28 29 30 31
	{0,0,0,0,0,0,0,0},//  32 33 34 35 36 37 38 39
	{0,0,0,0,0,0,0,1},//  40 41 42 43 44 45 46 47
	{1,1,1,0,0,0,0,1},//  48 49 50 51 52 53 54 55
	{1,1,1,0,0,0,0,1}};// 56 57 58 59 60 61 62 63

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
		EmptyTiles.Clear();
		InitialTestMap(TestMapArray, TestMap);
		TestMap.AddGameObjectInList(32, GeneGameObjectAttribute.entrance);
		TestMap.AddGameObjectInList(39, GeneGameObjectAttribute.exit);
		TestMap.AddGameObjectInList(4, GeneGameObjectAttribute.enemy);
		TestMap.AddGameObjectInList(5, GeneGameObjectAttribute.enemy);
		TestMap.AddGameObjectInList(60, GeneGameObjectAttribute.enemy);
		TestMap.AddGameObjectInList(61, GeneGameObjectAttribute.enemy);
		//TestMap.AddGameObjectInList(38, GeneGameObjectAttribute.trap);
		//TestMap.AddGameObjectInList(54, GeneGameObjectAttribute.trap);
		//TestMap.AddGameObjectInList(46, GeneGameObjectAttribute.treasure);
		// Calculate the number of empty tiles
		for (int indexGene = 0; indexGene < TestMap.genesList.Count; indexGene++)
		{
			if (TestMap.genesList[indexGene].type == GeneType.Empty)
			{
				EmptyTiles.Add(indexGene);
			}
		}

		// Fitness function
		float weight_RectangleQuality = 0.0f;
		float weight_CorridorQuality = 1.0f;
		float weight_ConnectedQuality = 1.0f;
		TestMap.FitnessScore[FitnessFunctionName.RectangleQuality] = FitnessFunction.Fitness_RectangleQuality(TestMap, _mapLength, _mapWidth);
		TestMap.FitnessScore[FitnessFunctionName.CorridorQuality] = FitnessFunction.Fitness_CorridorQuality(TestMap, _mapLength, _mapWidth);
		TestMap.FitnessScore[FitnessFunctionName.ConnectedQuality] = FitnessFunction.Fitness_ConnectedQuality(TestMap, _mapLength, _mapWidth);
		TestMap.FitnessScore[FitnessFunctionName.SumOfFitnessScore] = ( TestMap.FitnessScore[FitnessFunctionName.RectangleQuality] * weight_RectangleQuality
																			+ TestMap.FitnessScore[FitnessFunctionName.CorridorQuality] * weight_CorridorQuality
																			+ TestMap.FitnessScore[FitnessFunctionName.ConnectedQuality] * weight_ConnectedQuality ) / 3.0f;
		float weight_MainPathQuality = 1.0f;
		float weight_Fitness_Defense = 1.0f;
		TestMap.FitnessScore[FitnessFunctionName.MainPathQuality] = FitnessFunction.Fitness_MainPathQuality(TestMap, _mapLength, _mapWidth, EmptyTiles.Count, spaceGrid);
		TestMap.FitnessScore[FitnessFunctionName.Fitness_Defense] = FitnessFunction.Fitness_Defense(TestMap, _mapLength, _mapWidth, _numMaxGameObject);
		TestMap.FitnessScore[FitnessFunctionName.SumOfFitnessScore] = ( TestMap.FitnessScore[FitnessFunctionName.MainPathQuality] * weight_MainPathQuality
																			+ TestMap.FitnessScore[FitnessFunctionName.Fitness_Defense] * weight_Fitness_Defense ) / 2.0f;


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

	#region Fitness_TwoPronged



	
	#endregion
}
