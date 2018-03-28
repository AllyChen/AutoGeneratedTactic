using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IToolSwitch
{
    int GetStatus();
    void SetStatus(int status);
    List<ToolSwitch> GetSwitch();
    void SetSwitch();
}
