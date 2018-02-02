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

		public Dictionary<FitnessFunctionName, float> FitnessScore = new Dictionary<FitnessFunctionName, float>() {
					{ FitnessFunctionName.ImpassableDensity   , 0.0f },
					{ FitnessFunctionName.SumOfFitnessScore   , 0.0f },
				};
	}

	public class Gene
	{
		public Vector3 position;
		public GeneType type;
	}
}

