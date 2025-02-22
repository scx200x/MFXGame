using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine.Device;

public class TableManager : SingltionCreator<TableManager>, IManagerBaseInterface
{
    public class TableInfo
    {
        public string path;
        public string fileContent;
    }
    
    public TableManager()
    {
        LoadTableList = new Dictionary<string, TableInfo>();
        LoadTable("HeroTable");
    }

    public void OnDisable(params object[] package)
    {

    }
    
    public bool LoadTable(string filePath)
    {
        if (!LoadTableList.ContainsKey(filePath))
        {
            filePath = filePath += ".json";
            string fullFilePath = Path.Combine(Application.streamingAssetsPath, filePath);

            if (File.Exists(fullFilePath))
            {
                string jsonContent = File.ReadAllText(fullFilePath);
            
                TableInfo tableInfo = new TableInfo();
                tableInfo.path = filePath;
                tableInfo.fileContent = jsonContent;
                LoadTableList.Add(filePath,tableInfo);
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    public void Clean()
    {
        LoadTableList.Clear();
    }
    
    public string GetTableText(string filePath)
    {
        if (LoadTableList.ContainsKey(filePath))
        {
            return LoadTableList[filePath].fileContent;
        }
        else
        {
            if (LoadTable(filePath))
            {
                return LoadTableList[filePath].fileContent;
            }
        }

        return null;
    }

    private Dictionary<string, TableInfo> LoadTableList;
}