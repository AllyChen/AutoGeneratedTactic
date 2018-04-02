using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ChromosomeDefinition;
using System.Xml.Linq;
using System.Reflection;
using System;

namespace DataSerializationDefinition
{
	public class DataSerialization : MonoBehaviour
	{
		private string path = String.Empty;
		private const float MODEL_SIZE = 3f;
		private const float MODEL_HEIGHT = 8f;
		// Assume only ground floor was generated
		private int floorNum = 0;
		private string tileSpriteName = "Dungeon_Marble";
		private string tilePrefabName = "Dungeon_Marble";

		private void Awake()
		{
			path = Application.dataPath + @"/OutputTactics/";
		}

		public IEnumerator OutputAutoTacticData(int tacticLength, int tacticWidth, Chromosome outputChromosome)
		{
			Debug.Log("Output Folder: " + path);
			var startTime = Time.realtimeSinceStartup;
			// Prepare objects for output usage
			GameObject go = new GameObject();
			var tcl = go.AddComponent<ToolComponentList>();
			var tod = go.AddComponent<TacticObjectData>();
			tod.tScale = Vector3.one;
			tod.SetUp();
			Sprite sprite = Sprite.Create(new Texture2D(0, 0), Rect.zero, Vector2.zero);
			tod.tObjSprite = sprite;
			// Create a new XML file
			XDocument xdoc = new XDocument();
			xdoc.Add(new XElement("TacticPattern"));

			XElement tileFloors = new XElement("TileFloors");
			tileFloors.Add(new XElement("Width", tacticLength));
			tileFloors.Add(new XElement("Height", tacticWidth));

			XElement tileFloor = new XElement("Floor", new XAttribute("id", floorNum));

			var genesList = outputChromosome.genesList;
			for (int i = 0; i < genesList.Count; i++)
			{
				// Output Tile information
				var pos = GetGenePosition(i, tacticLength);
				XElement tile = new XElement("Tile", new XAttribute("x", pos.x), new XAttribute("y", pos.z));
				tile.Add(new XElement("TileSpriteName", tileSpriteName));
				tile.Add(new XElement("TilePrefabName", tilePrefabName));
				// Setup ToolComponentList
				ResetTCL(tcl);
				// Setup TacticObjectData
				tod.tName = tileSpriteName;
				tod.tPosition = new Vector3(pos.x * MODEL_SIZE / 10, pos.y * MODEL_HEIGHT / 10, -pos.z * MODEL_SIZE / 10);
				tod.tRotation = Vector3.zero;
				tod.tObjSprite.name = tileSpriteName;

				OutputScripts(go, tile);
				// Output Object information
				XElement @object = new XElement("Object");
				if (genesList[i].type == GeneType.Forbidden)
				{
					@object.Add(new XElement("ObjPrefabName", "Dungeon_Wall"));
					tcl.rotation = true;
					tcl.scale = true;
					tcl.anchor = true;
					tod.tName = "Dungeon_Wall";
					tod.tObjSprite.name = "Dungeon_Wall";
					OutputScripts(go, @object);
					tile.Add(@object);
				}
				else
				{
					switch (genesList[i].GameObjectAttribute)
					{
						case GeneGameObjectAttribute.None:
						case GeneGameObjectAttribute.NumberOfGeneSpaceAttribute:
							break;
						case GeneGameObjectAttribute.entrance:
						case GeneGameObjectAttribute.exit:
							@object.Add(new XElement("ObjPrefabName", "Dungeon_BigDoor"));
							var toolDoor = go.GetComponent<Tool_Door>() ?? go.AddComponent<Tool_Door>();
							tcl.rotation = true;
							tcl.scale = true;
							tcl.anchor = true;
							tcl.actTrigger = true;
							tod.tName = "Dungeon_BigDoor";
							tod.tObjSprite.name = "Dungeon_BigDoor";
							toolDoor.door = new GameObject("SM_Bld_Castle_Iron_Gate_02");
							toolDoor.status = 1;
							if (pos.x == 0 || pos.x == tacticLength - 1)
							{
								tod.tRotation = new Vector3(0f, 90f, 0f);
							}
							OutputScripts(go, @object);
							tile.Add(@object);
							break;
						case GeneGameObjectAttribute.enemy:
							@object.Add(new XElement("ObjPrefabName", "Monster"));
							tcl.rotation = true;
							tcl.scale = true;
							tcl.unique = true;
							tcl.anchor = true;
							tod.tName = "Monster";
							tod.tObjSprite.name = "Monster1";
							OutputScripts(go, @object);
							tile.Add(@object);
							break;
						case GeneGameObjectAttribute.trap:
							@object.Add(new XElement("ObjPrefabName", "Trap"));
							tcl.rotation = true;
							tcl.scale = true;
							tcl.unique = true;
							tcl.anchor = true;
							tod.tName = "Trap";
							tod.tObjSprite.name = "Trap";
							OutputScripts(go, @object);
							tile.Add(@object);
							break;
						case GeneGameObjectAttribute.treasure:
							@object.Add(new XElement("ObjPrefabName", "Treasure_Chest"));
							tcl.rotation = true;
							tcl.scale = true;
							tcl.unique = true;
							tcl.anchor = true;
							tod.tName = "Treasure_Chest";
							tod.tObjSprite.name = "Treasure_Chest";
							OutputScripts(go, @object);
							tile.Add(@object);
							break;
					}
				}
				tileFloor.Add(tile);
				var td = go.GetComponent<Tool_Door>();
				if (td != null)
				{
					Destroy(td);
					yield return new WaitForEndOfFrame();
				}
			}
			tileFloors.Add(tileFloor);
			xdoc.Root.Add(tileFloors);

			var fileName = path + "Tactic00.ag.xml";
			xdoc.Save(fileName);

			Destroy(go);
			Debug.Log("Output elapsed time: " + ( Time.realtimeSinceStartup - startTime ));
		}

