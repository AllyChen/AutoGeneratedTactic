using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parameters : MonoBehaviour {

	private int tileLengh;
	private int tileWidth;

	// Use this for initialization
	void Start () {
		GameObject.Find("InputField_Length").GetComponent<InputField>().text = "1";
		GameObject.Find("InputField_Width").GetComponent<InputField>().text = "1";
	}
	
	// Update is called once per frame
	void Update () {
		
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

}
