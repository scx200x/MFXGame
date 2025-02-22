using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRecord : MonoBehaviour
{
    public GameObject RoundImageObj;
    public GameObject EmptyObj;
    public Text RoundTxt;
    public Text GameTimeTxt;
    public Text PlayTimeTxt;

    private long roleID;

    public long RoleID
    {
        set { roleID = value;}
        get { return roleID; }
    }

    private void Awake()
    {
        GameTimeTxt.text = "";
        PlayTimeTxt.text = "";
    }

    // Start is called before the first frame update
    void Start()
    {
        EmptyObj.SetActive(true);
        RoundImageObj.SetActive(true);
    }

    public void Play()
    {
        AudioManager.GetInstance().PlaySound();
        
        if (GameServerNetProcess.Instance.GetConnectStatus() == GameServerNetProcess.ConnectionStatus.Connected)
        {
            GameServerNetProcess.Instance.SendRoleLoginRequest(RoleID);
        }
    }
    
    public void UpdateUI(Cs.AccSimpleInfo simpleInfo)
    {
        RoleID = simpleInfo.RoleId;

        if (simpleInfo.GameTime > 0)
        {
            GameTimeTxt.text = "";
            PlayTimeTxt.text = "";
        }
        else
        {
            PlayTimeTxt.text = Utility.GetGameTime(simpleInfo.GameTime);
            GameTimeTxt.text = Utility.GetRealGameTime(simpleInfo.GameTime);
        }
    }
    
}
