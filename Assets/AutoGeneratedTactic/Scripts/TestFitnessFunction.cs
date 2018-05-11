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
	//private int[,] TestMapArray = {
	//{0,1,1,1,1,1,1,1},//  00 01 02 03 04 05 06 07
	//{0,0,0,0,1,1,1,1},//  08 09 10 11 12 13 14 15
	//{0,1,1,0,1,1,1,1},//  16 17 18 19 20 21 22 23
	//{0,1,1,0,1,1,1,1},//  24 25 26 27 28 29 30 31
	//{0,1,1,0,0,0,0,1},//  32 33 34 35 36 37 38 39
	//{0,0,0,0,0,0,0,1},//  40 41 42 43 44 45 46 47
	//{1,1,1,1,1,1,0,1},//  48 49 50 51 52 53 54 55
	//{1,1,1,1,1,1,0,1}};// 56 57 58 59 60 61 62 63

	private int[,] TestMapArray = {
	{1,1,1,0,0,1,0,0},//  00 01 02 03 04 05 06 07
	{0,1,0,0,0,1,0,0},//  08 09 10 11 12 13 14 15
	{0,1,1,1,0,0,0,0},//  16 17 18 19 20 21 22 23
	{0,0,0,0,0,1,0,1},//  24 25 26 27 28 29 30 31
	{1,0,1,0,0,0,0,1},//  32 33 34 35 36 37 38 39
	{0,0,0,0,1,1,0,0},//  40 41 42 43 44 45 46 47
	{0,1,1,1,1,0,0,1},//  48 49 50 51 52 53 54 55
	{0,0,1,0,0,0,1,1}};// 56 57 58 59 60 61 62 63

	private int _mapLength = 8;
	private int _mapWidth = 8;

	private Chromosome TestMap = new Chromosome();
	private Chromosome TestMap2 = new Chromosome();

	int[] _numMinGameObject = new int[5] { 1, 1, 1, 1, 1 };
	int[] _numMaxGameObject = new int[5] { 1, 1, 4, 1 ,1 };

	private List<int> EmptyTiles = new List<int>();
	private Grid spaceGrid;

	void Start()
	{
		//EmptyTiles.Clear();
		//InitialTestMap(TestMapArray, TestMap);
		//TestMap.AddGameObjectInList(57, GeneGameObjectAttribute.entrance);
		//TestMap.AddGameObjectInList(59, GeneGameObjectAttribute.exit);
		//TestMap.AddGameObjectInList(6, GeneGameObjectAttribute.enemy);
		//TestMap.AddGameObjectInList(12, GeneGameObjectAttribute.enemy);
		//TestMap.AddGameObjectInList(23, GeneGameObjectAttribute.enemy);
		//TestMap.AddGameObjectInList(36, GeneGameObjectAttribute.enemy);
		//TestMap.AddGameObjectInList(15, GeneGameObjectAttribute.trap);
		//TestMap.AddGameObjectInList(21, GeneGameObjectAttribute.treasure);
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

		//Test();
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

	#region 
	void Test()
	{
		Debug.Log("Fitness_DefenseScroe = " + TestMap.FitnessScore[FitnessFunctionName.Fitness_Defense]);
		Debug.Log("TestMap.defenseScroe[0] = " + TestMap.defenseScroe[0]);
		Debug.Log("TestMap.defenseScroe[1] = " + TestMap.defenseScroe[1]);
	}


	
	#endregion
}
