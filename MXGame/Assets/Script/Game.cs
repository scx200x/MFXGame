using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public int LevelID;
}

public class Game : MonoBehaviour
{
    private static Game instance = null;

    public static Game GetInstance()
    {
        return instance;
    }

    public GameData gameData;
    
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //string Path = Application.streamingAssetsPath + "/ConsoleApplication1.exe";
        //Application.OpenURL(Path);
    }

    // Update is called once per frame
    void Update()
    {
        GameServerNetProcess.Instance.Update();
    }

    private void OnDisable()
    {
        EventMsgCenter.SendMsg(EventName.GameOver);
    }
}
