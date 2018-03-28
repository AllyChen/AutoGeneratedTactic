using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool_Torch : MonoBehaviour, IToolSetUp, IToolSwitch
{
    public List<ToolSwitch> tSwitch;
    //互動物件
    public GameObject tLight;
    public int status;
    // Use this for initialization
    public void ToolInitialization()
    {
        //status = 0;
        ToolSwitch lightOn = new ToolSwitch()
        {
            triggerName = "Light On",
            active = true
        };
        ToolSwitch lightDown = new ToolSwitch()
        {
            triggerName = "Light Down",
            active = false
        };
        tSwitch = new List<ToolSwitch>
        {
            lightOn,
            lightDown
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
        if (status >= tSwitch.Count)
        {
            status = 0;
        }
    }
    private void ChangeState(int status)
    {
        tLight.SetActive(tSwitch[status].active);
    }
}
