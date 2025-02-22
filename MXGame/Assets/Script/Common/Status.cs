using System;
using System.Collections;
using System.Collections.Generic;

public class Status
{
    public bool isFinish = false;
    public StatusManager subStatusChainList = null;
    public Status NextStatus = null;

    public virtual void OnInit() { }
    public virtual void OnUpdate() { }
    public virtual void OnEnd() { }

    public void CreateSubStatusChain()
    {
        subStatusChainList = new StatusManager();
    }
}