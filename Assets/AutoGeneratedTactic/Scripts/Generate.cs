using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using GeneticAlgorithmSettingDefinition;
using ChromosomeDefinition;
using GeneticAlgorithmSettingGameObjectDefinition;
using ParticleSwarmOptimizationSettingDefinition;
using DataSerializationDefinition;

public class Generate : MonoBehaviour {

	private GameObject ParameterSetting;
	private GameObject AutoTacticRender;
	private AutoTacticRenderHandler TacticRenderHandlar;
	private GameObject GeneticAlgorithmSetting;
	private GeneticAlgorithmSetting GeneticAlgorithm;
	private GeneticAlgorithmSettingGameObject GeneticAlgorithmSettingGameObject;
	private ParticleSwarmOptimizationSetting ParticleSwarmOptimization;
	private DataSerialization DataSerialization;

	private int runGenerate = 0;
	private Chromosome BestChromosome_Space = new Chromosome();
	private Chromosome BestChromosome = new Chromosome();

	private bool outputData = true;

	// Use this for initialization
	void Start () {
		ParameterSetting = GameObject.Find("ParameterSetting");
		AutoTacticRender = GameObject.Find("AutoTacticRender");
		TacticRenderHandlar = GameObject.Find("AutoTacticRender").GetComponent<AutoTacticRenderHandler>();
		GeneticAlgorithm = GameObject.Find("GeneticAlgorithmSetting").GetComponent<GeneticAlgorithmSetting>();
		GeneticAlgorithmSettingGameObject = GameObject.Find("GeneticAlgorithmSettingGameObject").GetComponent<GeneticAlgorithmSettingGameObject>();
		ParticleSwarmOptimization = GameObject.Find("ParticleSwarmOptimizationSetting").GetComponent<ParticleSwarmOptimizationSetting>();
		DataSerialization = GameObject.Find("OutputData").GetComponent<DataSerialization>() ?? GameObject.Find("OutputData").AddComponent<DataSerialization>();
	}

	int useMethod = 0;// 0:GeneticAlgorithm, 1:ParticleSwarmOptimization
	int numGeneration = 50;
	int numChromosome = 100;
	float rato_crossover = 0.8f;
	float rato_mutation = 1.0f;
	int numGenerationGameObject = 50;
	int numChromosomeGameObject = 100;
	float ratio_GameObjectCrossover = 0.8f;
	float ratio_GameObjectMutation = 1.0f;
	string spaceID = "";
	string gameObjectID = "";

	// Get the Parameters
	int length;
	int width;
	int minEnemy;
	int minTrap;
	int minTreasure;
	int maxEnemy;
	int maxTrap;
	int maxTreasure;
	int[] numMinGameObject;
	int[] numMaxGameObject;

	bool fitness_Rectangle;
	bool fitness_Corridor;
	bool fitness_Defense;
	bool fitness_OnMainPath;
	bool fitness_BesideMainPath;
	bool fitness_TwoPronged;

	bool isTreasureOnMainPath;
	bool isTreasureBesideMainPath;

	bool Tactic_Bait;
	bool Tactic_Ambush;
	bool Tactic_TwoProngedAttack;
	bool Tactic_Defense;
	bool Tactic_Clash;

	float weight_RectangleQuality;
	float weight_CorridorQuality;
	float weight_Fitness_Defense;
	float weight_Fitness_OnMainPath;
	float weight_Fitness_BesideMainPath;
	float weight_Fitness_TwoPronged;
	float weight_Tactic_Bait;
	float weight_Tactic_Ambush;
	float weight_Tactic_TwoProngedAttack;
	float weight_Tactic_Defense;
	float weight_Tactic_Clash;

