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
		MainPathQuality,
		Fitness_Defense,
		Fitness_OnMainPath,
		Fitness_BesideMainPath,
		Fitness_TwoPronged,
		NumberOfFitnessFunctionName,
		SumOfFitnessScore
	}

	public class Chromosome
	{
		// Genes
		public List<Gene> genesList = new List<Gene>();
		// FitnessScore
		public Dictionary<FitnessFunctionName, float> FitnessScore = new Dictionary<FitnessFunctionName, float>() {
					{ FitnessFunctionName.ImpassableDensity			, 0.0f },
					{ FitnessFunctionName.RectangleQuality			, 0.0f },
					{ FitnessFunctionName.CorridorQuality			, 0.0f },
					{ FitnessFunctionName.ConnectedQuality			, 0.0f },
					{ FitnessFunctionName.MainPathQuality			, 0.0f },
					{ FitnessFunctionName.Fitness_Defense			, 0.0f },
					{ FitnessFunctionName.Fitness_OnMainPath		, 0.0f },
					{ FitnessFunctionName.Fitness_BesideMainPath	, 0.0f },
					{ FitnessFunctionName.Fitness_TwoPronged		, 0.0f },
					{ FitnessFunctionName.SumOfFitnessScore			, 0.0f },
				};

		public void copyFitnessScore(Chromosome sourceChromosome)
		{
			this.FitnessScore[FitnessFunctionName.ImpassableDensity] = sourceChromosome.FitnessScore[FitnessFunctionName.ImpassableDensity];
			this.FitnessScore[FitnessFunctionName.RectangleQuality] = sourceChromosome.FitnessScore[FitnessFunctionName.RectangleQuality];
			this.FitnessScore[FitnessFunctionName.CorridorQuality] = sourceChromosome.FitnessScore[FitnessFunctionName.CorridorQuality];
			this.FitnessScore[FitnessFunctionName.ConnectedQuality] = sourceChromosome.FitnessScore[FitnessFunctionName.ConnectedQuality];
			this.FitnessScore[FitnessFunctionName.MainPathQuality] = sourceChromosome.FitnessScore[FitnessFunctionName.MainPathQuality];
			this.FitnessScore[FitnessFunctionName.Fitness_Defense] = sourceChromosome.FitnessScore[FitnessFunctionName.Fitness_Defense];
			this.FitnessScore[FitnessFunctionName.Fitness_OnMainPath] = sourceChromosome.FitnessScore[FitnessFunctionName.Fitness_OnMainPath];
			this.FitnessScore[FitnessFunctionName.Fitness_BesideMainPath] = sourceChromosome.FitnessScore[FitnessFunctionName.Fitness_BesideMainPath];
			this.FitnessScore[FitnessFunctionName.Fitness_TwoPronged] = sourceChromosome.FitnessScore[FitnessFunctionName.Fitness_TwoPronged];
			this.FitnessScore[FitnessFunctionName.SumOfFitnessScore] = sourceChromosome.FitnessScore[FitnessFunctionName.SumOfFitnessScore];
		}

		public Chromosome Clone()
		{
			var ChromosomeClone = new Chromosome();
			// Genes
			var genesListClone = new List<Gene>();
			foreach (var originalGene in this.genesList)
			{
				Gene gene = new Gene();
				gene.type = originalGene.type;
				genesListClone.Add(gene);
			}
			ChromosomeClone.genesList = genesListClone;

			// FitnessScore
			ChromosomeClone.FitnessScore[FitnessFunctionName.ImpassableDensity] = this.FitnessScore[FitnessFunctionName.ImpassableDensity];
			ChromosomeClone.FitnessScore[FitnessFunctionName.RectangleQuality] = this.FitnessScore[FitnessFunctionName.RectangleQuality];
			ChromosomeClone.FitnessScore[FitnessFunctionName.CorridorQuality] = this.FitnessScore[FitnessFunctionName.CorridorQuality];
			ChromosomeClone.FitnessScore[FitnessFunctionName.ConnectedQuality] = this.FitnessScore[FitnessFunctionName.ConnectedQuality];
			ChromosomeClone.FitnessScore[FitnessFunctionName.MainPathQuality] = this.FitnessScore[FitnessFunctionName.MainPathQuality];
			ChromosomeClone.FitnessScore[FitnessFunctionName.Fitness_Defense] = this.FitnessScore[FitnessFunctionName.Fitness_Defense];
			ChromosomeClone.FitnessScore[FitnessFunctionName.Fitness_OnMainPath] = this.FitnessScore[FitnessFunctionName.Fitness_OnMainPath];
			ChromosomeClone.FitnessScore[FitnessFunctionName.Fitness_BesideMainPath] = this.FitnessScore[FitnessFunctionName.Fitness_BesideMainPath];
			ChromosomeClone.FitnessScore[FitnessFunctionName.Fitness_TwoPronged] = this.FitnessScore[FitnessFunctionName.Fitness_TwoPronged];
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
				gene.type = originalGene.type;
				gene.SpaceAttribute = originalGene.SpaceAttribute;
				gene.isMainPath = originalGene.isMainPath;
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
		// GameObject
		public List<GameObjectInfo> gameObjectList = new List<GameObjectInfo>();

		public void AddGameObjectInList(int Position, GeneGameObjectAttribute GameObjectAttribute)
		{
			this.gameObjectList.Add(new GameObjectInfo());
			this.gameObjectList[gameObjectList.Count - 1].Position = Position;
			this.gameObjectList[gameObjectList.Count - 1].GameObjectAttribute = GameObjectAttribute;
		}

		public void settingGameObject()
		{
			// Initial
			foreach (var gene in this.genesList)
			{
				gene.GameObjectAttribute = GeneGameObjectAttribute.None;
			}
			// Setting the game object
			foreach (var gameObject in this.gameObjectList)
			{
				genesList[gameObject.Position].GameObjectAttribute = gameObject.GameObjectAttribute;
			}
		}

		public void settingGameObject(List<GameObjectInfo> sourceGameObject)
		{
			// Initial
			this.gameObjectList.Clear();

			foreach (var gene in this.genesList)
			{
				gene.GameObjectAttribute = GeneGameObjectAttribute.None;
			}
			// Setting the game object
			foreach (var item in sourceGameObject)
			{
				this.gameObjectList.Add(new GameObjectInfo());
				this.gameObjectList[this.gameObjectList.Count - 1].Position = item.Position;
				this.gameObjectList[this.gameObjectList.Count - 1].GameObjectAttribute = item.GameObjectAttribute;
				genesList[item.Position].GameObjectAttribute = item.GameObjectAttribute;
			}
		}

		public Chromosome CloneSpaceGameObject()
		{
			var ChromosomeClone = new Chromosome();
			// Genes
			var genesListClone = new List<Gene>();
			foreach (var originalGene in this.genesList)
			{
				Gene gene = new Gene();
				gene.type = originalGene.type;
				gene.SpaceAttribute = originalGene.SpaceAttribute;
				gene.GameObjectAttribute = originalGene.GameObjectAttribute;
				gene.isMainPath = originalGene.isMainPath;
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

			// gameObjectList
			var gameObjectListClone = new List<GameObjectInfo>();
			foreach (var originalGameObjectList in this.gameObjectList)
			{
				GameObjectInfo gameObjectInfo = new GameObjectInfo();
				gameObjectInfo.Position = originalGameObjectList.Position;
				gameObjectInfo.GameObjectAttribute = originalGameObjectList.GameObjectAttribute;
				gameObjectListClone.Add(gameObjectInfo);
			}
			ChromosomeClone.gameObjectList = gameObjectListClone;

			// Main Path
			var mainPathListClone = new List<int>();
			foreach (var originalmainPath in this.mainPath)
			{
				mainPathListClone.Add(originalmainPath);
			}
			ChromosomeClone.mainPath = mainPathListClone;

			return ChromosomeClone;
		}
		// Main Path
		public List<int> mainPath = new List<int>();

		public void settingMainPath()
		{
			foreach (var gene in this.genesList)
			{
				gene.isMainPath = false;
			}
			foreach (var index in this.mainPath)
			{
				this.genesList[index].isMainPath = true;
			}
		}
	}

	public class Gene
	{
		public GeneType type;
		public GeneSpaceAttribute SpaceAttribute;
		public GeneGameObjectAttribute GameObjectAttribute;
		public bool isMainPath;
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

	public class GameObjectInfo
	{
		public int Position;
		public GeneGameObjectAttribute GameObjectAttribute;
	}
}

