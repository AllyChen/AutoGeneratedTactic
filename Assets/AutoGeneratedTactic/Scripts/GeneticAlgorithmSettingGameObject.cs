using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using ChromosomeDefinition;

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

		public void InitialPopulation(int length, int width, int numGene, int numChromosome, int numGeneration, Chromosome spaceChromosome)
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
			_numMinGameObject = new int[5] { 1, 1, 1, 1, 1 };
			_numMaxGameObject = new int[5] { 1, 1, 3, 2 ,2 };
			
			// Save the data of the empty tiles around of the room.			
			EmptyTilesAround = FindEmptyTiles(_mapLength, _mapWidth, spaceChromosome);
			int leastNumEmptyTilesAround = _numMinGameObject[(int)GeneGameObjectAttribute.entrance - 1] + _numMinGameObject[(int)GeneGameObjectAttribute.exit - 1];

			// Calculate the number of empty tiles
			for (int indexGene = 0; indexGene < _numGenes; indexGene++)
			{
				if (spaceChromosome.genesList[indexGene].type == GeneType.Empty)
				{
					EmptyTiles.Add(indexGene);
				}
			}
			// The least number of tiles for game object
			int leastNumEmptyTiles = leastNumEmptyTilesAround + _numMinGameObject[(int)GeneGameObjectAttribute.enemy - 1] + _numMinGameObject[(int)GeneGameObjectAttribute.trap - 1] + _numMinGameObject[(int)GeneGameObjectAttribute.treasure - 1];

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

		public void CalculateFitnessScores()
		{
			// Calculate the fitness score of chromosomes in population.
			for (int i = 0; i < _numChromosomes; i++)
			{
				// Fitness function
				_population[i].FitnessScore[FitnessFunctionName.MainPathQuality] = FitnessFunction.Fitness_MainPathQuality(_population[i], _mapLength, _mapWidth, EmptyTiles.Count);
				_population[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = _population[i].FitnessScore[FitnessFunctionName.MainPathQuality] * weight_MainPathQuality;
			}
		}
		// Calculate the fitness score of chromosomes in specific population.
		void CalculatePopulationFitnessScores(List<Chromosome> _specificPopulation)
		{
			// Calculate the fitness score of chromosomes in population.
			for (int i = 0; i < _numChromosomes; i++)
			{
				//inital
				foreach (Gene gene in _specificPopulation[i].genesList)
				{
					gene.SpaceAttribute = GeneSpaceAttribute.None;
				}
				// Fitness function
				_specificPopulation[i].FitnessScore[FitnessFunctionName.ImpassableDensity] = FitnessFunction.Fitness_MainPathQuality(_specificPopulation[i], _mapLength, _mapWidth, EmptyTiles.Count);
				_specificPopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = _specificPopulation[i].FitnessScore[FitnessFunctionName.MainPathQuality] * weight_MainPathQuality;
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

		#endregion

		#region Replace

		#endregion

		#region BestChromesome
		int index_BestChromesome = 0;

		public Chromosome BestChromesome()
		{
			return _population[index_BestChromesome];
		}
		#endregion

		#region OutputData

		#endregion

		#region DebugTest
		public void DebugTest()
		{
			int index = 0;
			foreach (var Gene in _population[0].genesList)
			{
				Debug.Log("index_" + index + "_" + Gene.GameObjectAttribute);
				index++;
			}
		}
		#endregion

	}

}


