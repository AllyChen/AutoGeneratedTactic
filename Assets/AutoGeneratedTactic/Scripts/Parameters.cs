using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parameters : MonoBehaviour {

	private int tileLengh;
	private int tileWidth;
	private int minEnemy;
	private int minTrap;
	private int minTreasure;
	private int maxEnemy;
	private int maxTrap;
	private int maxTreasure;

	private bool fitness_Rectangle;
	private bool fitness_Corridor;
	private bool fitness_ProtectTreasure;
	private bool fitness_OnMainPath;
	private bool fitness_BesideMainPath;
	private bool fitness_TwoPronged;

	private bool isTreasureOnMainPath;
	private bool isTreasureBesideMainPath;

	private float weight_RectangleQuality;
	private float weight_CorridorQuality;
	private float weight_Fitness_Defense;
	private float weight_Fitness_OnMainPath;
	private float weight_Fitness_BesideMainPath;
	private float weight_Fitness_TwoPronged;

	private Toggle Toggle_Rectangle;
	private Toggle Toggle_Corridor;
	private Toggle Toggle_ProtectTreasure;
	private Toggle Toggle_OnMainPath;
	private Dropdown Dropdown_OnMainPath;
	private Toggle Toggle_BesideMainPath;
	private Dropdown Dropdown_BesideMainPath;
	private Toggle Toggle_TwoPronged;

	// Use this for initialization
	void Start () {
		GameObject.Find("InputField_Length").GetComponent<InputField>().text = "1";
		GameObject.Find("InputField_Width").GetComponent<InputField>().text = "1";
		GameObject.Find("InputField_minEnemy").GetComponent<InputField>().text = "1";
		GameObject.Find("InputField_minTrap").GetComponent<InputField>().text = "1";
		GameObject.Find("InputField_minTreasure").GetComponent<InputField>().text = "1";
		GameObject.Find("InputField_MAXEnemy").GetComponent<InputField>().text = "1";
		GameObject.Find("InputField_MAXTrap").GetComponent<InputField>().text = "1";
		GameObject.Find("InputField_MAXTreasure").GetComponent<InputField>().text = "1";
		GameObject.Find("InputField_Rectangle").GetComponent<InputField>().text = "1.0";
		GameObject.Find("InputField_Corridor").GetComponent<InputField>().text = "1.0";
		GameObject.Find("InputField_Defense").GetComponent<InputField>().text = "1.0";
		GameObject.Find("InputField_OnMainPath").GetComponent<InputField>().text = "1.0";
		GameObject.Find("InputField_BesideMainPath").GetComponent<InputField>().text = "1.0";
		GameObject.Find("InputField_TwoPronged").GetComponent<InputField>().text = "1.0";

		Toggle_Rectangle = GameObject.Find("Toggle_Rectangle").GetComponent<Toggle>();
		Toggle_Corridor = GameObject.Find("Toggle_Corridor").GetComponent<Toggle>();
		Toggle_ProtectTreasure = GameObject.Find("Toggle_ProtectTreasure").GetComponent<Toggle>();
		Toggle_OnMainPath = GameObject.Find("Toggle_OnMainPath").GetComponent<Toggle>();
		Dropdown_OnMainPath = GameObject.Find("Dropdown_OnMainPath").GetComponent<Dropdown>();
		Toggle_BesideMainPath = GameObject.Find("Toggle_BesideMainPath").GetComponent<Toggle>();
		Dropdown_BesideMainPath = GameObject.Find("Dropdown_BesideMainPath").GetComponent<Dropdown>();
		Toggle_TwoPronged = GameObject.Find("Toggle_TwoPronged").GetComponent<Toggle>();
	}
	
	public int GetTheLenghOfTile()
	{
		string Lengh = GameObject.Find("InputField_Length").GetComponent<InputField>().text;
		tileLengh = int.Parse(Lengh);
		return tileLengh;
	}

	public int GetTheWidthOfTile()
	{
		string Width = GameObject.Find("InputField_Width").GetComponent<InputField>().text;
		tileWidth = int.Parse(Width);
		return tileWidth;
	}

	public int GetMinEnemy()
	{
		minEnemy = int.Parse(GameObject.Find("InputField_minEnemy").GetComponent<InputField>().text);
		return minEnemy;
	}

	public int GetMinTrap()
	{
		minTrap = int.Parse(GameObject.Find("InputField_minTrap").GetComponent<InputField>().text);
		return minTrap;
	}

	public int GetMinTreasure()
	{
		minTreasure = int.Parse(GameObject.Find("InputField_minTreasure").GetComponent<InputField>().text);
		return minTreasure;
	}

	public int GetMaxEnemy()
	{
		maxEnemy = int.Parse(GameObject.Find("InputField_MAXEnemy").GetComponent<InputField>().text);
		return maxEnemy;
	}

	public int GetMaxTrap()
	{
		maxTrap = int.Parse(GameObject.Find("InputField_MAXTrap").GetComponent<InputField>().text);
		return maxTrap;
	}

	public int GetMaxTreasure()
	{
		maxTreasure = int.Parse(GameObject.Find("InputField_MAXTreasure").GetComponent<InputField>().text);
		return maxTreasure;
	}

	public void OnClick_Toggle_Rectangle()
	{
		if (Toggle_Rectangle.isOn)
		{
			fitness_Rectangle = true;
		}
		else
		{
			fitness_Rectangle = false;
		}
		Debug.Log("isRectangle = " + fitness_Rectangle + ", Weight = " + GetTheWeight_RectangleQuality());
	}

	public void OnClick_Toggle_Corridor()
	{
		if (Toggle_Corridor.isOn)
		{
			fitness_Corridor = true;
		}
		else
		{
			fitness_Corridor = false;
		}
		Debug.Log("isCorridor = " + fitness_Corridor + ", Weight = " + GetTheWeight_CorridorQuality());
	}

	public void OnClick_Toggle_ProtectTreasure()
	{
		if (Toggle_ProtectTreasure.isOn)
		{
			fitness_ProtectTreasure = true;
		}
		else
		{
			fitness_ProtectTreasure = false;
		}
		Debug.Log("isProtectTreasure = " + fitness_ProtectTreasure + ", Weight = " + GetTheWeight_Fitness_Defense());
	}

	public void OnClick_Toggle_TwoPronged()
	{
		if (Toggle_TwoPronged.isOn)
		{
			fitness_TwoPronged = true;
		}
		else
		{
			fitness_TwoPronged = false;
		}
		Debug.Log("isTwoPronged = " + fitness_TwoPronged + ", Weight = " + GetTheWeight_Fitness_TwoPronged());
	}

	public void OnClick_Toggle_OnMainPath()
	{
		if (Toggle_OnMainPath.isOn)
		{
			fitness_OnMainPath = true;
			if (Dropdown_OnMainPath.value == 0)
			{
				isTreasureOnMainPath = false;
			}
			else
			{
				isTreasureOnMainPath = true;
			}
		}
		else
		{
			fitness_OnMainPath = false;
			if (Dropdown_OnMainPath.value == 0)
			{
				isTreasureOnMainPath = false;
			}
			else
			{
				isTreasureOnMainPath = true;
			}
		}
		Debug.Log("isOnMainPath = " + fitness_OnMainPath + "// isTreasureOnMainPath = " + isTreasureOnMainPath + ", Weight = " + GetTheWeight_Fitness_OnMainPath());
	}

	public void OnClick_Toggle_BesideMainPath()
	{
		if (Toggle_BesideMainPath.isOn)
		{
			fitness_BesideMainPath = true;
			if (Dropdown_BesideMainPath.value == 0)
			{
				isTreasureBesideMainPath = false;
			}
			else
			{
				isTreasureBesideMainPath = true;
			}
		}
		else
		{
			fitness_BesideMainPath = false;
			if (Dropdown_BesideMainPath.value == 0)
			{
				isTreasureBesideMainPath = false;
			}
			else
			{
				isTreasureBesideMainPath = true;
			}
		}
		Debug.Log("isBesideMainPath = " + fitness_BesideMainPath + "// isTreasureBesideMainPath = " + isTreasureBesideMainPath + ", Weight = " + GetTheWeight_Fitness_BesideMainPath());
	}

	public void OnClick_Dropdown_OnMainPath()
	{
		if (Dropdown_OnMainPath.value == 0)
		{
			isTreasureOnMainPath = false;
		}
		else
		{
			isTreasureOnMainPath = true;
		}
		Debug.Log("isTreasureOnMainPath = " + isTreasureOnMainPath);
	}

	public void OnClick_Dropdown_BesideMainPath()
	{
		if (Dropdown_BesideMainPath.value == 0)
		{
			isTreasureBesideMainPath = false;
		}
		else
		{
			isTreasureBesideMainPath = true;
		}
		Debug.Log("isTreasureBesideMainPath = " + isTreasureBesideMainPath);
	}

	public bool GetIsFitness_Rectangle()
	{
		return fitness_Rectangle;
	}

	public bool GetIsFitness_Corridor()
	{
		return fitness_Corridor;
	}

	public bool GetIsFitness_Defense()
	{
		return fitness_ProtectTreasure;
	}

	public bool GetIsFitness_TwoPronged()
	{
		return fitness_TwoPronged;
	}

	public bool GetIsFitness_OnMainPath()
	{
		return fitness_OnMainPath;
	}

	public bool GetIsFitness_BesideMainPath()
	{
		return fitness_BesideMainPath;
	}

	public bool GetIsTreasureOnMainPath()
	{
		return isTreasureOnMainPath;
	}

	public bool GetIsTreasureBesideMainPath()
	{
		return isTreasureBesideMainPath;
	}

	public float GetTheWeight_RectangleQuality()
	{
		string getParamet = GameObject.Find("InputField_Rectangle").GetComponent<InputField>().text;
		weight_RectangleQuality = float.Parse(getParamet);
		return weight_RectangleQuality;
	}

	public float GetTheWeight_CorridorQuality()
	{
		string getParamet = GameObject.Find("InputField_Corridor").GetComponent<InputField>().text;
		weight_CorridorQuality = float.Parse(getParamet);
		return weight_CorridorQuality;
	}

	public float GetTheWeight_Fitness_Defense()
	{
		string getParamet = GameObject.Find("InputField_Defense").GetComponent<InputField>().text;
		weight_Fitness_Defense = float.Parse(getParamet);
		return weight_Fitness_Defense;
	}

	public float GetTheWeight_Fitness_TwoPronged()
	{
		string getParamet = GameObject.Find("InputField_TwoPronged").GetComponent<InputField>().text;
		weight_Fitness_TwoPronged = float.Parse(getParamet);
		return weight_Fitness_TwoPronged;
	}

	public float GetTheWeight_Fitness_OnMainPath()
	{
		string getParamet = GameObject.Find("InputField_OnMainPath").GetComponent<InputField>().text;
		weight_Fitness_OnMainPath = float.Parse(getParamet);
		return weight_Fitness_OnMainPath;
	}

	public float GetTheWeight_Fitness_BesideMainPath()
	{
		string getParamet = GameObject.Find("InputField_BesideMainPath").GetComponent<InputField>().text;
		weight_Fitness_BesideMainPath = float.Parse(getParamet);
		return weight_Fitness_BesideMainPath;
	}
}
