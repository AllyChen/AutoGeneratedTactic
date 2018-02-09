﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GeneticAlgorithmSettingDefinition;
using ChromosomeDefinition;

using ParticleSwarmOptimizationSettingDefinition;

public class Generate : MonoBehaviour {

	//public GameObject Tile_Empty;

	private GameObject ParameterSetting;
	private GameObject AutoTacticRender;
	private AutoTacticRenderHandler TacticRenderHandlar;
	private GameObject GeneticAlgorithmSetting;
	private GeneticAlgorithmSetting GeneticAlgorithm;
	private ParticleSwarmOptimizationSetting ParticleSwarmOptimization;
	private int runGenerate = 0;
	private Chromosome BestChromesome = new Chromosome();

	// Use this for initialization
	void Start () {
		ParameterSetting = GameObject.Find("ParameterSetting");
		AutoTacticRender = GameObject.Find("AutoTacticRender");
		TacticRenderHandlar = GameObject.Find("AutoTacticRender").GetComponent<AutoTacticRenderHandler>();
		GeneticAlgorithm = GameObject.Find("GeneticAlgorithmSetting").GetComponent<GeneticAlgorithmSetting>();
		ParticleSwarmOptimization = GameObject.Find("ParticleSwarmOptimizationSetting").GetComponent<ParticleSwarmOptimizationSetting>();
	}


	int useMethod = 0;// 0:GeneticAlgorithm, 1:ParticleSwarmOptimization

	public void OnClick_Generate()
	{
		int length;
		int width;
		length = ParameterSetting.GetComponent<Parameters>().GetTheLenghOfTile();
		width = ParameterSetting.GetComponent<Parameters>().GetTheWidthOfTile();
		int numGeneration = 10;
		int numChromosome = 10;

		switch (useMethod)
		{
			case 0:
				// Start GeneticAlgorithm
				float rato_crossover = 0.5f;
				float rato_mutation = 0.9f;

				GeneticAlgorithm.InitialPopulation(length, width, length * width, numChromosome, numGeneration);

				for (int num_generation = 0; num_generation < numGeneration; num_generation++)
				{
					GeneticAlgorithm.CalculateFitnessScores();
					GeneticAlgorithm.SaveData(num_generation);
					GeneticAlgorithm.Selection();
					GeneticAlgorithm.Crossover(rato_crossover);
					GeneticAlgorithm.Mutation(rato_mutation);
					GeneticAlgorithm.Replace();
				}

				BestChromesome = GeneticAlgorithm.BestChromesome();
				GeneticAlgorithm.SaveData(numGeneration);
				GeneticAlgorithm.OutputData(runGenerate);

				GeneticAlgorithm.DebugTest();

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

				ParticleSwarmOptimization.DebugTest();

				break;
		}

		// Render the tiles.
		TacticRenderHandlar.CleanBoard(AutoTacticRender);
		TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromesome);

		runGenerate++;
	}
}
