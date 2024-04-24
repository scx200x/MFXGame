using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerBase
{
    public virtual void OnInit()
    { }

    public void OnStart()
    { }

    public void OnEnd()
    { }
    
    public void OnDestory()
    { }
    
    public void OnUpdate(float DeltaTime){}
}