using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;

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

		//List<GameObjectInfo> parent1 = new List<GameObjectInfo>();
		//List<GameObjectInfo> parent2 = new List<GameObjectInfo>();
		//GameObjectInfo gane11 = new GameObjectInfo();
		//GameObjectInfo gane12 = new GameObjectInfo();
		//GameObjectInfo gane13 = new GameObjectInfo();
		//GameObjectInfo gane14 = new GameObjectInfo();
		//GameObjectInfo gane15 = new GameObjectInfo();
		//GameObjectInfo gane16 = new GameObjectInfo();
		//GameObjectInfo gane17 = new GameObjectInfo();
		//GameObjectInfo gane21 = new GameObjectInfo();
		//GameObjectInfo gane22 = new GameObjectInfo();
		//GameObjectInfo gane23 = new GameObjectInfo();
		//GameObjectInfo gane24 = new GameObjectInfo();
		//GameObjectInfo gane25 = new GameObjectInfo();
		//GameObjectInfo gane26 = new GameObjectInfo();
		//GameObjectInfo gane27 = new GameObjectInfo();

		//gane11.Position = 1;
		//gane12.Position = 2;
		//gane13.Position = 3;
		//gane14.Position = 4;
		//gane15.Position = 5;
		//gane16.Position = 6;
		//gane17.Position = 7;

		//gane21.Position = 1;
		//gane22.Position = 3;
		//gane23.Position = 5;
		//gane24.Position = 7;
		//gane25.Position = 9;
		//gane26.Position = 7;
		//gane27.Position = 2;

		//parent1.Add(gane11);
		//parent1.Add(gane12);
		//parent1.Add(gane13);
		//parent1.Add(gane14);
		//parent1.Add(gane15);
		//parent1.Add(gane16);
		//parent1.Add(gane17);

		//parent2.Add(gane21);
		//parent2.Add(gane22);
		//parent2.Add(gane23);
		//parent2.Add(gane24);
		//parent2.Add(gane25);
		////parent2.Add(gane26);
		////parent2.Add(gane27);

		//PMCrossoverMethod(parent1, parent2);
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

	void PMCrossoverMethod(List<GameObjectInfo> parent_1, List<GameObjectInfo> parent_2)
	{
		List<GameObjectInfo> child_1 = gameObjectListClone(parent_1);
		List<GameObjectInfo> child_2 = gameObjectListClone(parent_2);

		int numSwap = 3;//( child_1.Count > child_2.Count ) ? Random.Range(1, child_2.Count) : Random.Range(1, child_1.Count);
						// start & end point of Child_1
		int start_child_1 = 2;//Random.Range(0, child_1.Count);
		start_child_1 = ( ( start_child_1 + numSwap ) > child_1.Count ) ? ( start_child_1 - numSwap + 1 ) : start_child_1;

		// start & end point of Child_2
		int start_child_2 = 2;//Random.Range(0, child_2.Count);
		start_child_2 = ( ( start_child_2 + numSwap ) > child_2.Count ) ? ( start_child_2 - numSwap + 1 ) : start_child_2;
		#region Dictionary
		// Create the Dictionary about the swap mapping.
		Dictionary<int, int> MappedSwap = new Dictionary<int, int>();
		List<int> orlKey = new List<int>();
		for (int i = 0; i < numSwap; i++)
		{
			int newKey = parent_1[start_child_1 + i].Position;
			int newValue = parent_2[start_child_2 + i].Position;
			bool sameKey = false;

			// If new key is equal old vale. e.g. old:1->3 ,new:3->4 => old:1->3->4 = 1->4 
			foreach (var item in MappedSwap)
			{
				if (item.Value == newKey)
				{
					MappedSwap[item.Key] = newValue;
					sameKey = true;
					break;
				}
			}
			if (sameKey == false)
			{
				MappedSwap.Add(newKey, newValue);
				orlKey.Add(newKey);
			}
		}

		// Find the same key in Dictionary
		foreach (var oK in orlKey)
		{
			if (MappedSwap.ContainsValue(oK) == true)
			{
				foreach (var item in MappedSwap)
				{
					if (item.Value == oK)
					{
						MappedSwap[item.Key] = MappedSwap[oK]; // value = value
						MappedSwap.Remove(oK); // Remove the same one
						break;
					}
				}
			}
		}
		// Double create Dictionary
		Dictionary<int, int> MappedSwapInverted = new Dictionary<int, int>();
		foreach (var item in MappedSwap)
		{
			MappedSwapInverted.Add(item.Value, item.Key);
		}
		#endregion
		Debug.Log("======MappedSwap======");
		foreach (var item in MappedSwap)
		{
			Debug.Log("KEY = " + item.Key + " , VALUE = " + item.Value);
		}
		Debug.Log("======MappedSwapInverted======");
		foreach (var item in MappedSwapInverted)
		{
			Debug.Log("KEY = " + item.Key + " , VALUE = " + item.Value);
		}

		// Create Child 1
		for (int i = 0; i < parent_1.Count; i++)
		{
			int parentPosition;
			if (i < start_child_1)
			{
				parentPosition = parent_1[i].Position;
				if (MappedSwap.ContainsKey(parentPosition) == true)
				{
					child_1[i].Position = MappedSwap[parentPosition];
				}
				else if (MappedSwapInverted.ContainsKey(parentPosition) == true)
				{
					child_1[i].Position = MappedSwapInverted[parentPosition];
				}
			}
			else if (start_child_1 <= i && i < ( start_child_1 + numSwap ))
			{
				child_1[i].Position = parent_2[start_child_2 + ( i - start_child_1 )].Position;
			}
			else if (( start_child_1 + numSwap ) <= i)
			{
				parentPosition = parent_1[i].Position;
				if (MappedSwap.ContainsKey(parentPosition) == true)
				{
					child_1[i].Position = MappedSwap[parentPosition];
				}
				else if (MappedSwapInverted.ContainsKey(parentPosition) == true)
				{
					child_1[i].Position = MappedSwapInverted[parentPosition];
				}
			}
		}
		// Create Child 2
		for (int i = 0; i < parent_2.Count; i++)
		{
			int parentPosition;
			if (i < start_child_2)
			{
				parentPosition = parent_2[i].Position;
				if (MappedSwap.ContainsKey(parentPosition) == true)
				{
					child_2[i].Position = MappedSwap[parentPosition];
				}
				else if (MappedSwapInverted.ContainsKey(parentPosition) == true)
				{
					child_2[i].Position = MappedSwapInverted[parentPosition];
				}
			}
			else if (start_child_2 <= i && i < ( start_child_2 + numSwap ))
			{
				child_2[i].Position = parent_1[start_child_1 + ( i - start_child_2 )].Position;
			}
			else if (( start_child_2 + numSwap ) <= i)
			{
				parentPosition = parent_2[i].Position;
				if (MappedSwap.ContainsKey(parentPosition) == true)
				{
					child_2[i].Position = MappedSwap[parentPosition];
				}
				else if (MappedSwapInverted.ContainsKey(parentPosition) == true)
				{
					child_2[i].Position = MappedSwapInverted[parentPosition];
				}
			}
		}
		Debug.Log("====C1====");
		foreach (var item in child_1)
		{
			Debug.Log(item.Position);
		}
		Debug.Log("====C2====");
		foreach (var item in child_2)
		{
			Debug.Log(item.Position);
		}
	}
	// Clone List<GameObjectInfo>
	List<GameObjectInfo> gameObjectListClone(List<GameObjectInfo> originalGameObjectList)
	{
		var GameObjectListClone = new List<GameObjectInfo>();
		foreach (var item in originalGameObjectList)
		{
			GameObjectListClone.Add(new GameObjectInfo());
			GameObjectListClone[GameObjectListClone.Count - 1].Position = item.Position;
			GameObjectListClone[GameObjectListClone.Count - 1].GameObjectAttribute = item.GameObjectAttribute;
		}
		return GameObjectListClone;
	}
}
