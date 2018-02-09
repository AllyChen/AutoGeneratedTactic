using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

using ChromosomeDefinition;

namespace GeneticAlgorithmSettingDefinition
{
	// Enum for type of gene.
	public enum GeneType
	{
		Forbidden = 0,
		Empty = 1,
		NumberOfGeneType
	}
	// Enum for fitness function.
	public enum FitnessFunctionName
	{
		ImpassableDensity,
		NumberOfFitnessFunctionName,
		SumOfFitnessScore
	}

	public class GeneticAlgorithmSetting : MonoBehaviour
	{
		#region Initial
		// Define the basic parameters.
		private int _mapLength;
		private int _mapWidth;
		private int _numGenes;
		private int _numChromosomes;
		private int _numGenerations;
		private List<Chromosome> _population = new List<Chromosome>();
		
		public void InitialPopulation(int length, int width, int numGene, int numChromosome, int numGeneration)
		{
			// Clean all the population.
			_population.Clear();
			_crossoverPoll.Clear();
			_childsPopulation.Clear();

			// Get the data of parameters.
			_mapLength = length;
			_mapWidth = width;
			_numGenes = numGene;
			_numChromosomes = numChromosome;
			_numGenerations = numGeneration;

			// Create the chromosomes in population.
			for (int x = 0; x < numChromosome; x++)
			{
				_population.Add(new Chromosome());

				// Create the genes in each chromosomes.
				for (int y = 0; y < numGene; y++)
				{
					_population[x].genesList.Add(new Gene());
					_population[x].genesList[y].type = GeneType.Empty;
				}
			}

			// InitialData
			InitialData();
		}
		#endregion

		#region Fitness
		FitnessFunctions FitnessFunction = new FitnessFunctions();

