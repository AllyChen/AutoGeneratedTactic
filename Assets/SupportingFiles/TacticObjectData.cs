using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticObjectData : MonoBehaviour {

	public string tName;
	public Vector3 tPosition;
	public Vector3 tRotation;
	public Vector3 tScale;
	public bool unique;
	public int tAnchor;
	public Sprite tObjSprite;
	private GameObject[] _objPrefabs;
	// Use this for initialization
	void Start()
	{
		tName = gameObject.name;
		tPosition = new Vector3(transform.localPosition.x / 10, transform.localPosition.y / 10, transform.localPosition.z / 10);
		tRotation = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
		tScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

	//初始化
	public void SetUp()
	{
		unique = false;
		tAnchor = 4;
	}
	/*
	public void UpDateInfo(TacticBoardController.EditTool tool)
	{
		//丟給UIManager處理，讓它顯示Inf在右側
		GameObject.Find("AttributePanel").GetComponent<TacticAttributeView>().ReadInfo(gameObject, tool);
	}*/
	public GameObject[] ObjPrefabs
	{
		get { return _objPrefabs; }
		set { _objPrefabs = value; }
	}
	public GameObject GetRandomObj()
	{
		GameObject obj = _objPrefabs[Random.Range(0, _objPrefabs.Length)];
		return obj;
	}
}
