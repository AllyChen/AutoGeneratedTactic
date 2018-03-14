using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GeneticAlgorithmSettingDefinition;

namespace ChromosomeDefinition
{
	// Enum for type of gene.
	public enum GeneType
	{
		Forbidden = 0,
		Empty = 1,
		NumberOfGeneType
	}

	// Enum for space attribute of gene.
	public enum GeneSpaceAttribute
	{
		None = 0,
		Rectangle = 1,
		Corridor = 2,
		Turn = 3,
		NumberOfGeneSpaceAttribute
	}

	// Enum for GameObject attribute of gene.
	public enum GeneGameObjectAttribute
	{
		None = 0,
		entrance = 1,
		exit = 2,
		enemy = 3,
		trap = 4,
		treasure = 5,
		NumberOfGeneSpaceAttribute
	}

	// Enum for fitness function.
	public enum FitnessFunctionName
	{
		ImpassableDensity,
		RectangleQuality,
		CorridorQuality,
		ConnectedQuality,
		NumberOfFitnessFunctionName,
		SumOfFitnessScore
	}

	public class Chromosome
	{
		// Genes
		public List<Gene> genesList = new List<Gene>();
		// FitnessScore
		public Dictionary<FitnessFunctionName, float> FitnessScore = new Dictionary<FitnessFunctionName, float>() {
					{ FitnessFunctionName.ImpassableDensity   , 0.0f },
					{ FitnessFunctionName.RectangleQuality   , 0.0f },
					{ FitnessFunctionName.CorridorQuality   , 0.0f },
					{ FitnessFunctionName.ConnectedQuality   , 0.0f },
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
			ChromosomeClone.FitnessScore[FitnessFunctionName.RectangleQuality] = this.FitnessScore[FitnessFunctionName.RectangleQuality];
			ChromosomeClone.FitnessScore[FitnessFunctionName.CorridorQuality] = this.FitnessScore[FitnessFunctionName.CorridorQuality];
			ChromosomeClone.FitnessScore[FitnessFunctionName.ConnectedQuality] = this.FitnessScore[FitnessFunctionName.ConnectedQuality];
			ChromosomeClone.FitnessScore[FitnessFunctionName.SumOfFitnessScore] = this.FitnessScore[FitnessFunctionName.SumOfFitnessScore];

			return ChromosomeClone;
		}

		// Space
		public List<SpaceInfo> spaceList = new List<SpaceInfo>();

		public Chromosome CloneSpace()
		{
			var ChromosomeClone = new Chromosome();
			// Genes
			var genesListClone = new List<Gene>();
			foreach (var originalGene in this.genesList)
			{
				Gene gene = new Gene();
				gene.position = originalGene.position;
				gene.type = originalGene.type;
				gene.SpaceAttribute = originalGene.SpaceAttribute;
				genesListClone.Add(gene);
			}
			ChromosomeClone.genesList = genesListClone;

			// SpaceList
			var spaceListClone = new List<SpaceInfo>();
			foreach (var originalSpaceInfo in this.spaceList)
			{
				SpaceInfo spaceInfo = new SpaceInfo();
				spaceInfo.startPos = originalSpaceInfo.startPos;
				spaceInfo.length = originalSpaceInfo.length;
				spaceInfo.width = originalSpaceInfo.width;
				spaceInfo.SpaceAttribute = originalSpaceInfo.SpaceAttribute;
				spaceListClone.Add(spaceInfo);
			}
			ChromosomeClone.spaceList = spaceListClone;

			return ChromosomeClone;
		}

	}

	public class Gene
	{
		public Vector3 position;
		public GeneType type;
		public GeneSpaceAttribute SpaceAttribute;
		public GeneGameObjectAttribute GameObjectAttribute;
	}

	public class SpaceInfo
	{
		public int startPos;
		public int length;
		public int width;
		public GeneSpaceAttribute SpaceAttribute;
	}
	// we calculate the root which connected the other spaces
	public class SpaceConnected_Root
	{
		public int spaceIndex;
		public List<SpaceConnected_Leaf> connectedLeaf = new List<SpaceConnected_Leaf>();
	}
	// we calculate the Leaf which connected the Root
	public class SpaceConnected_Leaf
	{
		public int spaceIndex;
	}
}

