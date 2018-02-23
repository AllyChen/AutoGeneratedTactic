using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GeneticAlgorithmSettingDefinition;

namespace ChromosomeDefinition
{
	public class Chromosome
	{
		// Genes
		public List<Gene> genesList = new List<Gene>();
		// FitnessScore
		public Dictionary<FitnessFunctionName, float> FitnessScore = new Dictionary<FitnessFunctionName, float>() {
					{ FitnessFunctionName.ImpassableDensity   , 0.0f },
					{ FitnessFunctionName.SumOfFitnessScore   , 0.0f },
				};

		public Chromosome Clone()
		{
			var ChromosomeClone = new Chromosome();
			// Genes
			var genesListClone = new List<Gene>();
			foreach (var originalGene in this.genesList)
			{
				Gene gene = new Gene();
				gene.position = originalGene.position;
				gene.type = originalGene.type;
				genesListClone.Add(gene);
			}
			ChromosomeClone.genesList = genesListClone;

			// FitnessScore
			ChromosomeClone.FitnessScore[FitnessFunctionName.ImpassableDensity] = this.FitnessScore[FitnessFunctionName.ImpassableDensity];
			ChromosomeClone.FitnessScore[FitnessFunctionName.SumOfFitnessScore] = this.FitnessScore[FitnessFunctionName.SumOfFitnessScore];

			return ChromosomeClone;
		}
	}

	public class Gene
	{
		public Vector3 position;
		public GeneType type;
	}
}

