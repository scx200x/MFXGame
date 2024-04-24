
using System.IO;
using UnityEngine;

[System.Serializable]
public class Record
{
    public int ResulationIndex;
    public float VolumePercent;
    public float SoundEffectPercent;
}

public static class SaveSystem
{
    public static void SaveRecord(Record record)
    {
        string path = Application.persistentDataPath + "/Record.json";
        var content = JsonUtility.ToJson(record, true);
        File.WriteAllText(path,content);
    }

    public static Record LoadRecord()
    {
        string path = Application.persistentDataPath + "/Record.json";
        if (File.Exists(path))
        {
            var content = File.ReadAllText(path);
            var record = JsonUtility.FromJson<Record>(content);
            return record;
        }
        else
        {
            return null;
        }
    }
}
