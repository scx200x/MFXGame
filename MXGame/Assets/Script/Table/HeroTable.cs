using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

[Serializable]
public class HeroTableInfo
{
    public int Id;
    public int ResId;
    public string Name;
    public int Rank;
    public int HeroType;
}

public class HeroTable : SingltionCreator<HeroTable>
{
    private Dictionary<int,HeroTableInfo> TableDatas;
    
    public HeroTableInfo GetTableInfo(int Id)
    {
        if (TableDatas == null)
        {
            TableDatas = new Dictionary<int, HeroTableInfo>();

            string jsonData = TableManager.Instance.GetTableText("HeroTable");

            if (jsonData != null)
            {
                HeroTableInfo[] heroes = JsonMapper.ToObject<HeroTableInfo[]>(jsonData);

                foreach (var hero in heroes)
                {
                    TableDatas.Add(hero.Id,hero);
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