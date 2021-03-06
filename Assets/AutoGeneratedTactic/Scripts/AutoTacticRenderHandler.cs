﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChromosomeDefinition;

public class AutoTacticRenderHandler : MonoBehaviour {

	private GameObject TileStyle;
	public GameObject Tile_Empty;
	public GameObject Tile_Forbidden;
	public GameObject Tile_Rectangle;
	public GameObject Tile_Corridor;

	private GameObject GameObjectStyle;
	public GameObject Entrance;
	public GameObject Exit;
	public GameObject Enemy;
	public GameObject Trap;
	public GameObject Treasure;

	public GameObject MainPath;

	public void CleanBoard(GameObject board)
	{
		foreach (Transform child in board.transform)
		{
			GameObject.Destroy(child.gameObject);
		}
	}

	public void RenderTileOfTactic(int tileLength, int tileWidth, GameObject render, Chromosome bestChromosome)
	{
		int indexGene = 0;

		// Get the size of tile
		RectTransform rt = (RectTransform)Tile_Empty.transform;
		float tile_size = rt.rect.width;

		float positionOfLastTile_x = ( tileLength - 1 ) * tile_size;
		float positionOfLastTile_y = ( tileWidth - 1 ) * tile_size;
		// Offset to center	
		float offset_x = positionOfLastTile_x / 2;
		float offset_y = positionOfLastTile_y / 2;

		// Render the tiles
		for (int y = 0; y < tileWidth; y++)
		{
			for (int x = 0; x < tileLength; x++)
			{
				switch ((int)bestChromosome.genesList[indexGene].type)
				{
					case 0:
						TileStyle = Tile_Forbidden;
						break;
					case 1:
						TileStyle = Tile_Empty;
						break;
				}
				if (bestChromosome.genesList[indexGene].SpaceAttribute == GeneSpaceAttribute.Rectangle)
				{
					TileStyle = Tile_Rectangle;
				}
				else if (bestChromosome.genesList[indexGene].SpaceAttribute == GeneSpaceAttribute.Corridor)
				{
					TileStyle = Tile_Corridor;
				}

				switch ((int)bestChromosome.genesList[indexGene].GameObjectAttribute)
				{
					case 1:
						GameObjectStyle = Entrance;
						break;
					case 2:
						GameObjectStyle = Exit;
						break;
					case 3:
						GameObjectStyle = Enemy;
						break;
					case 4:
						GameObjectStyle = Trap;
						break;
					case 5:
						GameObjectStyle = Treasure;
						break;
				}				

				var newTile = Instantiate(TileStyle);
				newTile.name = "Tile(" + Convert.ToString(y) +"," + Convert.ToString(x) + ")";
				newTile.transform.parent = render.transform;
				newTile.GetComponent<RectTransform>().localPosition = new Vector3(x * tile_size - offset_x, -(y * tile_size - offset_y), 0);
				newTile.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

				if (bestChromosome.genesList[indexGene].GameObjectAttribute != GeneGameObjectAttribute.None)
				{
					var gameObject = Instantiate(GameObjectStyle);
					gameObject.name = bestChromosome.genesList[indexGene].GameObjectAttribute.ToString();
					gameObject.transform.parent = newTile.transform;
					gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
					gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.8f, 0.8f, 1);
				}

				if (bestChromosome.genesList[indexGene].isMainPath == true)
				{
					var gameObject = Instantiate(MainPath);
					gameObject.name = "MainPath";
					gameObject.transform.parent = newTile.transform;
					gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
					gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
				}

				indexGene++;
			}
		}
	}
}
