using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

[Serializable]
public class HeroResInfo
{
    public int ResId;
    public string IconName;
    public string ResPath;
}

public class HeroResTable : SingltionCreator<HeroResTable>
{
    private Dictionary<int,HeroResInfo> TableDatas;
    
    public HeroResInfo GetTableInfo(int Id)
    {
        if (TableDatas == null)
        {
            TableDatas = new Dictionary<int, HeroResInfo>();

            string jsonData = TableManager.Instance.GetTableText("HeroResTable");

            if (jsonData != null)
            {
                HeroResInfo[] heroeResList = JsonMapper.ToObject<HeroResInfo[]>(jsonData);

                foreach (var heroRes in heroeResList)
                {
                    TableDatas.Add(heroRes.ResId,heroRes);
                }
            }
        }

        if (TableDatas.ContainsKey(Id))
        {
            return TableDatas[Id];
        }

        return null;
    }
}