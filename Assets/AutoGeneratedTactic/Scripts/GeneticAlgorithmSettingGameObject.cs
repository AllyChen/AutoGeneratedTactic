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

		public void InitialPopulation(int length, int width, int numGene, int numChromosome, int numGeneration, Chromosome spaceChromosome)
		{
			// Clean all the population.
			_population.Clear();

			// Get the data of parameters.
			_mapLength = length;
			_mapWidth = width;
			_numGenes = numGene;
			_numChromosomes = numChromosome;
			_numGenerations = numGeneration;

			// Save the data of the empty tiles around of the room.
			List<int> EmptyTiles = new List<int>();
			int indexEntrance = 0;
			int indexExit = 0;
			// Create the chromosomes in population.
			for (int x = 0; x < numChromosome; x++)
			{
				_population.Add(spaceChromosome.CloneSpace());

				// Randomly set the door in each chromosome
				EmptyTiles.Clear();
				EmptyTiles = FindEmptyTiles(_mapLength, _mapWidth, _population[x]);
				if (EmptyTiles.Count >= 2)
				{
					indexEntrance = Random.Range(0, EmptyTiles.Count);// does not contain mix number
					indexExit = Random.Range(0, EmptyTiles.Count - 1);
					indexExit = ( indexExit >= indexEntrance ) ? indexExit + 1 : indexExit;

					_population[x].genesList[EmptyTiles[indexEntrance]].GameObjectAttribute = GeneGameObjectAttribute.entrance;
					_population[x].genesList[EmptyTiles[indexExit]].GameObjectAttribute = GeneGameObjectAttribute.exit;
				}
				else
				{
					Debug.Log("There are no enough tiles to put the door!!");
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
				if (_population[0].genesList[indexGene].type == GeneType.Empty)
				{
					indexOfEmptyTiles.Add(indexGene);
				}
			}
			// RightColumn
			for (int i = 1; i <= numRightLeft; i++)
			{
				indexGene = ( mapLength - 1 ) + ( i - 1 ) * mapLength;
				if (_population[0].genesList[indexGene].type == GeneType.Empty)
				{
					indexOfEmptyTiles.Add(indexGene);
				}
			}
			// DownRow
			for (int i = 1; i <= numTopDown; i++)
			{
				indexGene = ( mapLength * ( mapWidth - 1 ) + 1 ) + ( i - 1 );
				if (_population[0].genesList[indexGene].type == GeneType.Empty)
				{
					indexOfEmptyTiles.Add(indexGene);
				}
			}
			// LeftColumn
			for (int i = 1; i <= numRightLeft; i++)
			{
				indexGene = mapLength + ( i - 1 ) * mapLength;
				if (_population[0].genesList[indexGene].type == GeneType.Empty)
				{
					indexOfEmptyTiles.Add(indexGene);
				}
			}

			return indexOfEmptyTiles;
		}
		#endregion

		#region Fitness

		#endregion

		#region Selection

		#endregion

		#region Crossover

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


