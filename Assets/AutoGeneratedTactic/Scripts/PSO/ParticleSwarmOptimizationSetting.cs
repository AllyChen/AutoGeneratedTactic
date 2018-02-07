using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChromosomeDefinition;
using GeneticAlgorithmSettingDefinition;

namespace ParticleSwarmOptimizationSettingDefinition
{
	public class ParticleSwarmOptimizationSetting : MonoBehaviour
	{

		#region Initial
		// Define the basic parameters.
		private int _mapLength;
		private int _mapWidth;
		private int _numGenes;
		private int _numChromosomes;
		private int _numGenerations;
		private List<Chromosome> _particlePopulation = new List<Chromosome>();// personal position

		// Define the basic parameters of PSO.
		private List<Chromosome> _velocityPopulation = new List<Chromosome>();// personal velocity
		private List<Chromosome> _personBestPopulation = new List<Chromosome>();// personal bests
		private Chromosome _globalBestChromosome = new Chromosome();// global best
		private float _inertia;
		private float _personalInfluence;
		private float _socialInfluence;

		public void InitialPopulation(int length, int width, int numGene, int numChromosome)
		{
			// Clean all the population and chromosome.
			_particlePopulation.Clear();
			_velocityPopulation.Clear();
			_personBestPopulation.Clear();

			// Initial the parameters.
			_inertia = 1.0f;
			_personalInfluence = 1.0f;
			_socialInfluence = 1.0f;

			// Get the data of basic parameters.
			_mapLength = length;
			_mapWidth = width;
			_numGenes = numGene;
			_numChromosomes = numChromosome;

			// Create the chromosomes in population.
			for (int x = 0; x < numChromosome; x++)
			{
				_particlePopulation.Add(new Chromosome());

				// Create the genes in each chromosomes.
				// Random
				for (int y = 0; y < numGene; y++)
				{
					_particlePopulation[x].genesList.Add(new Gene());
					if (Random.Range(0, 2) == 0)
					{
						_particlePopulation[x].genesList[y].type = GeneType.Empty;
					}
					else
					{
						_particlePopulation[x].genesList[y].type = GeneType.Forbidden;
					}		
				}
			}
			Debug.Log("BeforeCreateInitial_velocityPopulation.Count = " + _velocityPopulation.Count);
			// Initial the fitness score of chromosomes
			for (int i = 0; i < _numChromosomes; i++)
			{
				_particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = 0.0f;
			}

			if (_velocityPopulation.Count != 0)
			{
				for (int i = 0; i < _numChromosomes; i++)
				{
					Debug.Log("initial_velocityPopulation[" + i + "].genesList.Count = " + _velocityPopulation[i].genesList.Count);
				}
			}
			else
			{
				Debug.Log("initial_velocityPopulation.Count = 0");
			}
			
			// Randomly initial the personal velocity of chromosomes
			for (int x = 0; x < numChromosome; x++)
			{
				_velocityPopulation.Add(new Chromosome());

				// Create the genes in each chromosomes.
				// Random
				for (int y = 0; y < numGene; y++)
				{
					_velocityPopulation[x].genesList.Add(new Gene());
					if (Random.Range(0, 2) == 0)
					{
						_velocityPopulation[x].genesList[y].type = GeneType.Empty;
					}
					else
					{
						_velocityPopulation[x].genesList[y].type = GeneType.Forbidden;
					}
				}
			}
			Debug.Log("AfterCreateInitial_velocityPopulation.Count = " + _velocityPopulation.Count);
		}
		#endregion

		#region Fitness & Update personal bests and global best
		FitnessFunctions FitnessFunction = new FitnessFunctions();
		Chromosome tempGlobalBestChromosome = new Chromosome();

