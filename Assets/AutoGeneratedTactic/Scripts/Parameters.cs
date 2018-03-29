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

	private Toggle onlyRectangle;

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

		onlyRectangle = GameObject.Find("Toggle_OnlyRectangle").GetComponent<Toggle>();
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

	public void OnClick_Toggle_onlyRectangle()
	{
		bool isOnlyRectangle = false;

		if (onlyRectangle.isOn)
		{
			isOnlyRectangle = true;
		}
		else
		{
			isOnlyRectangle = false;
		}
		Debug.Log(isOnlyRectangle);
	}

}
