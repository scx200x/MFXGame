using System.Collections;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TestProto : MonoBehaviour
{
    public string FileName = "sjl.txt";

    private string DirPath = Application.streamingAssetsPath + "/";

    private string FilePath;

    public Button WriteBtn;
    public Button ReadBtn;
    
    // Start is called before the first frame update
    void Start()
    {
        
        
        
        
        if (!Directory.Exists(DirPath))
        {
            Directory.CreateDirectory(DirPath);
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        FilePath = DirPath + FileName;
        WriteBtn.onClick.AddListener(WriteTo);
        ReadBtn.onClick.AddListener(ReadFrom);
    }

    public void WriteTo()
    {
        SJL sr = new SJL()
        {
            Name = "sjl",
            Age = 26,
            Height = 176
        };

        using (FileStream fs = File.OpenWrite(FilePath))
        {
            byte[] bytes = sr.ToByteArray();
            fs.Write(bytes,0,bytes.Length);
        }
        
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        #endif
    }

    public void ReadFrom()
    {
        using (Stream stream = File.OpenRead(FilePath))
        {
            SJL sjl = SJL.Parser.ParseFrom(stream);
            Debug.LogFormat("Name:{0} Age:{1} Height:{2}",sjl.Name,sjl.Age,sjl.Height);
        }
    }
    
}