		public void CalculateFitnessScores()
		{
			// Calculate the fitness score of chromosomes in population
			for (int i = 0; i < _numChromosomes; i++)
			{
				_particlePopulation[i].FitnessScore[FitnessFunctionName.ImpassableDensity] = FitnessFunction.Fitness_ImpassableDensity(_particlePopulation[i], _numGenes, _mapLength, _mapWidth);
			}
			// Calculate the sum of fitness score
			// & Save the personal best of chromosome
			// & Calculate the global best of chromosome.
			for (int i = 0; i < _numChromosomes; i++)
			{
				// Sum the fitness score of new chromosome
				float tempSumFitnessScore = _particlePopulation[i].FitnessScore[FitnessFunctionName.ImpassableDensity];

				// New chromosome is better than previous chromosome.
				if (tempSumFitnessScore >= _particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore])
				{
					_personBestPopulation.Add(_particlePopulation[i]);
				}

				// Update the SumFitnessScore of each chromosome.
				_particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = tempSumFitnessScore;

				// Update the global best chromosome.
				if (i == 0)
				{
					tempGlobalBestChromosome = _particlePopulation[i];
				}
				else
				{
					if (_particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] >= tempGlobalBestChromosome.FitnessScore[FitnessFunctionName.SumOfFitnessScore])
					{
						tempGlobalBestChromosome = _particlePopulation[i];
					}
				}
			}
			// Save the global best of chromosome.
			_globalBestChromosome = tempGlobalBestChromosome;
		}
		#endregion

		#region Update velocities
		/// <summary>
		/// Update velocities
		/// <remarks>
		/// 根據影響係數，計算出要被替換的基因數量，方法類似crossover。
		/// </remarks>
		/// </summary>
		List<Chromosome> tempVelocityPopulation = new List<Chromosome>();// temp personal velocity

		public void UpdateVelocities()
		{
			tempVelocityPopulation.Clear();
			int numInertiaGene = (int)(_inertia * _numGenes);
			int numPersonalInfluenceGene;
			int numSocialInfluenceGene;

			for (int i = 0; i < _numChromosomes; i++)
			{
				Debug.Log("_velocityPopulation[" + i + "].genesList.Count = " + _velocityPopulation[i].genesList.Count);
			}

			for (int i = 0; i < _numChromosomes; i++)
			{
				Debug.Log("inertiaMethod("+i+", "+numInertiaGene+")");
				// Inertia
				tempVelocityPopulation.Add(inertiaMethod(i, numInertiaGene));
				// PersonalInfluence
				numPersonalInfluenceGene = (int)( _personalInfluence * _numGenes * Random.Range(0.0f, 1.0f) );
				if (numPersonalInfluenceGene != 0)
				{
					tempVelocityPopulation[i] = personalInfluenceMethod(i, numPersonalInfluenceGene);
				}
				// SocialInfluence
				numSocialInfluenceGene = (int)( _socialInfluence * _numGenes * Random.Range(0.0f, 1.0f) );
				if (numSocialInfluenceGene != 0)
				{
					tempVelocityPopulation[i] = socialInfluenceMethod(i, numSocialInfluenceGene);
				}
				_velocityPopulation[i] = tempVelocityPopulation[i];
			}
		}

		Chromosome tempVelocity = new Chromosome();

		Chromosome inertiaMethod(int indexChromosome, int _numInertiaGene)
		{
			for (int i = 0; i < _numChromosomes; i++)
			{
				Debug.Log("Before_velocityPopulation[" + i + "].genesList.Count = " + _velocityPopulation[i].genesList.Count);
			}
			int numGene = _velocityPopulation[indexChromosome].genesList.Count;
			int startIndex = Random.Range(0, numGene);
			int endIndex;
			// Calculate the index of gene which need to swap.
			if (( numGene - startIndex ) < _numInertiaGene)
			{
				endIndex = _numInertiaGene - ( numGene - startIndex ) - 1;
			}
			else
			{
				endIndex = startIndex + _numInertiaGene - 1;
			}
			// First, copy the origenial position.
			tempVelocity = _particlePopulation[indexChromosome];
			// Swap!!
			tempVelocity = afterSwap(tempVelocity, _velocityPopulation[indexChromosome], numGene, startIndex, endIndex);

			for (int i = 0; i < _numChromosomes; i++)
			{
				Debug.Log("After_velocityPopulation[" + i + "].genesList.Count = " + _velocityPopulation[i].genesList.Count);
			}

			return tempVelocity;
		}

		Chromosome personalInfluenceMethod(int indexChromosome, int _numPersonalInfluenceGene)
		{
			tempVelocity.genesList.Clear();
			int numGene = _particlePopulation[indexChromosome].genesList.Count;
			int startIndex = Random.Range(0, numGene);
			int endIndex;
			Debug.Log("_particlePopulation[indexChromosome].genesList.Count=" + _particlePopulation[indexChromosome].genesList.Count);
			// Calculate the index of gene which need to swap.
			if (( numGene - startIndex ) < _numPersonalInfluenceGene)
			{
				endIndex = _numPersonalInfluenceGene - ( numGene - startIndex) - 1;
			}
			else
			{
				endIndex = startIndex + _numPersonalInfluenceGene - 1;
			}
			// First, copy the origenial position.
			tempVelocity = tempVelocityPopulation[indexChromosome];
			// Swap!!
			Debug.Log("tempVelocity.genesList.Count=" + tempVelocity.genesList.Count);
			Debug.Log("_particlePopulation["+indexChromosome+"].genesList.Count=" + _particlePopulation[indexChromosome].genesList.Count);
			tempVelocity = afterSwap(tempVelocity, _particlePopulation[indexChromosome], numGene, startIndex, endIndex);
			return tempVelocity;
		}

		Chromosome socialInfluenceMethod(int indexChromosome, int _numSocialInfluenceGene)
		{
			tempVelocity.genesList.Clear();
			int numGene = _personBestPopulation[indexChromosome].genesList.Count;
			int startIndex = Random.Range(0, numGene);
			int endIndex;

			// Calculate the index of gene which need to swap.
			if (( numGene - startIndex ) < _numSocialInfluenceGene)
			{
				endIndex = _numSocialInfluenceGene - ( numGene - startIndex ) - 1;
			}
			else
			{
				endIndex = startIndex + _numSocialInfluenceGene - 1;
			}
			// First, copy the origenial position.
			tempVelocity = tempVelocityPopulation[indexChromosome];
			// Swap!!
			tempVelocity = afterSwap(tempVelocity, _personBestPopulation[indexChromosome], numGene, startIndex, endIndex);
			return tempVelocity;
		}

		Chromosome afterSwap(Chromosome swapChromosomeParent, Chromosome swapChromosomeChild, int numGene, int startIndex, int endIndex)
		{
			// Swap
			// 0 1 2 3 4 5
			// x s o o e x
			if (startIndex < endIndex)
			{
				for (int i = startIndex; i <= endIndex; i++)
				{
					swapChromosomeChild.genesList[i] = swapChromosomeParent.genesList[i];
				}
			}
			// 0 1 2 3 4 5
			// o e x x s o
			else if (startIndex > endIndex)
			{
				for (int i = startIndex; i < numGene; i++)
				{
					swapChromosomeChild.genesList[i] = swapChromosomeParent.genesList[i];
				}
				for (int i = 0; i <= endIndex; i++)
				{
					swapChromosomeChild.genesList[i] = swapChromosomeParent.genesList[i];
				}
			}
			// 0 1 2 3 4 5
			// x x x x s x
			// x x x x e x
			// x x x x o x
			else if (startIndex == endIndex)
			{
				swapChromosomeChild.genesList[startIndex] = swapChromosomeParent.genesList[startIndex];
			}
			return swapChromosomeChild;
		}

		#endregion

		#region Update positions (chromosomes)
		public void UpdatePosition()
		{
			for (int indexChromosome = 0; indexChromosome < _particlePopulation.Count; indexChromosome++)
			{
				_particlePopulation[indexChromosome] = _velocityPopulation[indexChromosome];
			}
		}
		#endregion

		#region BestChromesome
		int index_BestChromesome = 0;

		public Chromosome BestChromesome()//Chromosome bestChromesome
		{
			CalculateFitnessScores();
			return _globalBestChromosome;
		}
		#endregion

		#region DebugTest
		public void DebugTest()
		{
			for (int x = 0; x < _numChromosomes; x++)
			{
				Debug.Log("Chromosome[" + x + "] Fitness =" + _particlePopulation[x].FitnessScore[FitnessFunctionName.SumOfFitnessScore]);
			}
			Debug.Log("index_BestChromesome = " + index_BestChromesome);
		}
		#endregion
	}
}

