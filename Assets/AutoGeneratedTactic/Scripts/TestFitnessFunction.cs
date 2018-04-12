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

		//Debug.Log("====Main Path====");
		//foreach (var path in TestMap.mainPath)
		//{
		//	Debug.Log(path);
		//}

		//Debug.Log("FitnessScore = "+Fitness_TwoPronged(TestMap, _mapLength, _mapWidth));
		
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
	public float Fitness_TwoPronged(Chromosome chromosome, int length, int width, int[] MaxNumObject)
	{
		float fitnessScore = 0.0f;
		List<List<float>> qualityDistance = qualityDistanceEnemy(chromosome, length, width);
		List<List<bool>> checkDistance = checkEnemyDistance(chromosome, length, width, qualityDistance);

		for (int indexMain = 0; indexMain < qualityDistance.Count - 1; indexMain++)
		{
			for (int indexPartner = 1; indexPartner < qualityDistance[indexMain].Count; indexPartner++)
			{
				if (checkDistance[indexMain][indexPartner] == true)
				{
					fitnessScore = fitnessScore + qualityDistance[indexMain][indexPartner] * 1.0f;
				}
				else
				{
					fitnessScore = fitnessScore + qualityDistance[indexMain][indexPartner] * 0.0f;
				}
			}
		}

		int numEnemy = qualityDistance.Count;
		int numDistance = 0;
		for (int i = 0; i < numEnemy; i++)
		{
			numDistance = numDistance + ( numEnemy - 1 - i );
		}

		// Normalize
		fitnessScore = fitnessScore / numDistance;

		return fitnessScore;
	}
	// Calculate the distance of two enemy is in the range or not.
	List<List<float>> qualityDistanceEnemy(Chromosome chromosome, int length, int width)
	{
		float enemy_sensitiveRange = 3.5f;
		float fitnessDistance = enemy_sensitiveRange * 2;
		List<List<float>> EnemyPartnerList = new List<List<float>>(); // The distance between the partner is in the range.

		// The first element of EnemyPartner is the position of EnemyMain.(List->position(element1))
		foreach (var gameObject in chromosome.gameObjectList)
		{
			if (gameObject.GameObjectAttribute == GeneGameObjectAttribute.enemy)
			{
				EnemyPartnerList.Add(new List<float>());
				EnemyPartnerList[EnemyPartnerList.Count - 1].Add(gameObject.Position);
			}
		}
		// Calculate the score of EnemyPartner and store in list.(List->position(element1)->score(element2)->score(element3)->...)
		for (int indexMainEnemy = 0; indexMainEnemy < EnemyPartnerList.Count - 1; indexMainEnemy++)
		{
			int main_x = (int)EnemyPartnerList[indexMainEnemy][0] / length;
			int main_y = (int)EnemyPartnerList[indexMainEnemy][0] % length;

			for (int indexPartnerEnemy = indexMainEnemy + 1; indexPartnerEnemy < EnemyPartnerList.Count; indexPartnerEnemy++)
			{
				int partner_x = (int)EnemyPartnerList[indexPartnerEnemy][0] / length;
				int partner_y = (int)EnemyPartnerList[indexPartnerEnemy][0] % length;

				float distance = (float)Math.Sqrt(Math.Pow(( main_x - partner_x ), 2) + Math.Pow(( main_y - partner_y ), 2));
				float score = ( fitnessDistance - Math.Abs(distance - fitnessDistance) ) / fitnessDistance;

				EnemyPartnerList[indexMainEnemy].Add(score);
			}
		}
		return EnemyPartnerList;
	}
	// Checking that there isn't any forbidden between main enemy and it's partner.
	// Checking that there exist main path between main enemy and it's partner.
	List<List<bool>> checkEnemyDistance(Chromosome chromosome, int length, int width, List<List<float>> EnemyPartnerList)
	{
		List<List<bool>> EnemyLegalDistanceList = new List<List<bool>>(); // The distance between the enemys is legal or not.
		for (int indexMainEnemy = 0; indexMainEnemy < EnemyPartnerList.Count - 1; indexMainEnemy++)
		{
			EnemyLegalDistanceList.Add(new List<bool>());

			EnemyLegalDistanceList[indexMainEnemy].Add(false); // The first element is itself.
			int main_x = (int)EnemyPartnerList[indexMainEnemy][0] / length;
			int main_y = (int)EnemyPartnerList[indexMainEnemy][0] % length;
			for (int indexPartnerEnemy = indexMainEnemy + 1; indexPartnerEnemy < EnemyPartnerList.Count; indexPartnerEnemy++)
			{
				int partner_x = (int)EnemyPartnerList[indexPartnerEnemy][0] / length;
				int partner_y = (int)EnemyPartnerList[indexPartnerEnemy][0] % length;

				EnemyLegalDistanceList[indexMainEnemy].Add(isDistanceLegal(chromosome, length, main_x, main_y, partner_x, partner_y));
			}
		}
		return EnemyLegalDistanceList;
	}
	bool isDistanceLegal(Chromosome chromosome, int length, int main_x, int main_y, int partner_x, int partner_y)
	{
		bool isLegal = true;
		int numMainPath = 0;
		int min_x = 0;
		int min_y = 0;
		int max_x = 0;
		int max_y = 0;

		if (main_x < partner_x)
		{
			min_x = main_x;
			max_x = partner_x;
		}
		else if (main_x > partner_x)
		{
			min_x = partner_x;
			max_x = main_x;
		}
		else if (main_x == partner_x)
		{
			min_x = max_x = main_x;
		}
		if (main_y < partner_y)
		{
			min_y = main_y;
			max_y = partner_y;
		}
		else if (main_y > partner_y)
		{
			min_y = partner_y;
			max_y = main_y;
		}
		else if (main_y == partner_y)
		{
			min_y = max_y = main_y;
		}
		// Checking forbidden and main path.
		for (int x = min_x; x <= max_x; x++)
		{
			for (int y = min_y; y <= max_y; y++)
			{
				if (chromosome.genesList[x * length + y].isMainPath == true)
				{
					numMainPath++;
				}
				if (chromosome.genesList[x * length + y].type == GeneType.Forbidden)
				{
					isLegal = false;
					break;
				}
			}
			if (isLegal == false)
			{
				break;
			}
		}

		if (isLegal == true && numMainPath == 0)
		{
			isLegal = false;
		}

		return isLegal;
	}
	#endregion
}
