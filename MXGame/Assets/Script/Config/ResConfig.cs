using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SystemID
{
    Hero = 0,
    Level = 1,
    Lineup = 2
}

public struct SystemPrefabPath
{
    public SystemPrefabPath(string Path)
    {
        this.Path = Path;
    }
    
    public string Path;
}


public class GameConfig
{
    public static Dictionary<SystemID, SystemPrefabPath> SystemConfigDict;

    public GameConfig()
    {
        SystemConfigDict = new Dictionary<SystemID, SystemPrefabPath>();
        SystemConfigDict.Add(SystemID.Hero,new SystemPrefabPath("UI/System/Hero"));
        SystemConfigDict.Add(SystemID.Level,new SystemPrefabPath("UI/System/Level"));
        SystemConfigDict.Add(SystemID.Lineup,new SystemPrefabPath("UI/System/Lineup"));
    }
}
