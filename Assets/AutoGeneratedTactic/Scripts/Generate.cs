using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GeneticAlgorithmSettingDefinition;
using ChromosomeDefinition;

public class Generate : MonoBehaviour {

	//public GameObject Tile_Empty;

	private GameObject ParameterSetting;
	private GameObject AutoTacticRender;
	private AutoTacticRenderHandler TacticRenderHandlar;
	private GameObject GeneticAlgorithmSetting;
	private GeneticAlgorithmSetting GeneticAlgorithm;

	private Chromosome BestChromesome = new Chromosome();

	// Use this for initialization
	void Start () {
		ParameterSetting = GameObject.Find("ParameterSetting");
		AutoTacticRender = GameObject.Find("AutoTacticRender");
		TacticRenderHandlar = GameObject.Find("AutoTacticRender").GetComponent<AutoTacticRenderHandler>();
		GeneticAlgorithm = GameObject.Find("GeneticAlgorithmSetting").GetComponent<GeneticAlgorithmSetting>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClick_Generate()
	{
		int length;
		int width;
		length = ParameterSetting.GetComponent<Parameters>().GetTheLenghOfTile();
		width = ParameterSetting.GetComponent<Parameters>().GetTheWidthOfTile();

		// Start GeneticAlgorithm
		float rato_crossover = 0.5f;
		float rato_mutation = 0.9f;

		GeneticAlgorithm.InitialPopulation(length, width, length * width, 100);

		for (int num_generation = 0; num_generation < 100; num_generation++)
		{		
			GeneticAlgorithm.CalculateFitnessScores();
			GeneticAlgorithm.Selection();
			GeneticAlgorithm.Crossover(rato_crossover);
			GeneticAlgorithm.Mutation(rato_mutation);
			GeneticAlgorithm.Replace();
		}

		BestChromesome = GeneticAlgorithm.BestChromesome();
		GeneticAlgorithm.DebugTest();

		// Render the tiles.
		TacticRenderHandlar.CleanBoard(AutoTacticRender);
		TacticRenderHandlar.RenderTileOfTactic(length, width, AutoTacticRender, BestChromesome);
	}
}
