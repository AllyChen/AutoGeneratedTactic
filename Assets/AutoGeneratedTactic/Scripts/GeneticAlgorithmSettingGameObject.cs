using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;

using ChromosomeDefinition;
using NesScripts.Controls.PathFind;

namespace GeneticAlgorithmSettingGameObjectDefinition
{
	public class GeneticAlgorithmSettingGameObject : MonoBehaviour
	{
		#region Initial
		// Define the basic parameters.
		private int _mapLength;
		private int _mapWidth;
		private int _numGenes;
		private int _numChromosomes;
		private int _numGenerations;
		private List<Chromosome> _population = new List<Chromosome>();
		private List<int> EmptyTilesAround = new List<int>();
		private List<int> EmptyTiles = new List<int>();
		private int[] _numMinGameObject;
		private int[] _numMaxGameObject;
		private Chromosome _spaceChromosome;
		private Grid spaceGrid; // grid of space for using the A* 

		public void InitialPopulation(int length, int width, int numGene, int numChromosome, int numGeneration, Chromosome spaceChromosome, int[] numMinGameObject, int[] numMaxGameObject)
		{
			// Clean all the population.
			_population.Clear();
			EmptyTilesAround.Clear();
			EmptyTiles.Clear();

			// Get the data of parameters.
			_mapLength = length;
			_mapWidth = width;
			_numGenes = numGene;
			_numChromosomes = numChromosome;
			_numGenerations = numGeneration;
			_numMinGameObject = numMinGameObject;
			_numMaxGameObject = numMaxGameObject;
			_spaceChromosome = spaceChromosome;
			
			// Save the data of the empty tiles around of the room.			
			EmptyTilesAround = FindEmptyTiles(_mapLength, _mapWidth, spaceChromosome);
			int leastNumEmptyTilesAround = _numMaxGameObject[(int)GeneGameObjectAttribute.entrance - 1] + _numMaxGameObject[(int)GeneGameObjectAttribute.exit - 1];

			// Calculate the number of empty tiles
			for (int indexGene = 0; indexGene < _numGenes; indexGene++)
			{
				if (spaceChromosome.genesList[indexGene].type == GeneType.Empty)
				{
					EmptyTiles.Add(indexGene);
				}
			}
			// The least number of tiles for game object
			int leastNumEmptyTiles = leastNumEmptyTilesAround + _numMaxGameObject[(int)GeneGameObjectAttribute.enemy - 1] + _numMaxGameObject[(int)GeneGameObjectAttribute.trap - 1] + _numMaxGameObject[(int)GeneGameObjectAttribute.treasure - 1];

			// Create the chromosomes in population.
			if (EmptyTilesAround.Count >= leastNumEmptyTilesAround && EmptyTiles.Count >= leastNumEmptyTiles)
			{
				for (int x = 0; x < numChromosome; x++)
				{
					_population.Add(spaceChromosome.CloneSpace());

					// New the temp list
					List<int> EmptyTilesAroundTemp = new List<int>(EmptyTilesAround.ToArray());
					List<int> EmptyTilesTemp = new List<int>(EmptyTiles.ToArray());
					
					// Randomly set the number of game object
					// Entrance
					EmptyTilesAroundTemp = setGameObjectList(EmptyTilesAroundTemp, _numMinGameObject[(int)GeneGameObjectAttribute.entrance - 1], _numMaxGameObject[(int)GeneGameObjectAttribute.entrance - 1], GeneGameObjectAttribute.entrance, _population[x]);
					// Exit
					EmptyTilesAroundTemp = setGameObjectList(EmptyTilesAroundTemp, _numMinGameObject[(int)GeneGameObjectAttribute.exit - 1], _numMaxGameObject[(int)GeneGameObjectAttribute.exit - 1], GeneGameObjectAttribute.exit, _population[x]);

					// Remove the empty tiles in EmptyTilesTemp, because there are entrance and exit.
					var find = from objectItem in _population[x].gameObjectList
							   where objectItem.GameObjectAttribute == GeneGameObjectAttribute.entrance || objectItem.GameObjectAttribute == GeneGameObjectAttribute.exit
							   select objectItem;

					foreach (var item in find)
					{
						for (int indexEmpty = 0; indexEmpty < EmptyTilesTemp.Count; indexEmpty++)
						{
							if (EmptyTilesTemp[indexEmpty] == item.Position)
							{
								EmptyTilesTemp.RemoveAt(indexEmpty);
								break;
							}
						}
					}
					// Enemy
					EmptyTilesTemp = setGameObjectList(EmptyTilesTemp, _numMinGameObject[(int)GeneGameObjectAttribute.enemy - 1], _numMaxGameObject[(int)GeneGameObjectAttribute.enemy - 1], GeneGameObjectAttribute.enemy, _population[x]);
					// Trap
					EmptyTilesTemp = setGameObjectList(EmptyTilesTemp, _numMinGameObject[(int)GeneGameObjectAttribute.trap - 1], _numMaxGameObject[(int)GeneGameObjectAttribute.trap - 1], GeneGameObjectAttribute.trap, _population[x]);
					// Treasure
					EmptyTilesTemp = setGameObjectList(EmptyTilesTemp, _numMinGameObject[(int)GeneGameObjectAttribute.treasure - 1], _numMaxGameObject[(int)GeneGameObjectAttribute.treasure - 1], GeneGameObjectAttribute.treasure, _population[x]);

					// Setting the game object base on the gameObjectList.
					_population[x].settingGameObject();
				}
			}
			else
			{
				// Only add the space without game object.
				_population.Add(spaceChromosome.CloneSpace());
				if (EmptyTilesAround.Count < leastNumEmptyTilesAround)
				{
					Debug.Log("There are no enough tiles to put the door!!");
				}
				else if (EmptyTiles.Count < leastNumEmptyTiles)
				{
					Debug.Log("There are no enough tiles to put the game object!!");
				}			
			}
			// InitialData
			InitialData();

			// Initial the grid of space
			// create the tiles map
			bool[,] tilesmap = new bool[width, length];
			// set values here....
			// true = walkable, false = blocking
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < length; y++)
				{
					if (spaceChromosome.genesList[x * length + y].type == GeneType.Empty)
					{
						tilesmap[x, y] = true;
					}
					if (spaceChromosome.genesList[x * length + y].type == GeneType.Forbidden)
					{
						tilesmap[x, y] = false;
					}
				}
			}

