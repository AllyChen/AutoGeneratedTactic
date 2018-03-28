using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolComponentList : MonoBehaviour
{
    //所有物件能用的Component
    public bool objName;
    public bool position;
    public bool rotation;
    public bool scale;
    public bool unique;
    public bool anchor;
    //actTrigger需要有自己的script並有使用ToolTrigger
    public bool actTrigger;
}
