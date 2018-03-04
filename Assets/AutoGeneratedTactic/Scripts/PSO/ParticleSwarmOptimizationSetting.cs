using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

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
		private Chromosome _globalBestChromosome;// = new Chromosome();// global best
		private float _inertia;
		private float _personalInfluence;
		private float _socialInfluence;

		public void InitialPopulation(int length, int width, int numGene, int numChromosome, int numGeneration)
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
			_numGenerations = numGeneration;

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
					
			// Initial the fitness score of chromosomes
			for (int i = 0; i < _numChromosomes; i++)
			{
				_particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = 0.0f;
			}

			// Initial the person best of chromosomes in personBestPopulation
			for (int x = 0; x < numChromosome; x++)
			{
				// Copy the chromosomes from particlePopulation.
				_personBestPopulation.Add(_particlePopulation[x].Clone());
				//_personBestPopulation.Add(new Chromosome());

				//// Copy the genes in each chromosomes from particlePopulation.
				//for (int y = 0; y < numGene; y++)
				//{
				//	_personBestPopulation[x].genesList.Add(new Gene());
				//	_personBestPopulation[x].genesList[y] = _particlePopulation[x].genesList[y];
				//}
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
			// InitialData
			InitialData();
		}
		#endregion

		#region Fitness & Update personal bests and global best
		FitnessFunctions FitnessFunction = new FitnessFunctions();
		int tempGlobalBestChromosome_index = 0;
		float tempGlobalBestChromosome_FitnessSumScore = 0;

		public void CalculateFitnessScores()
		{
			// Calculate the fitness score of chromosomes in population
			// & Calculate the sum of fitness score
			// & Save the personal best of chromosome
			// & Calculate the global best of chromosome.
			for (int i = 0; i < _numChromosomes; i++)
			{
				#region Calculate the fitness score
				// Calculate the fitness score of chromosomes in population
				_particlePopulation[i].FitnessScore[FitnessFunctionName.ImpassableDensity] = FitnessFunction.Fitness_ImpassableDensity(_particlePopulation[i], _numGenes, _mapLength, _mapWidth);
				#endregion

				// Sum the fitness score of new chromosome
				_particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = _particlePopulation[i].FitnessScore[FitnessFunctionName.ImpassableDensity];//A+B+C+D

				// New chromosome is better than previous chromosome.
				if (_particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] >= _personBestPopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore])
				{
					// Copy Genes
					for (int indexGene = 0; indexGene < _personBestPopulation[i].genesList.Count; indexGene++)
					{
						_personBestPopulation[i].genesList[indexGene] = _particlePopulation[i].genesList[indexGene];
					}
					//Copy FitnssScore
					_personBestPopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = _particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore];
				}

				// Update the global best chromosome.
				if (i == 0)
				{
					tempGlobalBestChromosome_index = 0;
					tempGlobalBestChromosome_FitnessSumScore = _particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore];
				}
				else
				{
					if (_particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] >= tempGlobalBestChromosome_FitnessSumScore)
					{
						tempGlobalBestChromosome_index = i;
						tempGlobalBestChromosome_FitnessSumScore = _particlePopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore];
					}
				}
			}
			// Save the global best of chromosome.
			//_globalBestChromosome.genesList.Clear();
			_globalBestChromosome = _particlePopulation[tempGlobalBestChromosome_index].Clone();
			////Copy Genes
			//for (int indexGene = 0; indexGene < _particlePopulation[tempGlobalBestChromosome_index].genesList.Count; indexGene++)
			//{
			//	_globalBestChromosome.genesList.Add(_particlePopulation[tempGlobalBestChromosome_index].genesList[indexGene]);
			//}
			//Copy FitnssScore
			_globalBestChromosome.FitnessScore[FitnessFunctionName.SumOfFitnessScore] = tempGlobalBestChromosome_FitnessSumScore;
		}
		#endregion

		#region Update velocities
		/// <summary>
		/// Update velocities
		/// <remarks>
		/// 根據影響係數，計算出要被替換的基因數量，方法類似crossover。
		/// </remarks>
		/// </summary>
		List<Gene> tempVelocityGeneList = new List<Gene>();// temp personal velocity
		List<Gene> swapGeneListParent = new List<Gene>();
		List<Gene> swapGeneListChild = new List<Gene>();
		List<Gene> afterSwapGeneList = new List<Gene>();

		public void UpdateVelocities()
		{
			tempVelocityGeneList.Clear();

			int numInertiaGene = (int)(_inertia * _numGenes);
			int numPersonalInfluenceGene;
			int numSocialInfluenceGene;

			for (int i = 0; i < _numChromosomes; i++)
			{
				// Inertia
				inertiaMethod(i, numInertiaGene);

				// PersonalInfluence
				numPersonalInfluenceGene = (int)( _personalInfluence * _numGenes * Random.Range(0.0f, 1.0f) );
				if (numPersonalInfluenceGene != 0)
				{
					personalInfluenceMethod(i, numPersonalInfluenceGene);
				}
				// SocialInfluence
				numSocialInfluenceGene = (int)( _socialInfluence * _numGenes * Random.Range(0.0f, 1.0f) );
				if (numSocialInfluenceGene != 0)
				{
					socialInfluenceMethod(i, numSocialInfluenceGene);
				}
				// Get the gene list of Velocity
				for (int indexGenes = 0; indexGenes < _numGenes; indexGenes++)
				{
					_velocityPopulation[i].genesList[indexGenes] = tempVelocityGeneList[indexGenes];
				}		
			}
		}

		void inertiaMethod(int indexChromosome, int _numInertiaGene)
		{
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
			// First, copy the origenial position's GeneList.
			for (int indexGenes = 0; indexGenes < numGene; indexGenes++)
			{
				tempVelocityGeneList.Add(_particlePopulation[indexChromosome].genesList[indexGenes]);
			}
			// Create swapGeneListParent
			swapGeneListParent.Clear();
			for (int indexGenes = 0; indexGenes < numGene; indexGenes++)
			{
				swapGeneListParent.Add(tempVelocityGeneList[indexGenes]);
			}
			// Create swapGeneListChild
			swapGeneListChild.Clear();
			for (int indexGenes = 0; indexGenes < numGene; indexGenes++)
			{
				swapGeneListChild.Add(_velocityPopulation[indexChromosome].genesList[indexGenes]);
			}
			// Swap!!
			afterSwap(swapGeneListParent, swapGeneListChild, numGene, startIndex, endIndex);

			// Finished -- Copy Genes
			for (int indexGene = 0; indexGene < swapGeneListChild.Count; indexGene++)
			{
				tempVelocityGeneList[indexGene] = afterSwapGeneList[indexGene];
			}
		}

		void personalInfluenceMethod(int indexChromosome, int _numPersonalInfluenceGene)
		{
			int numGene = _particlePopulation[indexChromosome].genesList.Count;
			int startIndex = Random.Range(0, numGene);
			int endIndex;

			// Calculate the index of gene which need to swap.
			if (( numGene - startIndex ) < _numPersonalInfluenceGene)
			{
				endIndex = _numPersonalInfluenceGene - ( numGene - startIndex) - 1;
			}
			else
			{
				endIndex = startIndex + _numPersonalInfluenceGene - 1;
			}
			// Create swapGeneListParent
			swapGeneListParent.Clear();
			for (int indexGenes = 0; indexGenes < numGene; indexGenes++)
			{
				swapGeneListParent.Add(tempVelocityGeneList[indexGenes]);
			}
			// Create swapGeneListChild
			swapGeneListChild.Clear();
			for (int indexGenes = 0; indexGenes < numGene; indexGenes++)
			{
				swapGeneListChild.Add(_particlePopulation[indexChromosome].genesList[indexGenes]);
			}
			// Swap!!
			afterSwap(swapGeneListParent, swapGeneListChild, numGene, startIndex, endIndex);

			// Finished -- Copy Genes
			for (int indexGene = 0; indexGene < swapGeneListChild.Count; indexGene++)
			{
				tempVelocityGeneList[indexGene] = afterSwapGeneList[indexGene];
			}
		}

		void socialInfluenceMethod(int indexChromosome, int _numSocialInfluenceGene)
		{
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
			// Create swapGeneListParent
			swapGeneListParent.Clear();
			for (int indexGenes = 0; indexGenes < numGene; indexGenes++)
			{
				swapGeneListParent.Add(tempVelocityGeneList[indexGenes]);
			}
			// Create swapGeneListChild
			swapGeneListChild.Clear();
			for (int indexGenes = 0; indexGenes < numGene; indexGenes++)
			{
				swapGeneListChild.Add(_personBestPopulation[indexChromosome].genesList[indexGenes]);
			}
			// Swap!!
			afterSwap(swapGeneListParent, swapGeneListChild, numGene, startIndex, endIndex);

			// Finished -- Copy Genes
			for (int indexGene = 0; indexGene < swapGeneListChild.Count; indexGene++)
			{
				tempVelocityGeneList[indexGene] = afterSwapGeneList[indexGene];
			}
		}

		void afterSwap(List<Gene> swapGeneListParent, List<Gene> swapGeneListChild, int numGene, int startIndex, int endIndex)
		{
			afterSwapGeneList.Clear();
			// Swap
			// 0 1 2 3 4 5
			// x s o o e x
			if (startIndex < endIndex)
			{
				for (int i = startIndex; i <= endIndex; i++)
				{
					swapGeneListChild[i] = swapGeneListParent[i];
				}
			}
			// 0 1 2 3 4 5
			// o e x x s o
			else if (startIndex > endIndex)
			{
				for (int i = startIndex; i < numGene; i++)
				{
					swapGeneListChild[i] = swapGeneListParent[i];
				}
				for (int i = 0; i <= endIndex; i++)
				{
					swapGeneListChild[i] = swapGeneListParent[i];
				}
			}
			// 0 1 2 3 4 5
			// x x x x s x
			// x x x x e x
			// x x x x o x
			else if (startIndex == endIndex)
			{
				swapGeneListChild[startIndex] = swapGeneListParent[startIndex];
			}

			// Finished -- Copy Genes
			for (int indexGene = 0; indexGene < swapGeneListChild.Count; indexGene++)
			{
				afterSwapGeneList.Add(swapGeneListChild[indexGene]);
			}
		}

		#endregion

		#region Update positions (chromosomes)
		public void UpdatePosition()
		{
			for (int indexChromosome = 0; indexChromosome < _particlePopulation.Count; indexChromosome++)
			{
				// Copy Genes
				for (int indexGene = 0; indexGene < _particlePopulation[indexChromosome].genesList.Count; indexGene++)
				{
					_particlePopulation[indexChromosome].genesList[indexGene] = _velocityPopulation[indexChromosome].genesList[indexGene];
				}
			}
		}
		#endregion

		#region BestChromesome
		public Chromosome BestChromesome()//Chromosome bestChromesome
		{
			CalculateFitnessScores();
			return _globalBestChromosome;
		}
		#endregion

		#region OutputData
		private List<string[]> basicData = new List<string[]>();
		string[] tileData = new string[4];

		void InitialData()
		{
			basicData.Clear();

			// Create the title of data
			tileData[0] = "Generation";
			tileData[1] = "ChromosomeIndex";
			tileData[2] = "Fitness_ImpassableDensity";
			tileData[3] = "Fitness_SumOfFitnessScore";
			basicData.Add(tileData);
		}

		public void SaveData(int indexGeneration)
		{
			for (int indexChromosome = 0; indexChromosome < _numChromosomes; indexChromosome++)
			{
				string[] contentData = new string[tileData.Length];
				contentData[0] = indexGeneration.ToString(); // Generation
				contentData[1] = indexChromosome.ToString(); // ChromosomeIndex
				contentData[2] = _particlePopulation[indexChromosome].FitnessScore[FitnessFunctionName.ImpassableDensity].ToString(); // Fitness_ImpassableDensity
				contentData[3] = _particlePopulation[indexChromosome].FitnessScore[FitnessFunctionName.SumOfFitnessScore].ToString(); // Fitness_SumOfFitnessScore
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
			string filePath = Application.dataPath + "/AutoGeneratedTactic/Data/" + "ParticleSwarmOptimization_Data_" + runGenerate + ".csv";
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
			for (int x = 0; x < _numChromosomes; x++)
			{
				Debug.Log("Chromosome[" + x + "] Fitness =" + _particlePopulation[x].FitnessScore[FitnessFunctionName.SumOfFitnessScore]);
			}
			Debug.Log("BestChromesome_SumOfFitnessScore = " + _globalBestChromosome.FitnessScore[FitnessFunctionName.SumOfFitnessScore]);
		}
		#endregion
	}
}

