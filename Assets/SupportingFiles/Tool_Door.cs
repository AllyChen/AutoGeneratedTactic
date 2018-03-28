using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_Door : MonoBehaviour, IToolSetUp, IToolSwitch
{
    public List<ToolSwitch> tSwitch;
    //互動物件
    public GameObject door;
    //設定動畫用
    public Vector3[] tRotate;
    //設定動畫用
    public Vector3[] tTramsform;
    public int status;

    public void ToolInitialization()
    {
        ToolSwitch DoorOpen = new ToolSwitch()
        {
            triggerName = "Close",
            active = true
        };
        ToolSwitch DoorClose = new ToolSwitch()
        {
            triggerName = "Open",
            active = false
        };
        tSwitch = new List<ToolSwitch>
        {
            DoorOpen,
            DoorClose
        };
		SetStatus(status);

	}

    public int GetStatus()
    {
        return status;
    }

    public void SetStatus(int status)
    {
        this.status = status;
		//door.transform.localPosition = tTramsform[status] ;
		if (gameObject.activeSelf == true) 
		{
			ChangeState(status);
		}
        
    }

    public List<ToolSwitch> GetSwitch()
    {
        return tSwitch;
    }

    //玩家觸碰執行
    public void SetSwitch()
    {
        status++;
        if (status > tSwitch.Count)
        {
            status = 0;
        }
        ChangeState(status);
    }

    private void ChangeState(int status)
    {
		door.transform.localPosition = tTramsform[status];
		door.transform.localEulerAngles = tRotate[status];
    }

    
}