		private Vector3 GetGenePosition(int index, int width)
		{
			return new Vector3(index % width, floorNum, index / width);
		}

		private void ResetTCL(ToolComponentList tcl)
		{
			foreach (var fieldInfo in tcl.GetType().GetFields())
			{
				if (fieldInfo.Name == "objName" || fieldInfo.Name == "position")
				{
					fieldInfo.SetValue(tcl, true);
				}
				else
				{
					fieldInfo.SetValue(tcl, false);
				}
			}
		}

		private void OutputScripts(GameObject obj, XElement classRoot)
		{
			MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
			int scriptCount = 0;
			foreach (MonoBehaviour mb in scripts)
			{
				string scriptName = mb.GetType().Name;
				//Debug.Log(scriptName);
				//創建Class
				XElement elmClass = new XElement("Class", new XAttribute("id", scriptCount.ToString()), new XAttribute("name", scriptName));
				Component script = obj.GetComponent(Type.GetType(scriptName));
				//開始一個一個輸出
				foreach (FieldInfo fieldInfo in script.GetType().GetFields())
				{
					System.Object ob = script;
					//新增variable的element
					XElement elmType = new XElement(fieldInfo.Name, new XAttribute("type", fieldInfo.FieldType.ToString()));
					switch (fieldInfo.FieldType.ToString())
					{
						case "System.Boolean":
						case "System.Int32":
						case "System.String":
							elmType.Value = fieldInfo.GetValue(ob).ToString();
							break;
						case "UnityEngine.Vector3":
							string vector3 = fieldInfo.GetValue(ob).ToString();
							vector3 = vector3.Substring(1, vector3.Length - 2);
							string[] sArray = vector3.Split(',');
							// store as a Vector3
							Vector3 result = new Vector3(float.Parse(sArray[0]), float.Parse(sArray[1]), float.Parse(sArray[2]));
							XElement keyX = new XElement("x", result.x.ToString());
							XElement keyY = new XElement("y", result.y.ToString());
							XElement keyZ = new XElement("z", result.z.ToString());
							elmType.Add(keyX);
							elmType.Add(keyY);
							elmType.Add(keyZ);
							break;
						case "UnityEngine.Sprite":
							elmType.Value = fieldInfo.GetValue(ob).ToString().Split(' ')[0];
							break;
						case "UnityEngine.GameObject":
							elmType.Value = fieldInfo.GetValue(ob).ToString().Split(' ')[0];
							break;
						case "UnityEngine.GameObject[]":
							IList objs = (IList)fieldInfo.GetValue(ob);
							elmType.SetAttributeValue("count", objs.Count.ToString());
							for (int i = 0; i < objs.Count; i++)
							{
								XElement objPrefab = new XElement("obj", objs[i].ToString().Split(' ')[0]);
								elmType.Add(objPrefab);
							}
							break;
					}
					elmClass.Add(elmType);
				}
				scriptCount++;
				// 新增至Object底下
				classRoot.Add(elmClass);
			}
		}
	}
}