		public void CalculateFitnessScores()
		{
			// Calculate the fitness score of chromosomes in population.
			for (int i = 0; i < _numChromosomes; i++)
			{
				_population[i].FitnessScore[FitnessFunctionName.ImpassableDensity] = FitnessFunction.Fitness_ImpassableDensity(_population[i], _numGenes, _mapLength, _mapWidth);
				_population[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = _population[i].FitnessScore[FitnessFunctionName.ImpassableDensity];
			}
		}
		#endregion

		#region Selection
		/// <summary>
		/// Roulette Wheel Selection
		/// <remarks>
		/// Is a kind of Fitness Proportionate Selection. 
		/// <method>
		/// Step.1 計算所有染色體的適應分數之和，為分母。
		/// Step.2 將染色體的適應分數之和除以分母後，為該染色體被選擇的機率。
		/// Step.3 將各染色體被選擇的機率，依序累加存入rouletteWheel陣列中。
		/// Step.4 依據 rouletteWheel List 紀錄的機率，將各染色體複製後放入 _crossoverPoll。
		/// </method>
		/// </remarks>
		/// </summary>

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
		/// Two-Point Crossover (C2)
		/// <remarks>
		/// Two-point crossover calls for two points to be selected on the parents. 
		/// Everything between the two points is swapped between the parents, rendering two children.
		/// <method>
		/// Step.1 隨機取得父母染色體。在indexChromosomesArray裡儲存染色體的Index，並將此陣列隨機洗牌。
		/// Step.2 利用陣列內的隨機Index值，依序從 _crossoverPoll 裡拿染色體進行一定機率的交配。
		/// Step.3 交配後的子代存入 _childsPopulation。
		/// Step.4 若染色體沒進行交配則原封不動放入。
		/// </method>
		/// </remarks>
		/// </summary>

		float _rateCrossover;
		int[] indexChromosomesArray;
		List<Chromosome> _childsPopulation = new List<Chromosome>();

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

		void CrossoverMethod(Chromosome parent_1, Chromosome parent_2)
		{
			int start = Random.Range(0, parent_1.genesList.Count);
			int end = Random.Range(start, parent_1.genesList.Count);

			for (int i = start; i < end; i++)
			{
				Gene swapGene = parent_1.genesList[i];
				parent_1.genesList[i] = parent_2.genesList[i];
				parent_2.genesList[i] = swapGene;
			}
			// After crossover
			_childsPopulation.Add(parent_1);
			_childsPopulation.Add(parent_2);
		}

		public void Crossover(float rateCrossover)
		{
			_rateCrossover = rateCrossover;

			RandomChromosomesIndex();
			// Get the parents
			Chromosome parent_A;
			Chromosome parent_B;
			int lengthArray = indexChromosomesArray.Length;

			for (int i = 0; i < lengthArray; i++)
			{
				parent_A = _crossoverPoll[i % lengthArray];
				parent_B = _crossoverPoll[(i + 1) % lengthArray];

				if (rateCrossover > Random.Range(0.0f, 1.0f))
				{
					// Step.3
					CrossoverMethod(parent_A, parent_B);
				}
				else
				{
					// Step.4
					_childsPopulation.Add(parent_A);
					_childsPopulation.Add(parent_B);
				}		
			}
		}
		#endregion

		#region Mutation
		/// <summary>
		/// Takes the chosen genome and inverts the bits (i.e. if the genome bit is 1, it is changed to 0 and vice versa).
		/// <remarks>
		/// When using this mutation the genetic algorithm should use IBinaryChromosome.
		/// <method>
		/// Step.1 隨機取得要mutate的基因。在陣列裡儲存基因的Index，並將此陣列隨機洗牌。
		/// Step.2 將要變異成何種基因Type做優先排序。
		/// 如：變異的順序為Type B D A C，若要進行變異的基因之原始Type為B，則因與第一優先變異Type一樣，所以改為變異成D。
		/// Step.3 進行變異。
		/// Step.4 放回 _childsPopulation。					
		/// </method>
		/// </remarks>
		/// </summary>

		float _rateMutation;
		int[] indexGenesArray;
		int[] mutateGeneTypeArray;

		// Step.1
		void RandomGenesIndex()
		{
			// Initial the array.
			indexGenesArray = new int[_numGenes];
			for (int index = 0; index < _numGenes; index++)
			{
				indexGenesArray[index] = index;
			}

			// Random the index array.
			for (int index = _numGenes - 1; index > 0; index--)
			{
				int randomArrayIndex = Random.Range(0, index);
				int tempValue = indexGenesArray[randomArrayIndex];
				// Swap the value with the last one.
				indexGenesArray[randomArrayIndex] = indexGenesArray[index];
				indexGenesArray[index] = tempValue;
			}
		}

		// Step.2
		void RandomMutateGeneType()
		{
			// The count of GeneType
			int count_GeneType = (int)GeneType.NumberOfGeneType;

			// Initial the array.
			mutateGeneTypeArray = new int[count_GeneType];
			for (int index = 0; index < count_GeneType; index++)
			{
				mutateGeneTypeArray[index] = index;
			}

			// Random the index array.
			for (int index = count_GeneType - 1; index > 0; index--)
			{
				int randomArrayIndex = Random.Range(0, index);
				int tempValue = mutateGeneTypeArray[randomArrayIndex];
				// Swap the value with the last one.
				mutateGeneTypeArray[randomArrayIndex] = mutateGeneTypeArray[index];
				mutateGeneTypeArray[index] = tempValue;
			}
		}

		Chromosome MutationMethod_ChangeGeneType(Chromosome originalChromosome)
		{
			int maxNumGene = _numGenes;
			int numMutateGenes = 1;//Random.Range(1, _numGenes % 5);
			int index_MutateGeneType;

			// Step.1
			RandomGenesIndex();

			for (int i = 0; i < numMutateGenes; i++)
			{
				index_MutateGeneType = 0;
				// Step.2
				RandomMutateGeneType();
				while ((int)originalChromosome.genesList[indexGenesArray[i]].type == mutateGeneTypeArray[index_MutateGeneType])
				{
					index_MutateGeneType++;
				}
				// Step.3
				// Mutate the gene type.
				originalChromosome.genesList[indexGenesArray[i]].type = (GeneType)mutateGeneTypeArray[index_MutateGeneType];
			}

			return originalChromosome;
		}

		public void Mutation(float rateMutation)
		{
			_rateMutation = rateMutation;

			for (int i = 0; i < _numChromosomes; i++)
			{
				if (rateMutation > Random.Range(0.0f, 1.0f))
				{
					// Step.4
					_childsPopulation[i] = MutationMethod_ChangeGeneType(_childsPopulation[i]);
				}
			}
		}
		#endregion

		#region Replace
		public void Replace()
		{
			_population = _childsPopulation;
		}
		#endregion

		#region BestChromesome
		int index_BestChromesome = 0;

		public Chromosome BestChromesome()//Chromosome bestChromesome)
		{
			CalculateFitnessScores();

			// Search the best chromosome in population.
			for (int i = 0; i < _numChromosomes; i++)
			{
				if (_population[index_BestChromesome].FitnessScore[FitnessFunctionName.SumOfFitnessScore] < _population[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore])
				{
					index_BestChromesome = i;
				}
			}

			return _population[index_BestChromesome];
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
				contentData[2] = _population[indexChromosome].FitnessScore[FitnessFunctionName.ImpassableDensity].ToString(); // Fitness_ImpassableDensity
				contentData[3] = _population[indexChromosome].FitnessScore[FitnessFunctionName.SumOfFitnessScore].ToString(); // Fitness_SumOfFitnessScore
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
			string filePath = Application.dataPath + "/AutoGeneratedTactic/Data/" + "GeneticAlgorithm_Data_" + runGenerate + ".csv";
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
			Debug.Log("==Last Population==");
			for (int x = 0; x < _numChromosomes; x++)
			{
				Debug.Log("Chromosome[" + x + "] Fitness =" + _population[x].FitnessScore[FitnessFunctionName.ImpassableDensity]);
			}
			Debug.Log("index_BestChromesome = " + index_BestChromesome);
		}
		#endregion
	}
}