	public void OnClick_Generate()
	{
		GetParameters(false, false, false, false, false);
		completelyGenerate();
		//if (outputData == true)
		//{
		//	OnClick_OutputAutoTacticData();
		//}

		//bool[] tacticArray = new bool[5] { false, false, false, false, false };
		//float[] tacticWeightArray = new float[5] { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

		//for (int partnerA = 0; partnerA < 5; partnerA++)
		//{
		//	for (int partnerB = 0; partnerB < 5; partnerB++)
		//	{
		//		if (partnerA != partnerB)
		//		{
		//			// inital
		//			for (int i = 0; i < 5; i++)
		//			{
		//				tacticArray[i] = false;
		//				tacticWeightArray[i] = 0.0f;
		//			}
		//			// Setting
		//			tacticArray[partnerA] = true;
		//			tacticArray[partnerB] = true;
		//			tacticWeightArray[partnerA] = 0.8f;
		//			tacticWeightArray[partnerB] = 0.2f;

		//			// Start to generate
		//			for (int time = 0; time < 2; time++)
		//			{
		//				GetParameters(tacticArray[0], tacticArray[1], tacticArray[2], tacticArray[3], tacticArray[4]);
		//				weight_Tactic_Bait = tacticWeightArray[0];
		//				weight_Tactic_Ambush = tacticWeightArray[1];
		//				weight_Tactic_TwoProngedAttack = tacticWeightArray[2];
		//				weight_Tactic_Defense = tacticWeightArray[3];
		//				weight_Tactic_Clash = tacticWeightArray[4];
		//				completelyGenerate();
		//				OnClick_OutputAutoTacticData();
		//			}


		//			// Setting
		//			tacticWeightArray[partnerA] = 0.6f;
		//			tacticWeightArray[partnerB] = 0.4f;

		//			// Start to generate another weight one
		//			for (int time = 0; time < 2; time++)
		//			{
		//				weight_Tactic_Bait = tacticWeightArray[0];
		//				weight_Tactic_Ambush = tacticWeightArray[1];
		//				weight_Tactic_TwoProngedAttack = tacticWeightArray[2];
		//				weight_Tactic_Defense = tacticWeightArray[3];
		//				weight_Tactic_Clash = tacticWeightArray[4];
		//				completelyGenerate();
		//				OnClick_OutputAutoTacticData();
		//			}					
		//		}
		//	}
		//}



		//for (int time_Bait = 0; time_Bait < 10; time_Bait++)
		//{
		//	GetParameters(true, false, false, false, false);
		//	completelyGenerate();
		//	OnClick_OutputAutoTacticData();
		//}
		//for (int time_Ambush = 0; time_Ambush < 10; time_Ambush++)
		//{
		//	GetParameters(false, true, false, false, false);
		//	completelyGenerate();
		//	OnClick_OutputAutoTacticData();
		//}
		//for (int time_Pronged = 0; time_Pronged < 10; time_Pronged++)
		//{
		//	GetParameters(false, false, true, false, false);
		//	completelyGenerate();
		//	OnClick_OutputAutoTacticData();
		//}
		//for (int time_Defense = 0; time_Defense < 10; time_Defense++)
		//{
		//	GetParameters(false, false, false, true, false);
		//	completelyGenerate();
		//	OnClick_OutputAutoTacticData();
		//}
		//for (int time_Clash = 0; time_Clash < 10; time_Clash++)
		//{
		//	GetParameters(false, false, false, false, true);
		//	completelyGenerate();
		//	OnClick_OutputAutoTacticData();
		//}
	}

