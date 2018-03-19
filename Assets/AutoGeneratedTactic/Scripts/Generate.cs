using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GeneticAlgorithmSettingDefinition;
using ChromosomeDefinition;
using GeneticAlgorithmSettingGameObjectDefinition;
using ParticleSwarmOptimizationSettingDefinition;

public class Generate : MonoBehaviour {

	//public GameObject Tile_Empty;

	private GameObject ParameterSetting;
	private GameObject AutoTacticRender;
	private AutoTacticRenderHandler TacticRenderHandlar;
	private GameObject GeneticAlgorithmSetting;
	private GeneticAlgorithmSetting GeneticAlgorithm;
	private GeneticAlgorithmSettingGameObject GeneticAlgorithmSettingGameObject;
	private ParticleSwarmOptimizationSetting ParticleSwarmOptimization;
	private int runGenerate = 0;
	private Chromosome BestChromesome_Space = new Chromosome();
	private Chromosome BestChromesome = new Chromosome();

	// Use this for initialization
	void Start () {
		ParameterSetting = GameObject.Find("ParameterSetting");
		AutoTacticRender = GameObject.Find("AutoTacticRender");
		TacticRenderHandlar = GameObject.Find("AutoTacticRender").GetComponent<AutoTacticRenderHandler>();
		GeneticAlgorithm = GameObject.Find("GeneticAlgorithmSetting").GetComponent<GeneticAlgorithmSetting>();
		GeneticAlgorithmSettingGameObject = GameObject.Find("GeneticAlgorithmSettingGameObject").GetComponent<GeneticAlgorithmSettingGameObject>();
		ParticleSwarmOptimization = GameObject.Find("ParticleSwarmOptimizationSetting").GetComponent<ParticleSwarmOptimizationSetting>();
	}

	int useMethod = 0;// 0:GeneticAlgorithm, 1:ParticleSwarmOptimization
	int length;
	int width;
	int numGeneration = 100;
	int numChromosome = 100;
	float rato_crossover = 0.8f;
	float rato_mutation = 1.0f;
	int numGenerationGameObject = 2;
	int numChromosomeGameObject = 10;
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

				BestChromesome_Space = GeneticAlgorithm.BestChromesome().CloneSpace();
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

				BestChromesome = ParticleSwarmOptimization.BestChromesome();
				ParticleSwarmOptimization.SaveData(numGeneration);
				ParticleSwarmOptimization.OutputData(runGenerate);

				//ParticleSwarmOptimization.DebugTest();
				//Time
				//var PSOendTime = Time.realtimeSinceStartup - startTime;
				//Debug.Log(length + " x " + width + "ParticleSwarmOptimization_Time = " + PSOendTime);
				break;
		}
		#endregion

		GeneticAlgorithmSettingGameObject.InitialPopulation(length, width, length * width, numChromosomeGameObject, numGenerationGameObject, BestChromesome_Space);
		for (int num_generation = 0; num_generation < numGenerationGameObject; num_generation++)
		{
			GeneticAlgorithmSettingGameObject.CalculateFitnessScores();
			GeneticAlgorithmSettingGameObject.SaveData(num_generation);
			GeneticAlgorithmSettingGameObject.Selection();
			GeneticAlgorithmSettingGameObject.Crossover(ratio_GameObjectCrossover);
			GeneticAlgorithmSettingGameObject.Mutation(ratio_GameObjectMutation);
			GeneticAlgorithmSettingGameObject.Replace();
		}
		BestChromesome = GeneticAlgorithmSettingGameObject.BestChromesome();
		GeneticAlgorithmSettingGameObject.SaveData(numGenerationGameObject);
		GeneticAlgorithmSettingGameObject.OutputData(0);

		// Render the tiles.
		TacticRenderHandlar.CleanBoard(AutoTacticRender);
		TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromesome);

		runGenerate++;
	}

	int runGenerateGameObject = 1;
	public void OnClick_GenerateGameObject()
	{
		GeneticAlgorithmSettingGameObject.InitialPopulation(length, width, length * width, numChromosomeGameObject, numGenerationGameObject, BestChromesome_Space);
		for (int num_generation = 0; num_generation < numGenerationGameObject; num_generation++)
		{
			GeneticAlgorithmSettingGameObject.CalculateFitnessScores();
			GeneticAlgorithmSettingGameObject.SaveData(num_generation);
			GeneticAlgorithmSettingGameObject.Selection();
			GeneticAlgorithmSettingGameObject.Crossover(ratio_GameObjectCrossover);
			GeneticAlgorithmSettingGameObject.Mutation(ratio_GameObjectMutation);
			GeneticAlgorithmSettingGameObject.Replace();
		}
		BestChromesome = GeneticAlgorithmSettingGameObject.BestChromesome();
		GeneticAlgorithmSettingGameObject.SaveData(numGenerationGameObject);
		GeneticAlgorithmSettingGameObject.OutputData(runGenerateGameObject);

		// Render the tiles.
		TacticRenderHandlar.CleanBoard(AutoTacticRender);
		TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromesome);

		runGenerateGameObject++;
	}

	public void OnClick_GenerateOnlySpace()
	{
		// Render the tiles.
		TacticRenderHandlar.CleanBoard(AutoTacticRender);
		TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromesome_Space);
	}
}
