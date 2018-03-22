using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;

using ChromosomeDefinition;

namespace GeneticAlgorithmSettingDefinition
{
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
				// Random
				for (int y = 0; y < numGene; y++)
				{
					_population[x].genesList.Add(new Gene());
					if (Random.Range(0, 2) == 0)
					{
						_population[x].genesList[y].type = GeneType.Empty;
					}
					else
					{
						_population[x].genesList[y].type = GeneType.Forbidden;
					}
				}
			}

			// InitialData
			InitialData();
		}
		#endregion

		#region Fitness
		FitnessFunctions FitnessFunction = new FitnessFunctions();
		float weight_RectangleQuality = 0.0f;
		float weight_CorridorQuality = 1.0f;
		float weight_ConnectedQuality = 1.0f;

		public void CalculateFitnessScores()
		{
			// Calculate the fitness score of chromosomes in population.
			for (int i = 0; i < _numChromosomes; i++)
			{
				//inital
				foreach (Gene gene in _population[i].genesList)
				{
					gene.SpaceAttribute = GeneSpaceAttribute.None;
				}
				// Fitness function
				_population[i].FitnessScore[FitnessFunctionName.RectangleQuality] = FitnessFunction.Fitness_RectangleQuality(_population[i], _mapLength, _mapWidth);
				_population[i].FitnessScore[FitnessFunctionName.CorridorQuality] = FitnessFunction.Fitness_CorridorQuality(_population[i], _mapLength, _mapWidth);
				_population[i].FitnessScore[FitnessFunctionName.ConnectedQuality] = FitnessFunction.Fitness_ConnectedQuality(_population[i], _mapLength, _mapWidth);
				_population[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = ( _population[i].FitnessScore[FitnessFunctionName.RectangleQuality] * weight_RectangleQuality
																					+ _population[i].FitnessScore[FitnessFunctionName.CorridorQuality] * weight_CorridorQuality
																					+ _population[i].FitnessScore[FitnessFunctionName.ConnectedQuality] * weight_ConnectedQuality ) / 3.0f;
			}
		}
		// Calculate the fitness score of chromosomes in specific population.
		void CalculatePopulationFitnessScores(List<Chromosome> _specificPopulation)
		{
			// Calculate the fitness score of chromosomes in population.
			for (int i = 0; i < _specificPopulation.Count; i++)
			{
				//inital
				foreach (Gene gene in _specificPopulation[i].genesList)
				{
					gene.SpaceAttribute = GeneSpaceAttribute.None;
				}
				// Fitness function
				_specificPopulation[i].FitnessScore[FitnessFunctionName.RectangleQuality] = FitnessFunction.Fitness_RectangleQuality(_specificPopulation[i], _mapLength, _mapWidth);
				_specificPopulation[i].FitnessScore[FitnessFunctionName.CorridorQuality] = FitnessFunction.Fitness_CorridorQuality(_specificPopulation[i], _mapLength, _mapWidth);
				_specificPopulation[i].FitnessScore[FitnessFunctionName.ConnectedQuality] =  FitnessFunction.Fitness_ConnectedQuality(_specificPopulation[i], _mapLength, _mapWidth);
				_specificPopulation[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore] = ( _specificPopulation[i].FitnessScore[FitnessFunctionName.RectangleQuality] * weight_RectangleQuality
																							+ _specificPopulation[i].FitnessScore[FitnessFunctionName.CorridorQuality] * weight_CorridorQuality
																							+ _specificPopulation[i].FitnessScore[FitnessFunctionName.ConnectedQuality] * weight_ConnectedQuality ) / 3.0f;
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
			_childsPopulation.Add(parent_1.Clone());
			_childsPopulation.Add(parent_2.Clone());
		}

		public void Crossover(float rateCrossover)
		{
			_rateCrossover = rateCrossover;

			RandomChromosomesIndex();
			// Get the parents
			Chromosome parent_A;
			Chromosome parent_B;
			int lengthArray = indexChromosomesArray.Length;
			int index_parent_A;
			int index_parent_B;

			for (int i = 0; i < lengthArray; i++)
			{
				index_parent_A = indexChromosomesArray[i % lengthArray];
				index_parent_B = indexChromosomesArray[i % lengthArray];
				// Clone the chromosomes which need to crossover.
				parent_A = _crossoverPoll[index_parent_A].Clone();
				parent_B = _crossoverPoll[index_parent_B].Clone();

				if (rateCrossover > Random.Range(0.0f, 1.0f))
				{
					// Step.3
					CrossoverMethod(parent_A, parent_B);
				}
				else
				{
					// Step.4
					_childsPopulation.Add(parent_A.Clone());
					_childsPopulation.Add(parent_B.Clone());
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

		void MutationMethod_ChangeGeneType(List<Gene> originalGenesList)
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
				while ((int)originalGenesList[indexGenesArray[i]].type == mutateGeneTypeArray[index_MutateGeneType])
				{
					index_MutateGeneType++;
				}
				// Step.3
				// Mutate the gene type.
				originalGenesList[indexGenesArray[i]].type = (GeneType)mutateGeneTypeArray[index_MutateGeneType];
			}
		}

		public void Mutation(float rateMutation)
		{
			_rateMutation = rateMutation;

			for (int i = 0; i < _numChromosomes; i++)
			{
				if (rateMutation > Random.Range(0.0f, 1.0f))
				{
					// Step.4
					MutationMethod_ChangeGeneType(_childsPopulation[i].genesList);
				}
			}
		}
		#endregion

		#region Replace
		List<Chromosome> _parentsChildsPopulation = new List<Chromosome>();

		public void Replace()
		{
			CalculatePopulationFitnessScores(_childsPopulation);

			// Copy the chromosomes from parentsPopulation and childsPopulation to this population.
			_parentsChildsPopulation.Clear();

			foreach (var parent in _population)
			{
				_parentsChildsPopulation.Add(parent.Clone());
			}

			foreach (var child in _childsPopulation)
			{
				_parentsChildsPopulation.Add(child.Clone());
			}

			// Sort the chromosomes
			var chromosomesOrder = from e in _parentsChildsPopulation
								   orderby e.FitnessScore[FitnessFunctionName.SumOfFitnessScore]
								   select e;

			_population.Clear();
			_childsPopulation.Clear();

			// Select best chromosomes
			int count = 0;
			foreach (var e in chromosomesOrder)
			{
				if (count >= ( _parentsChildsPopulation.Count - _numChromosomes ))
				{
					// Copy the chromosome
					_population.Add(e.Clone());
				}
				count++;
			}
		}
		#endregion

		#region BestChromesome
		int index_BestChromesome = 0;

		public Chromosome BestChromesome()
		{
			CalculateFitnessScores();

			// Search the best chromosome in population.
			for (int i = 0; i < _numChromosomes; i++)
			{
				if (_population[index_BestChromesome].FitnessScore[FitnessFunctionName.SumOfFitnessScore] < _population[i].FitnessScore[FitnessFunctionName.SumOfFitnessScore]
					&& _population[i].FitnessScore[FitnessFunctionName.ConnectedQuality] == 1)
				{
					index_BestChromesome = i;
				}
			}
			return _population[index_BestChromesome];
		}
		#endregion

		#region OutputData
		private List<string[]> basicData = new List<string[]>();
		string[] tileData = new string[6];

		void InitialData()
		{
			basicData.Clear();

			// Create the title of data
			tileData[0] = "Generation";
			tileData[1] = "ChromosomeIndex";
			tileData[2] = "Fitness_RectangleQuality";
			tileData[3] = "Fitness_CorridorQuality";
			tileData[4] = "Fitness_ConnectedQuality";
			tileData[5] = "Fitness_SumOfFitnessScore";
			basicData.Add(tileData);
		}

		public void SaveData(int indexGeneration)
		{
			for (int indexChromosome = 0; indexChromosome < _numChromosomes; indexChromosome++)
			{
				string[] contentData = new string[tileData.Length];
				contentData[0] = indexGeneration.ToString(); // Generation
				contentData[1] = indexChromosome.ToString(); // ChromosomeIndex
				contentData[2] = _population[indexChromosome].FitnessScore[FitnessFunctionName.RectangleQuality].ToString(); // Fitness_RectangleQuality
				contentData[3] = _population[indexChromosome].FitnessScore[FitnessFunctionName.CorridorQuality].ToString(); // Fitness_CorridorQuality
				contentData[4] = _population[indexChromosome].FitnessScore[FitnessFunctionName.ConnectedQuality].ToString(); // Fitness_ConnectedQuality
				contentData[5] = _population[indexChromosome].FitnessScore[FitnessFunctionName.SumOfFitnessScore].ToString(); // Fitness_SumOfFitnessScore
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
			string filePath = Application.dataPath + "/AutoGeneratedTactic/Data/Experiment/" + "GeneticAlgorithm_Data_" + _mapLength + "x" + _mapWidth + "_" + runGenerate + ".csv";
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
			Debug.Log("==BestChromesome==");
			Debug.Log("index = " + index_BestChromesome);
			Debug.Log("Fitness_RectangleQuality =" + _population[index_BestChromesome].FitnessScore[FitnessFunctionName.RectangleQuality]);
			Debug.Log("Fitness_CorridorQuality =" + _population[index_BestChromesome].FitnessScore[FitnessFunctionName.CorridorQuality]);
			Debug.Log("Fitness_ConnectedQuality =" + _population[index_BestChromesome].FitnessScore[FitnessFunctionName.ConnectedQuality]);
			Debug.Log("Fitness_SumOfFitnessScore =" + _population[index_BestChromesome].FitnessScore[FitnessFunctionName.SumOfFitnessScore]);		
		}
		#endregion
	}
}