	void completelyGenerate()
	{
		var startTime = Time.realtimeSinceStartup;
		spaceID = DateTime.Now.ToString("MMddhhmmss");

		#region GenerateSpace
		switch (useMethod)
		{
			case 0:
				// Start GeneticAlgorithm
				GeneticAlgorithm.InitialPopulation(length, width, length * width, numChromosome, numGeneration);
				GeneticAlgorithm.DetermineWeightFitness(fitness_Rectangle, fitness_Corridor, weight_RectangleQuality, weight_CorridorQuality
					, Tactic_Bait, Tactic_Ambush, Tactic_TwoProngedAttack, Tactic_Defense, Tactic_Clash
					, weight_Tactic_Bait, weight_Tactic_Ambush, weight_Tactic_TwoProngedAttack, weight_Tactic_Defense, weight_Tactic_Clash
					, Tactic_Fitness_Rectangle, Tactic_Fitness_Corridor);

				for (int num_generation = 0; num_generation < numGeneration; num_generation++)
				{
					GeneticAlgorithm.CalculateFitnessScores();
					if (outputData == true)
					{
						GeneticAlgorithm.SaveData(num_generation);
					}					
					GeneticAlgorithm.Selection();
					GeneticAlgorithm.Crossover(rato_crossover);
					GeneticAlgorithm.Mutation(rato_mutation);
					GeneticAlgorithm.Replace();
				}

				BestChromosome_Space = GeneticAlgorithm.BestChromosome().CloneSpace();
				if (outputData == true)
				{
					GeneticAlgorithm.SaveData(numGeneration);
					GeneticAlgorithm.OutputData(spaceID);
				}
				//GeneticAlgorithm.DebugTest();
				//Time
				var GAendTime = Time.realtimeSinceStartup - startTime;
				Debug.Log(length + " x " + width + "GeneticAlgorithm_Time = " + GAendTime);
				break;
				//case 1:
				//	// Start ParticleSwarmOptimization
				//	ParticleSwarmOptimization.InitialPopulation(length, width, length * width, numChromosome, numGeneration);

				//	for (int num_generation = 0; num_generation < numGeneration; num_generation++)
				//	{
				//		ParticleSwarmOptimization.CalculateFitnessScores();
				//		ParticleSwarmOptimization.SaveData(num_generation);
				//		ParticleSwarmOptimization.UpdateVelocities();
				//		ParticleSwarmOptimization.UpdatePosition();
				//	}

				//	BestChromosome = ParticleSwarmOptimization.BestChromosome();
				//	ParticleSwarmOptimization.SaveData(numGeneration);
				//	ParticleSwarmOptimization.OutputData(runGenerate);

				//	//ParticleSwarmOptimization.DebugTest();
				//	//Time
				//	//var PSOendTime = Time.realtimeSinceStartup - startTime;
				//	//Debug.Log(length + " x " + width + "ParticleSwarmOptimization_Time = " + PSOendTime);
				//	break;
		}
		#endregion

		#region GenerateGameObject
		gameObjectID = DateTime.Now.ToString("MMddhhmmss");
		GeneticAlgorithmSettingGameObject.InitialPopulation(length, width, length * width, numChromosomeGameObject, numGenerationGameObject, BestChromosome_Space, numMinGameObject, numMaxGameObject);
		GeneticAlgorithmSettingGameObject.DetermineWeightFitness(fitness_Defense, fitness_OnMainPath, fitness_BesideMainPath, fitness_TwoPronged, isTreasureOnMainPath, isTreasureBesideMainPath
																, weight_Fitness_Defense, weight_Fitness_OnMainPath, weight_Fitness_BesideMainPath, weight_Fitness_TwoPronged
																, Tactic_Bait, Tactic_Ambush, Tactic_TwoProngedAttack, Tactic_Defense, Tactic_Clash
																, weight_Tactic_Bait, weight_Tactic_Ambush, weight_Tactic_TwoProngedAttack, weight_Tactic_Defense, weight_Tactic_Clash
																, Tactic_Clash_fitness_OnMainPath, Tactic_Clash_fitness_BesideMainPath);

		for (int num_generation = 0; num_generation < numGenerationGameObject; num_generation++)
		{
			GeneticAlgorithmSettingGameObject.CalculateFitnessScores();
			if (outputData == true)
			{
				GeneticAlgorithmSettingGameObject.SaveData(num_generation);
			}		
			GeneticAlgorithmSettingGameObject.Selection();
			GeneticAlgorithmSettingGameObject.Crossover(ratio_GameObjectCrossover);
			GeneticAlgorithmSettingGameObject.Mutation(ratio_GameObjectMutation);
			GeneticAlgorithmSettingGameObject.Replace();
		}
		BestChromosome = GeneticAlgorithmSettingGameObject.BestChromosome();

		//Time
		var GAGOendTime = Time.realtimeSinceStartup - startTime;
		Debug.Log(length + " x " + width + "GeneticAlgorithmGameObject_Time = " + GAGOendTime);

		if (outputData == true)
		{
			GeneticAlgorithmSettingGameObject.SaveData(numGenerationGameObject);
			GeneticAlgorithmSettingGameObject.OutputData(spaceID, gameObjectID);
		}
		#endregion

		// Render the tiles.
		TacticRenderHandlar.CleanBoard(AutoTacticRender);
		TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromosome);
		isClickOnlySpace = false;
		runGenerate++;
	}

	int runGenerateGameObject = 1;
	public void OnClick_GenerateGameObject()
	{
		gameObjectID = DateTime.Now.ToString("MMddhhmmss");
		GetParameters(false, false, false, false, false);

		GeneticAlgorithmSettingGameObject.InitialPopulation(length, width, length * width, numChromosomeGameObject, numGenerationGameObject, BestChromosome_Space, numMinGameObject, numMaxGameObject);
		GeneticAlgorithmSettingGameObject.DetermineWeightFitness(fitness_Defense, fitness_OnMainPath, fitness_BesideMainPath, fitness_TwoPronged, isTreasureOnMainPath, isTreasureBesideMainPath
																, weight_Fitness_Defense, weight_Fitness_OnMainPath, weight_Fitness_BesideMainPath, weight_Fitness_TwoPronged
																, Tactic_Bait, Tactic_Ambush, Tactic_TwoProngedAttack, Tactic_Defense, Tactic_Clash
																, weight_Tactic_Bait, weight_Tactic_Ambush, weight_Tactic_TwoProngedAttack, weight_Tactic_Defense, weight_Tactic_Clash
																, Tactic_Clash_fitness_OnMainPath, Tactic_Clash_fitness_BesideMainPath);

		for (int num_generation = 0; num_generation < numGenerationGameObject; num_generation++)
		{
			GeneticAlgorithmSettingGameObject.CalculateFitnessScores();
			if (outputData == true)
			{
				GeneticAlgorithmSettingGameObject.SaveData(num_generation);
			}			
			GeneticAlgorithmSettingGameObject.Selection();
			GeneticAlgorithmSettingGameObject.Crossover(ratio_GameObjectCrossover);
			GeneticAlgorithmSettingGameObject.Mutation(ratio_GameObjectMutation);
			GeneticAlgorithmSettingGameObject.Replace();
		}
		BestChromosome = GeneticAlgorithmSettingGameObject.BestChromosome();

		if (outputData == true)
		{
			GeneticAlgorithmSettingGameObject.SaveData(numGenerationGameObject);
			GeneticAlgorithmSettingGameObject.OutputData(spaceID, gameObjectID);
		}
		
		// Render the tiles.
		TacticRenderHandlar.CleanBoard(AutoTacticRender);
		TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromosome);
		isClickOnlySpace = false;
		runGenerateGameObject++;
	}

	bool isClickOnlySpace = false;
	public void OnClick_GenerateOnlySpace()
	{
		if (isClickOnlySpace == false)
		{
			// Render the tiles.
			TacticRenderHandlar.CleanBoard(AutoTacticRender);
			TacticRenderHandlar.RenderTileOfTactic(length + 2, width + 2, AutoTacticRender, transformChromosome(BestChromosome, length, width));
			isClickOnlySpace = true;
		}
		else
		{
			// Render the tiles.
			TacticRenderHandlar.CleanBoard(AutoTacticRender);
			TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromosome);
			isClickOnlySpace = false;
		}		
	}

	public void OnClick_OutputAutoTacticData()
	{
		StartCoroutine(DataSerialization.OutputAutoTacticData(length + 2, width + 2, transformChromosome(BestChromosome, length, width), gameObjectID, Tactic_Bait, Tactic_Ambush, Tactic_TwoProngedAttack, Tactic_Defense, Tactic_Clash));
	}

	Chromosome transformChromosome(Chromosome originalChromosome, int originalLength, int originalWidth)
	{
		int resultLength = originalLength + 2;
		int resultWidth = originalWidth + 2;

		Chromosome resultChromosome = new Chromosome();

		for (int gene = 0; gene < ( resultLength * resultWidth ); gene++)
		{
			resultChromosome.genesList.Add(new Gene());
			resultChromosome.genesList[gene].type = GeneType.Forbidden;
		}

		for (int originalGene = 0; originalGene < originalChromosome.genesList.Count; originalGene++)
		{
			int original_posX = originalGene / length;
			int original_posY = originalGene % length;
			int resul_posX = original_posX + 1;
			int resul_posY = original_posY + 1;

			resultChromosome.genesList[resul_posX * resultLength + resul_posY].type = originalChromosome.genesList[originalGene].type;
			if (originalChromosome.genesList[originalGene].GameObjectAttribute == GeneGameObjectAttribute.entrance 
				|| originalChromosome.genesList[originalGene].GameObjectAttribute == GeneGameObjectAttribute.exit)
			{
				resultChromosome.genesList[transformPositionDoor(originalChromosome, originalLength, originalWidth, originalGene)].type = GeneType.Empty;
				resultChromosome.genesList[transformPositionDoor(originalChromosome, originalLength, originalWidth, originalGene)].GameObjectAttribute = originalChromosome.genesList[originalGene].GameObjectAttribute;
			}
			else
			{
				resultChromosome.genesList[resul_posX * resultLength + resul_posY].GameObjectAttribute = originalChromosome.genesList[originalGene].GameObjectAttribute;
			}			
			resultChromosome.genesList[resul_posX * resultLength + resul_posY].SpaceAttribute = originalChromosome.genesList[originalGene].SpaceAttribute;
		}

		return resultChromosome;
	}

	int transformPositionDoor(Chromosome originalChromosome, int originalLength, int originalWidth, int oldPosition)
	{
		int oldPosition_x = oldPosition / originalLength;
		int oldPosition_y = oldPosition % originalLength;
		int newPosition_x = 0;
		int newPosition_y = 0;
		int newPosition;

		#region Top
		if (oldPosition_x == 0)
		{
			// TopLeft
			if (oldPosition_y == 0)
			{
				if (originalChromosome.genesList[oldPosition + 1].isMainPath == true)
				{
					newPosition_x = 1;
					newPosition_y = 0;
				}
				else
				{
					newPosition_x = 0;
					newPosition_y = 1;
				}
			}
			// TopRight
			else if (oldPosition_y == ( originalLength - 1 ))
			{
				if (originalChromosome.genesList[oldPosition - 1].isMainPath == true)
				{
					newPosition_x = 1;
					newPosition_y = oldPosition_y + 2;
				}
				else
				{
					newPosition_x = 0;
					newPosition_y = oldPosition_y + 1;
				}
			}
			else
			{
				newPosition_x = 0;
				newPosition_y = oldPosition_y + 1;
			}
		}
		#endregion

		#region Bottom
		if (oldPosition_x == originalWidth - 1)
		{
			// BottomLeft
			if (oldPosition_y == 0)
			{
				if (originalChromosome.genesList[oldPosition + 1].isMainPath == true)
				{
					newPosition_x = oldPosition_x + 1;
					newPosition_y = 0;
				}
				else
				{
					newPosition_x = oldPosition_x + 2;
					newPosition_y = 1;
				}
			}
			// BottomRight
			else if (oldPosition_y == ( originalLength - 1 ))
			{
				if (originalChromosome.genesList[oldPosition - 1].isMainPath == true)
				{
					newPosition_x = oldPosition_x + 1;
					newPosition_y = oldPosition_y + 2;
				}
				else
				{
					newPosition_x = oldPosition_x + 2;
					newPosition_y = oldPosition_y + 1;
				}
			}
			else
			{
				newPosition_x = oldPosition_x + 2;
				newPosition_y = oldPosition_y + 1;
			}
		}
		#endregion

		#region Left
		if (oldPosition_y == 0 && oldPosition_x != 0 && oldPosition_x != originalWidth - 1)
		{
			newPosition_x = oldPosition_x + 1;
			newPosition_y = 0;
		}
		#endregion

		#region Right
		if (oldPosition_y == originalLength - 1 && oldPosition_x != 0 && oldPosition_x != originalWidth - 1)
		{
			newPosition_x = oldPosition_x + 1;
			newPosition_y = oldPosition_y + 2;
		}
		#endregion

		newPosition = newPosition_x * ( originalLength + 2 ) + newPosition_y;

		return newPosition;
	}

	void GetParameters(bool isBait, bool isAmbush, bool isPronged, bool isDefense, bool isClash)
	{
		length = ParameterSetting.GetComponent<Parameters>().GetTheLenghOfTile();
		width = ParameterSetting.GetComponent<Parameters>().GetTheWidthOfTile();
		minEnemy = ParameterSetting.GetComponent<Parameters>().GetMinEnemy();
		minTrap = ParameterSetting.GetComponent<Parameters>().GetMinTrap();
		minTreasure = ParameterSetting.GetComponent<Parameters>().GetMinTreasure();
		maxEnemy = ParameterSetting.GetComponent<Parameters>().GetMaxEnemy();
		maxTrap = ParameterSetting.GetComponent<Parameters>().GetMaxTrap();
		maxTreasure = ParameterSetting.GetComponent<Parameters>().GetMaxTreasure();

		fitness_Rectangle = ParameterSetting.GetComponent<Parameters>().GetIsFitness_Rectangle();
		fitness_Corridor = ParameterSetting.GetComponent<Parameters>().GetIsFitness_Corridor();
		fitness_Defense = ParameterSetting.GetComponent<Parameters>().GetIsFitness_Defense();
		fitness_OnMainPath = ParameterSetting.GetComponent<Parameters>().GetIsFitness_OnMainPath();
		fitness_BesideMainPath = ParameterSetting.GetComponent<Parameters>().GetIsFitness_BesideMainPath();
		fitness_TwoPronged = ParameterSetting.GetComponent<Parameters>().GetIsFitness_TwoPronged();

		isTreasureOnMainPath = ParameterSetting.GetComponent<Parameters>().GetIsTreasureOnMainPath();
		isTreasureBesideMainPath = ParameterSetting.GetComponent<Parameters>().GetIsTreasureBesideMainPath();

		weight_RectangleQuality = ParameterSetting.GetComponent<Parameters>().GetTheWeight_RectangleQuality();
		weight_CorridorQuality = ParameterSetting.GetComponent<Parameters>().GetTheWeight_CorridorQuality();
		weight_Fitness_Defense = ParameterSetting.GetComponent<Parameters>().GetTheWeight_Fitness_Defense();
		weight_Fitness_OnMainPath = ParameterSetting.GetComponent<Parameters>().GetTheWeight_Fitness_OnMainPath();
		weight_Fitness_BesideMainPath = ParameterSetting.GetComponent<Parameters>().GetTheWeight_Fitness_BesideMainPath();
		weight_Fitness_TwoPronged = ParameterSetting.GetComponent<Parameters>().GetTheWeight_Fitness_TwoPronged();

		Tactic_Bait = ParameterSetting.GetComponent<Parameters>().GetIsTactic_Bait();
		Tactic_Ambush = ParameterSetting.GetComponent<Parameters>().GetIsTactic_Ambush();
		Tactic_TwoProngedAttack = ParameterSetting.GetComponent<Parameters>().GetIsTactic_TwoProngedAttack();
		Tactic_Defense = ParameterSetting.GetComponent<Parameters>().GetIsTactic_Defense();
		Tactic_Clash = ParameterSetting.GetComponent<Parameters>().GetIsTactic_Clash();

		weight_Tactic_Bait = ParameterSetting.GetComponent<Parameters>().GetTheWeight_Tactic_Bait();
		weight_Tactic_Ambush = ParameterSetting.GetComponent<Parameters>().GetTheWeight_Tactic_Ambush();
		weight_Tactic_TwoProngedAttack = ParameterSetting.GetComponent<Parameters>().GetTheWeight_Tactic_TwoProngedAttack();
		weight_Tactic_Defense = ParameterSetting.GetComponent<Parameters>().GetTheWeight_Tactic_Defense();
		weight_Tactic_Clash = ParameterSetting.GetComponent<Parameters>().GetTheWeight_Tactic_Clash();

		// This setting is from the experiment
		if (isBait == true || isAmbush == true
			|| isPronged == true || isDefense == true || isClash == true)
		{
			Tactic_Bait = isBait;
			Tactic_Ambush = isAmbush;
			Tactic_TwoProngedAttack = isPronged;
			Tactic_Defense = isDefense;
			Tactic_Clash = isClash;
			getTacticParameters(Tactic_Bait, Tactic_Ambush, Tactic_TwoProngedAttack, Tactic_Defense, Tactic_Clash);
		}
		else
		{
			// This setting is from the parameter
			if (Tactic_Bait == true || Tactic_Ambush == true
			|| Tactic_TwoProngedAttack == true || Tactic_Defense == true || Tactic_Clash == true)
			{
				getTacticParameters(Tactic_Bait, Tactic_Ambush, Tactic_TwoProngedAttack, Tactic_Defense, Tactic_Clash);
			}
		}
		
		numMinGameObject = new int[5] { 1, 1, minEnemy, minTrap, minTreasure };
		numMaxGameObject = new int[5] { 1, 1, maxEnemy, maxTrap, maxTreasure };
	}

	// Parameters of tactic
	bool Tactic_Fitness_Rectangle;
	bool Tactic_Fitness_Corridor;

	bool Tactic_Clash_fitness_OnMainPath;
	bool Tactic_Clash_fitness_BesideMainPath;

	void getTacticParameters(bool isTactic_Bait, bool isTactic_Ambush, bool isTactic_TwoProngedAttack, bool isTactic_Defense, bool isTactic_Clash)
	{
		// Random the fitness_Rectangle & fitness_Corridor.
		Tactic_Fitness_Rectangle = ( UnityEngine.Random.Range(0, 2) == 1 ) ? true : false;
		Tactic_Fitness_Corridor = ( UnityEngine.Random.Range(0, 2) == 1 ) ? true : false;

		minEnemy = -1;
		maxEnemy = -1;
		minTrap = -1;
		maxTrap = -1;
		minTreasure = -1;
		maxTreasure = -1;

		if (isTactic_Ambush == true)
		{
			minEnemy = ( minEnemy == -1 || minEnemy > 1 ) ? 2 : minEnemy;
			maxEnemy = ( maxEnemy == -1 || maxEnemy < 4 ) ? 4 : maxEnemy;
			minTrap = ( minTrap == -1 || minTrap > 0 ) ? 0 : minTrap;
			maxTrap = ( maxTrap == -1 || maxTrap < 1 ) ? 1 : maxTrap;
			minTreasure = ( minTreasure == -1 || minTreasure > 0 ) ? 0 : minTreasure;
			maxTreasure = ( maxTreasure == -1 || maxTreasure < 0 ) ? 0 : maxTreasure;
		}
		if (isTactic_Bait == true)
		{
			minEnemy = ( minEnemy == -1 || minEnemy > 1 ) ? 1 : minEnemy;
			maxEnemy = ( maxEnemy == -1 || maxEnemy < 4 ) ? 4 : maxEnemy;
			minTrap = ( minTrap == -1 || minTrap > 0 ) ? 0 : minTrap;
			maxTrap = ( maxTrap == -1 || maxTrap < 1 ) ? 1 : maxTrap;
			minTreasure = ( minTreasure == -1 || minTreasure > 1 ) ? 1 : minTreasure;
			maxTreasure = ( maxTreasure == -1 || maxTreasure < 1 ) ? 1 : maxTreasure;
			Tactic_Fitness_Rectangle = true;
		}
		if (isTactic_TwoProngedAttack == true)
		{
			minEnemy = ( minEnemy == -1 || minEnemy > 1 ) ? 1 : minEnemy;
			maxEnemy = ( maxEnemy == -1 || maxEnemy < 4 ) ? 4 : maxEnemy;
			minTrap = ( minTrap == -1 || minTrap > 0 ) ? 0 : minTrap;
			maxTrap = ( maxTrap == -1 || maxTrap < 1 ) ? 1 : maxTrap;
			minTreasure = ( minTreasure == -1 || minTreasure > 0 ) ? 0 : minTreasure;
			maxTreasure = ( maxTreasure == -1 || maxTreasure < 1 ) ? 1 : maxTreasure;
			Tactic_Fitness_Rectangle = true;
		}
		if (isTactic_Defense == true)
		{
			minEnemy = ( minEnemy == -1 || minEnemy > 1 ) ? 1 : minEnemy;
			maxEnemy = ( maxEnemy == -1 || maxEnemy < 4 ) ? 4 : maxEnemy;
			minTrap = ( minTrap == -1 || minTrap > 0 ) ? 0 : minTrap;
			maxTrap = ( maxTrap == -1 || maxTrap < 1 ) ? 1 : maxTrap;
			minTreasure = ( minTreasure == -1 || minTreasure > 1 ) ? 1 : minTreasure;
			maxTreasure = ( maxTreasure == -1 || maxTreasure < 1 ) ? 1 : maxTreasure;
			Tactic_Fitness_Rectangle = true;
		}
		if (isTactic_Clash == true)
		{
			minEnemy = ( minEnemy == -1 || minEnemy > 1 ) ? 1 : minEnemy;
			maxEnemy = ( maxEnemy == -1 || maxEnemy < 4 ) ? 4 : maxEnemy;
			minTrap = ( minTrap == -1 || minTrap > 0 ) ? 0 : minTrap;
			maxTrap = ( maxTrap == -1 || maxTrap < 1 ) ? 1 : maxTrap;
			minTreasure = ( minTreasure == -1 || minTreasure > 0 ) ? 0 : minTreasure;
			maxTreasure = ( maxTreasure == -1 || maxTreasure < 1 ) ? 1 : maxTreasure;
			Tactic_Fitness_Rectangle = ( isTactic_Bait == true || isTactic_Ambush == true
										|| isTactic_Ambush == true || isTactic_Ambush == true ) ? Tactic_Fitness_Rectangle : false;
			Tactic_Fitness_Corridor = true;

			Tactic_Clash_fitness_OnMainPath = ( UnityEngine.Random.Range(0, 2) == 1 ) ? true : false;
			Tactic_Clash_fitness_BesideMainPath = ( UnityEngine.Random.Range(0, 2) == 1 ) ? true : false;
		}
	}

}
