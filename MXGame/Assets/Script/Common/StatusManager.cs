using System;
using System.Collections;
using System.Collections.Generic;

public class StatusManager
{
    public List<List<Status>> statusChainList;
    public bool isAccomomplish = false;
    
    public void OnUpdate()
    {
        foreach (var statusChain in statusChainList)
        {
            foreach (var status in statusChain)
            {
                status.OnUpdate();
            }
        }

        CheckAccomplish();
    }

    //是否全部完成了
    public void CheckAccomplish()
    {
        foreach (var statusChain in statusChainList)
        {
            foreach (var status in statusChain)
            {
                if (status.isFinish)
                {
                    statusChain.Remove(status);
                }
            }

            if (statusChain.Count <= 0)
            {
                statusChainList.Remove(statusChain);
            }
        }

        if (statusChainList.Count <= 0)
        {
            isAccomomplish = true;
        }
    }
}