			// create a grid
			spaceGrid = new Grid(tilesmap);
		}

		List<int> FindEmptyTiles(int mapLength, int mapWidth, Chromosome chromoseome)
		{
			List<int> indexOfEmptyTiles = new List<int>();
			int numTopDown = mapLength - 1;
			int numRightLeft = mapWidth - 1;
			int indexGene = 0;
			// TopRow
			for (int i = 1; i <= numTopDown; i++)
			{
				indexGene = 0 + ( i - 1 );
				if (chromoseome.genesList[indexGene].type == GeneType.Empty)
				{
					indexOfEmptyTiles.Add(indexGene);
				}
			}
			// RightColumn
			for (int i = 1; i <= numRightLeft; i++)
			{
				indexGene = ( mapLength - 1 ) + ( i - 1 ) * mapLength;
				if (chromoseome.genesList[indexGene].type == GeneType.Empty)
				{
					indexOfEmptyTiles.Add(indexGene);
				}
			}
			// DownRow
			for (int i = 1; i <= numTopDown; i++)
			{
				indexGene = ( mapLength * ( mapWidth - 1 ) + 1 ) + ( i - 1 );
				if (chromoseome.genesList[indexGene].type == GeneType.Empty)
				{
					indexOfEmptyTiles.Add(indexGene);
				}
			}
			// LeftColumn
			for (int i = 1; i <= numRightLeft; i++)
			{
				indexGene = mapLength + ( i - 1 ) * mapLength;
				if (chromoseome.genesList[indexGene].type == GeneType.Empty)
				{
					indexOfEmptyTiles.Add(indexGene);
				}
			}

			return indexOfEmptyTiles;
		}

		List<int> setGameObjectList(List<int> TilesList, int numMinGameObject, int numMaxGameObject, GeneGameObjectAttribute Attribute, Chromosome chromosome)
		{
			// Randomly set the number of game object
			int numGameObject = Random.Range(numMinGameObject, numMaxGameObject + 1);
			int indexTiles = 0;// The index of empty tile to put the game object.
			int position = 0;// The position that we take from the tile list.

			// Randomly set the game object in chromosome
			for (int time = 0; time < numGameObject; time++)
			{
				indexTiles = Random.Range(0, TilesList.Count);
				position = TilesList[indexTiles];
				chromosome.AddGameObjectInList(position, Attribute);
				TilesList.RemoveAt(indexTiles);
			}
			return TilesList;
		}

		#endregion

		#region Fitness
		FitnessFunctions FitnessFunction = new FitnessFunctions();
		float weight_MainPathQuality = 1.0f;
		float weight_Fitness_Defense;
		float weight_Fitness_OnMainPath;
		float weight_Fitness_BesideMainPath;
		bool isTreasureOnMainPath;
		bool isTreasureBesideMainPath;

		public void DetermineWeightFitness(bool isFitness_Defense, bool isFitness_OnMainPath, bool isFitness_BesideMainPath, 
											float weightFitness_Defense, float weightFitness_OnMainPath, float weightFitness_BesideMainPath,
											bool _isTreasureOnMainPath, bool _isTreasureBesideMainPath)
		{
			if (isFitness_Defense == true)
			{
				weight_Fitness_Defense = weightFitness_Defense;
			}
			else
			{
				weight_Fitness_Defense = 0.0f;
			}
			if (isFitness_OnMainPath == true)
			{
				weight_Fitness_OnMainPath = weightFitness_OnMainPath;
			}
			else
			{
				weight_Fitness_OnMainPath = 0.0f;
			}
			if (isFitness_BesideMainPath == true)
			{
				weight_Fitness_BesideMainPath = weightFitness_BesideMainPath;
			}
			else
			{
				weight_Fitness_BesideMainPath = 0.0f;
			}

			isTreasureOnMainPath = _isTreasureOnMainPath;
			isTreasureBesideMainPath = _isTreasureBesideMainPath;
		}

		public void CalculateFitnessScores()
		{
			// Calculate the fitness score of chromosomes in population.
			for (int i = 0; i < _numChromosomes; i++)
			{
				// Fitness function
				_population[i].FitnessScore[FitnessFunctionName.MainPathQuality] = FitnessFunction.Fitness_MainPathQuality(_population[i], _mapLength, _mapWidth, EmptyTiles.Count, spaceGrid);
				_population[i].FitnessScore[FitnessFunctionName.Fitness_Defense] = FitnessFunction.Fitness_Defense(_population[i], _mapLength, _mapWidth);
				if (isTreasureOnMainPath == true)
				{
					_population[i].FitnessScore[FitnessFunctionName.Fitness_OnMainPath] = FitnessFunction.Fitness_OnMainPath(_population[i], _mapLength, _mapWidth, GeneGameObjectAttribute.treasure);
				}
				else
				{
					_population[i].FitnessScore[FitnessFunctionName.Fitness_OnMainPath] = ( FitnessFunction.Fitness_OnMainPath(_population[i], _mapLength, _mapWidth, GeneGameObjectAttribute.enemy)
																						+ FitnessFunction.Fitness_OnMainPath(_population[i], _mapLength, _mapWidth, GeneGameObjectAttribute.trap) * 0.5f ) / 2.0f;
				}
				if (isTreasureBesideMainPath == true)
				{
					_population[i].FitnessScore[FitnessFunctionName.Fitness_BesideMainPath] = FitnessFunction.Fitness_BesideMainPath(_population[i], _mapLength, _mapWidth, GeneGameObjectAttribute.treasure);
				}
				else
				{
					_population[i].FitnessScore[FitnessFunctionName.Fitness_BesideMainPath] = ( FitnessFunction.Fitness_BesideMainPath(_population[i], _mapLength, _mapWidth, GeneGameObjectAttribute.enemy)
																							+ FitnessFunction.Fitness_BesideMainPath(_population[i], _mapLength, _mapWidth, GeneGameObjectAttribute.trap) * 0.5f ) / 2.0f;
				}
				_population[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = ( _population[i].FitnessScore[FitnessFunctionName.MainPathQuality] * weight_MainPathQuality
																					+ _population[i].FitnessScore[FitnessFunctionName.Fitness_Defense] * weight_Fitness_Defense
																					+ _population[i].FitnessScore[FitnessFunctionName.Fitness_OnMainPath] * weight_Fitness_OnMainPath
																					+ _population[i].FitnessScore[FitnessFunctionName.Fitness_BesideMainPath] * weight_Fitness_BesideMainPath ) / 4.0f;
			}
		}
		// Calculate the fitness score of chromosomes in specific population.
		void CalculatePopulationFitnessScores(List<Chromosome> _specificPopulation)
		{
			// Calculate the fitness score of chromosomes in population.
			for (int i = 0; i < _specificPopulation.Count; i++)
			{
				// Fitness function
				_specificPopulation[i].FitnessScore[FitnessFunctionName.MainPathQuality] = FitnessFunction.Fitness_MainPathQuality(_specificPopulation[i], _mapLength, _mapWidth, EmptyTiles.Count, spaceGrid);
				_specificPopulation[i].FitnessScore[FitnessFunctionName.Fitness_Defense] = FitnessFunction.Fitness_Defense(_specificPopulation[i], _mapLength, _mapWidth);
				if (isTreasureOnMainPath == true)
				{
					_specificPopulation[i].FitnessScore[FitnessFunctionName.Fitness_OnMainPath] = FitnessFunction.Fitness_OnMainPath(_specificPopulation[i], _mapLength, _mapWidth, GeneGameObjectAttribute.treasure);
				}
				else
				{
					_specificPopulation[i].FitnessScore[FitnessFunctionName.Fitness_OnMainPath] = ( FitnessFunction.Fitness_OnMainPath(_specificPopulation[i], _mapLength, _mapWidth, GeneGameObjectAttribute.enemy)
																								+ FitnessFunction.Fitness_OnMainPath(_specificPopulation[i], _mapLength, _mapWidth, GeneGameObjectAttribute.trap) * 0.5f ) / 2.0f;
				}
				if (isTreasureBesideMainPath == true)
				{
					_specificPopulation[i].FitnessScore[FitnessFunctionName.Fitness_BesideMainPath] = FitnessFunction.Fitness_BesideMainPath(_specificPopulation[i], _mapLength, _mapWidth, GeneGameObjectAttribute.treasure);
				}
				else
				{
					_specificPopulation[i].FitnessScore[FitnessFunctionName.Fitness_BesideMainPath] = ( FitnessFunction.Fitness_BesideMainPath(_specificPopulation[i], _mapLength, _mapWidth, GeneGameObjectAttribute.enemy)
																									+ FitnessFunction.Fitness_BesideMainPath(_specificPopulation[i], _mapLength, _mapWidth, GeneGameObjectAttribute.trap) * 0.5f ) / 2.0f;
				}
				_specificPopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = ( _specificPopulation[i].FitnessScore[FitnessFunctionName.MainPathQuality] * weight_MainPathQuality
																							+ _specificPopulation[i].FitnessScore[FitnessFunctionName.Fitness_Defense] * weight_Fitness_Defense
																							+ _specificPopulation[i].FitnessScore[FitnessFunctionName.Fitness_OnMainPath] * weight_Fitness_OnMainPath
																							+ _specificPopulation[i].FitnessScore[FitnessFunctionName.Fitness_BesideMainPath] * weight_Fitness_BesideMainPath ) / 4.0f;
			}
		}
		#endregion

		#region Selection
		// Select the chromosomes which be crossovered.
		List<Chromosome> _crossoverPoll = new List<Chromosome>();
		List<float> rouletteWheel = new List<float>();
		float sum_fitnessScore; // The sum of the fitness score of all chromosomes.
		float probabilityChoose; // The number of chromosomes which be put in the crossover poll.

		public void Selection()
		{
			rouletteWheel.Clear();
			_crossoverPoll.Clear();
			sum_fitnessScore = 0;
			probabilityChoose = 0;

			// Calculate the sum of the fitness score of all chromosomes.
			for (int i = 0; i < _numChromosomes; i++)
			{
				sum_fitnessScore += _population[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore];
			}
			// Calculate the probability of chromosomes which be choosed.
			for (int i = 0; i < _numChromosomes; i++)
			{
				if (sum_fitnessScore == 0)
				{
					probabilityChoose += 1.0f / _numChromosomes;
					rouletteWheel.Add(probabilityChoose);
				}
				else
				{
					probabilityChoose += _population[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] / sum_fitnessScore;
					rouletteWheel.Add(probabilityChoose);
				}
			}

			// Set the crossover pull.
			for (int i = 0; i < _numChromosomes; i++)
			{
				int index_Chromosome = 0;
				float randomChooseChromosomes = Random.Range(0.0f, 1.0f);

				// 尋找隨機選到的Chromosome
				while (randomChooseChromosomes > rouletteWheel[index_Chromosome])
				{
					index_Chromosome++;
				}
				_crossoverPoll.Add(_population[index_Chromosome]);
			}
		}
		#endregion

		#region Crossover
		/// <summary>
		/// Step.1 隨機取得父母染色體。在indexChromosomesArray裡儲存染色體的Index，並將此陣列隨機洗牌。
		/// Step.2 利用陣列內的隨機Index值，依序從 _crossoverPoll 裡拿染色體進行一定機率的交配。
		/// Step.3 交配後的子代存入 _childsPopulation。
		/// Step.4 若染色體沒進行交配則原封不動放入。
		/// </summary>
		int[] indexChromosomesArray;
		List<List<GameObjectInfo>> _childsGameObjectListPopulation = new List<List<GameObjectInfo>>();
		// Step.1
		void RandomChromosomesIndex()
		{
			// Initial the array.
			indexChromosomesArray = new int[_numChromosomes];
			for (int index = 0; index < _numChromosomes; index++)
			{
				indexChromosomesArray[index] = index;
			}

			// Random the index array.
			for (int index = _numChromosomes - 1; index > 0; index--)
			{
				int randomArrayIndex = Random.Range(0, index);
				int tempValue = indexChromosomesArray[randomArrayIndex];
				// Swap the value with the last one.
				indexChromosomesArray[randomArrayIndex] = indexChromosomesArray[index];
				indexChromosomesArray[index] = tempValue;
			}
		}

		public void Crossover(float rateCrossover)
		{
			_childsGameObjectListPopulation.Clear();
			RandomChromosomesIndex();
			// Get the parents
			List<GameObjectInfo> parent_A;
			List<GameObjectInfo> parent_B;
			int lengthArray = indexChromosomesArray.Length;
			int index_parent_A;
			int index_parent_B;

			for (int i = 0; i < lengthArray; i++)
			{
				index_parent_A = indexChromosomesArray[i % lengthArray];
				index_parent_B = indexChromosomesArray[i % lengthArray];
				// Clone the chromosomes which need to crossover.
				parent_A = gameObjectListClone(_crossoverPoll[index_parent_A].gameObjectList);
				parent_B = gameObjectListClone(_crossoverPoll[index_parent_B].gameObjectList);

				if (rateCrossover > Random.Range(0.0f, 1.0f))
				{
					// Step.3
					CrossoverMethod(_crossoverPoll[index_parent_A].gameObjectList, _crossoverPoll[index_parent_B].gameObjectList);
				}
				else
				{
					// Step.4
					_childsGameObjectListPopulation.Add(parent_A);
					_childsGameObjectListPopulation.Add(parent_B);
				}
			}
		}

		void CrossoverMethod(List<GameObjectInfo> parent_1, List<GameObjectInfo> parent_2)
		{
			List<List<GameObjectInfo>> child_1_GameObjectPositionList = SearchGameObject(parent_1);
			List<List<GameObjectInfo>> child_2_GameObjectPositionList = SearchGameObject(parent_2);

			// Save the position of Game Object
			List<List<GameObjectInfo>> Parent_1_GameObjectPositionList = SearchGameObject(parent_1);
			List<List<GameObjectInfo>> Parent_2_GameObjectPositionList = SearchGameObject(parent_2);

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
		// Transform List<List<GameObjectInfo>> TO List<GameObjectInfo> for population
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
		#endregion

		#region Mutation
		/// <summary>
		/// <method>
		/// Step.1 決定要更動的GameObjectType順序
		/// Step.2-1 隨機增加或減少GameObject。
		/// Step.2-2 隨機改變 GameObject 位置。
		/// Step.3 進行變異。
		/// </method>
		/// </summary>
		int[] mutateGameObjectTypeArray;
		// Step.1
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

		void MutationMethod_AddDelete(List<GameObjectInfo> originalGameObjectList)
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
				// Step.2
				#region Randomly Add or delete the same game object.
				// Randomly determine Add or delete the same game object.
				int randomDetermine = Random.Range(0, 2);
				// Add the same game object
				if (randomDetermine == 0)
				{
					// Add
					if (count_currentGameObject < _numMaxGameObject[mutateGameObjectTypeArray[index] - 1])
					{
						AddDeleteGameObject(originalGameObjectList, EmptyTiles, true, count_currentGameObject, startIndexGameObject, (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);
						break;
					}
					else
					{
						// Turn to delete 
						if (count_currentGameObject > _numMinGameObject[mutateGameObjectTypeArray[index] - 1])
						{
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
						AddDeleteGameObject(originalGameObjectList, EmptyTiles, false, count_currentGameObject, startIndexGameObject, (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);
						break;
					}
					else
					{
						// Turn to add
						if (count_currentGameObject < _numMaxGameObject[mutateGameObjectTypeArray[index] - 1])
						{
							AddDeleteGameObject(originalGameObjectList, EmptyTiles, true, count_currentGameObject, startIndexGameObject, (GeneGameObjectAttribute)mutateGameObjectTypeArray[index]);
							break;
						}
					}
				}
				#endregion
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
				originalGameObjectList.Insert(( startIndexGameObject + num_originalGameObject ), newGameObject);
			}
			else
			{
				// Find the position which will be delete the game object.
				int index_deletePosition = Random.Range(0, num_originalGameObject);
				originalGameObjectList.RemoveAt(startIndexGameObject + index_deletePosition);
			}
		}

		void MutationMethod_ChangePosition(List<GameObjectInfo> originalGameObjectList, List<int> EmptyTiles, List<int> EmptyTilesAround)
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
				// If the tactic have this game object
				if (count_currentGameObject != 0)
				{
					startIndexGameObject = startIndexGameObject - count_currentGameObject + 1;
					// Find the position which can be add this game object 
					int newPosition = 0;
					bool isFindPosition = false;
					while (isFindPosition != true)
					{
						if ((GeneGameObjectAttribute)mutateGameObjectTypeArray[index] == GeneGameObjectAttribute.entrance ||
							(GeneGameObjectAttribute)mutateGameObjectTypeArray[index] == GeneGameObjectAttribute.exit)
						{
							newPosition = EmptyTilesAround[Random.Range(0, EmptyTilesAround.Count)];
						}
						else
						{
							newPosition = EmptyTiles[Random.Range(0, EmptyTiles.Count)];
						}

						
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
					// Change the position
					originalGameObjectList[startIndexGameObject + Random.Range(0, count_currentGameObject)].Position = newPosition;
					break;
				}
			}

		}

		public void Mutation(float rateMutation)
		{
			for (int index = 0; index < _childsGameObjectListPopulation.Count; index++)
			{
				if (rateMutation > Random.Range(0.0f, 1.0f))
				{
					if (0.7f > Random.Range(0.0f, 1.0f))
					{
						// Step.3
						MutationMethod_AddDelete(_childsGameObjectListPopulation[index]);
					}
					else
					{
						// Step.3
						MutationMethod_ChangePosition(_childsGameObjectListPopulation[index], EmptyTiles, EmptyTilesAround);
					}						
				}
			}
		}
		#endregion

		#region Replace
		List<Chromosome> _parentsChildsPopulation = new List<Chromosome>();

		public void Replace()
		{
			_parentsChildsPopulation.Clear();

			// Copy the chromosomes from childsPopulation to this population.
			for (int index = 0; index < _childsGameObjectListPopulation.Count; index++)
			{
				_parentsChildsPopulation.Add(_spaceChromosome.CloneSpace());
				_parentsChildsPopulation[_parentsChildsPopulation.Count - 1].settingGameObject(_childsGameObjectListPopulation[index]);
			}
			// Only calculate Childs, because only Childs are in this population.
			CalculatePopulationFitnessScores(_parentsChildsPopulation);

			// Copy the chromosomes from parentsPopulation to this population.
			for (int index = 0; index < _population.Count; index++)
			{
				_parentsChildsPopulation.Add(_population[index].CloneSpaceGameObject());
				_parentsChildsPopulation[_parentsChildsPopulation.Count - 1].copyFitnessScore(_population[index]);
			}

			// Sort the chromosomes
			var chromosomesOrder = from allChromosome in _parentsChildsPopulation
								   orderby allChromosome.FitnessScore[FitnessFunctionName.SumOfFitnessScore]
								   select allChromosome;

			// Clean the population to store the new best chromosome.
			_population.Clear();

			// Select best chromosomes
			int count = 0;
			foreach (var orderChromosome in chromosomesOrder)
			{
				if (count >= ( _parentsChildsPopulation.Count - _numChromosomes ))
				{
					// Copy the chromosome
					_population.Add(orderChromosome.CloneSpaceGameObject());
					_population[_population.Count - 1].copyFitnessScore(orderChromosome);
				}
				count++;
			}
		}
		#endregion

		#region BestChromosome
		int index_BestChromosome = 0;

		public Chromosome BestChromosome()
		{
			// Search the best chromosome in population.
			for (int i = 0; i < _numChromosomes; i++)
			{
				if (_population[index_BestChromosome].FitnessScore[FitnessFunctionName.SumOfFitnessScore] < _population[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore])
				{
					index_BestChromosome = i;
				}
			}
			return _population[index_BestChromosome];
		}
		#endregion

		#region OutputData
		private List<string[]> basicData = new List<string[]>();
		string[] tileData = new string[7];

		bool isSaveWeight;
		void InitialData()
		{
			basicData.Clear();
			isSaveWeight = false;

			// Create the title of data
			tileData[0] = "Generation";
			tileData[1] = "ChromosomeIndex";
			tileData[2] = "Fitness_MainPathQuality";
			tileData[3] = "Fitness_Defense";
			tileData[4] = "Fitness_OnMainPath";
			tileData[5] = "Fitness_BesideMainPath";
			tileData[6] = "Fitness_SumOfFitnessScore";
			basicData.Add(tileData);
		}

		public void SaveData(int indexGeneration)
		{
			if (isSaveWeight == false)
			{
				string[] weightData = new string[tileData.Length];
				weightData[0] = "Weight"; // Generation
				weightData[1] = "-->"; // ChromosomeIndex
				weightData[2] = weight_MainPathQuality.ToString(); // weight_MainPathQuality
				weightData[3] = weight_Fitness_Defense.ToString(); // weight_Fitness_Defense
				weightData[4] = weight_Fitness_OnMainPath.ToString(); // weight_Fitness_OnMainPath
				weightData[5] = weight_Fitness_BesideMainPath.ToString(); // weight_Fitness_BesideMainPath
				weightData[6] = ""; // Fitness_SumOfFitnessScore
				basicData.Add(weightData);

				string[] gameObjectData = new string[tileData.Length];
				gameObjectData[0] = "MAX[" + _numMinGameObject[0].ToString() + "/ " + _numMinGameObject[1].ToString() + "/ " + _numMinGameObject[2].ToString() + "/ " + _numMinGameObject[3].ToString() + "/ " + _numMinGameObject[4].ToString() + "]"; // numberGameObject
				gameObjectData[1] = "min[" + _numMaxGameObject[0].ToString() + "/ " + _numMaxGameObject[1].ToString() + "/ " + _numMaxGameObject[2].ToString() + "/ " + _numMaxGameObject[3].ToString() + "/ " + _numMaxGameObject[4].ToString() + "]"; // numberGameObject
				gameObjectData[2] = ""; // weight_MainPathQuality
				gameObjectData[3] = ""; // weight_Fitness_Defense
				if (isTreasureOnMainPath == true)
				{
					gameObjectData[4] = "Treasure"; // weight_Fitness_OnMainPath
				}
				else
				{
					gameObjectData[4] = "Enemy/Trap"; // weight_Fitness_OnMainPath
				}
				if (isTreasureBesideMainPath == true)
				{
					gameObjectData[5] = "Treasure"; // weight_Fitness_BesideMainPath
				}
				else
				{
					gameObjectData[5] = "Enemy/Trap"; // weight_Fitness_BesideMainPath
				}
				gameObjectData[6] = ""; // Fitness_SumOfFitnessScore
				basicData.Add(gameObjectData);

				isSaveWeight = true;
			}

			for (int indexChromosome = 0; indexChromosome < _numChromosomes; indexChromosome++)
			{
				string[] contentData = new string[tileData.Length];
				contentData[0] = indexGeneration.ToString(); // Generation
				contentData[1] = indexChromosome.ToString(); // ChromosomeIndex
				contentData[2] = _population[indexChromosome].FitnessScore[FitnessFunctionName.MainPathQuality].ToString(); // Fitness_MainPathQuality
				contentData[3] = _population[indexChromosome].FitnessScore[FitnessFunctionName.Fitness_Defense].ToString(); // Fitness_Defense
				contentData[4] = _population[indexChromosome].FitnessScore[FitnessFunctionName.Fitness_OnMainPath].ToString(); // Fitness_OnMainPath
				contentData[5] = _population[indexChromosome].FitnessScore[FitnessFunctionName.Fitness_BesideMainPath].ToString(); // Fitness_BesideMainPath
				contentData[6] = _population[indexChromosome].FitnessScore[FitnessFunctionName.SumOfFitnessScore].ToString(); // Fitness_SumOfFitnessScore
				basicData.Add(contentData);
			}
		}

		public void OutputData(int runGenerate)
		{
			// Copy to output data.
			string[][] outputData = new string[basicData.Count][];
			for (int i = 0; i < outputData.Length; i++)
			{
				outputData[i] = basicData[i];
			}

			// Output
			string filePath = Application.dataPath + "/AutoGeneratedTactic/Data/Experiment/" + "GeneticAlgorithm_Data_GameObject" + _mapLength + "x" + _mapWidth + "_" + runGenerate + ".csv";
			int length = outputData.GetLength(0);// 得到第0維的長度，也就是有幾列。（第1維為行）
			StringBuilder sb = new StringBuilder();
			for (int index = 0; index < length; index++)
			{
				sb.AppendLine(string.Join(",", outputData[index]));
			}
			StreamWriter outStream = System.IO.File.CreateText(filePath);
			outStream.WriteLine(sb);
			outStream.Close();
		}
		#endregion

		#region DebugTest
		public void DebugTest()
		{
		}
		#endregion

	}

}


