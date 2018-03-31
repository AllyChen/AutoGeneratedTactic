﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	int length;
	int width;
	int numGeneration = 50;
	int numChromosome = 100;
	float rato_crossover = 0.8f;
	float rato_mutation = 1.0f;
	int numGenerationGameObject = 50;
	int numChromosomeGameObject = 100;
	float ratio_GameObjectCrossover = 0.8f;
	float ratio_GameObjectMutation = 1.0f;

	public void OnClick_Generate()
	{
		var startTime = Time.realtimeSinceStartup;
		length = ParameterSetting.GetComponent<Parameters>().GetTheLenghOfTile();
		width = ParameterSetting.GetComponent<Parameters>().GetTheWidthOfTile();

		#region GenerateSpace
		switch (useMethod)
		{
			case 0:
				// Start GeneticAlgorithm
				GeneticAlgorithm.InitialPopulation(length, width, length * width, numChromosome, numGeneration);

				for (int num_generation = 0; num_generation < numGeneration; num_generation++)
				{
					GeneticAlgorithm.CalculateFitnessScores();
					//GeneticAlgorithm.SaveData(num_generation);
					GeneticAlgorithm.Selection();
					GeneticAlgorithm.Crossover(rato_crossover);
					GeneticAlgorithm.Mutation(rato_mutation);
					GeneticAlgorithm.Replace();
				}

				BestChromosome_Space = GeneticAlgorithm.BestChromosome().CloneSpace();
				//GeneticAlgorithm.SaveData(numGeneration);
				//GeneticAlgorithm.OutputData(runGenerate);

				//GeneticAlgorithm.DebugTest();
				//Time
				var GAendTime = Time.realtimeSinceStartup - startTime;
				Debug.Log(length+" x "+width + "GeneticAlgorithm_Time = "+ GAendTime);
				break;
			case 1:
				// Start ParticleSwarmOptimization
				ParticleSwarmOptimization.InitialPopulation(length, width, length * width, numChromosome, numGeneration);

				for (int num_generation = 0; num_generation < numGeneration; num_generation++)
				{
					ParticleSwarmOptimization.CalculateFitnessScores();
					ParticleSwarmOptimization.SaveData(num_generation);
					ParticleSwarmOptimization.UpdateVelocities();
					ParticleSwarmOptimization.UpdatePosition();
				}

				BestChromosome = ParticleSwarmOptimization.BestChromosome();
				ParticleSwarmOptimization.SaveData(numGeneration);
				ParticleSwarmOptimization.OutputData(runGenerate);

				//ParticleSwarmOptimization.DebugTest();
				//Time
				//var PSOendTime = Time.realtimeSinceStartup - startTime;
				//Debug.Log(length + " x " + width + "ParticleSwarmOptimization_Time = " + PSOendTime);
				break;
		}
		#endregion

		GeneticAlgorithmSettingGameObject.InitialPopulation(length, width, length * width, numChromosomeGameObject, numGenerationGameObject, BestChromosome_Space);
		for (int num_generation = 0; num_generation < numGenerationGameObject; num_generation++)
		{
			GeneticAlgorithmSettingGameObject.CalculateFitnessScores();
			//GeneticAlgorithmSettingGameObject.SaveData(num_generation);
			GeneticAlgorithmSettingGameObject.Selection();
			GeneticAlgorithmSettingGameObject.Crossover(ratio_GameObjectCrossover);
			GeneticAlgorithmSettingGameObject.Mutation(ratio_GameObjectMutation);
			GeneticAlgorithmSettingGameObject.Replace();
		}
		BestChromosome = GeneticAlgorithmSettingGameObject.BestChromosome();

		//Time
		var GAGOendTime = Time.realtimeSinceStartup - startTime;
		Debug.Log(length + " x " + width + "GeneticAlgorithmGameObject_Time = " + GAGOendTime);

		//GeneticAlgorithmSettingGameObject.SaveData(numGenerationGameObject);
		//GeneticAlgorithmSettingGameObject.OutputData(0);

		// Render the tiles.
		TacticRenderHandlar.CleanBoard(AutoTacticRender);
		TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromosome);

		runGenerate++;
	}

	int runGenerateGameObject = 1;
	public void OnClick_GenerateGameObject()
	{
		GeneticAlgorithmSettingGameObject.InitialPopulation(length, width, length * width, numChromosomeGameObject, numGenerationGameObject, BestChromosome_Space);
		for (int num_generation = 0; num_generation < numGenerationGameObject; num_generation++)
		{
			GeneticAlgorithmSettingGameObject.CalculateFitnessScores();
			//GeneticAlgorithmSettingGameObject.SaveData(num_generation);
			GeneticAlgorithmSettingGameObject.Selection();
			GeneticAlgorithmSettingGameObject.Crossover(ratio_GameObjectCrossover);
			GeneticAlgorithmSettingGameObject.Mutation(ratio_GameObjectMutation);
			GeneticAlgorithmSettingGameObject.Replace();
		}
		BestChromosome = GeneticAlgorithmSettingGameObject.BestChromosome();
		//GeneticAlgorithmSettingGameObject.SaveData(numGenerationGameObject);
		//GeneticAlgorithmSettingGameObject.OutputData(runGenerateGameObject);

		// Render the tiles.
		TacticRenderHandlar.CleanBoard(AutoTacticRender);
		TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromosome);

		runGenerateGameObject++;
	}

	public void OnClick_GenerateOnlySpace()
	{
		// Render the tiles.
		TacticRenderHandlar.CleanBoard(AutoTacticRender);
		//TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromosome_Space);
		TacticRenderHandlar.RenderTileOfTactic(length + 2, width + 2, AutoTacticRender, transformChromosome(BestChromosome, length, width));
	}

	public void OnClick_OutputAutoTacticData()
	{
		StartCoroutine(DataSerialization.OutputAutoTacticData(length + 2, width + 2, transformChromosome(BestChromosome, length, width)));
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
}
