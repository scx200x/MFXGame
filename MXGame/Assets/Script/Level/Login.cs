using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    public Text ErrorMsg;
    public GameObject ErrorMsgObj;
    public GameObject ConfigObj;
    public AudioSource AolumeSource;
    public AudioSource SoundSource;
    public GameObject RecordObj;
    public GameRecord[] GameRecordItems;


    // Start is called before the first frame update
    void Start()
    {
        AudioManager.GetInstance().SetAolumeSource(AolumeSource);
        AudioManager.GetInstance().SetSoundSource(SoundSource);
        
        EventMsgCenter.RegisterNetMsg(NetEventName.AccountLogin,OnNetAccountLoginResp,false);
        EventMsgCenter.RegisterNetMsg(NetEventName.RoleLogin,OnNetRoleLoginResp,false);
    }

    private void OnDisable()
    {
        EventMsgCenter.UnRegisterNetMsg(NetEventName.AccountLogin,OnNetAccountLoginResp);
        EventMsgCenter.UnRegisterNetMsg(NetEventName.RoleLogin,OnNetRoleLoginResp);
    }

    public void SetMsgBoxEnd()
    {
        ErrorMsgObj.SetActive(false);
    }
    
    public void OnStartGame()
    {
        AudioManager.GetInstance().PlaySound();

        if (GameServerNetProcess.Instance.GetConnectStatus() == GameServerNetProcess.ConnectionStatus.None)
        {
            GameServerNetProcess.Instance.ConnectGameServer();
        }
        else
        {
            GameServerNetProcess.Instance.CloseConnect();
            GameServerNetProcess.Instance.ConnectGameServer();
        }
    }

    public void OnConfig()
    {
        AudioManager.GetInstance().PlaySound();
        ConfigObj.SetActive(true);
        ConfigObj.GetComponent<Config>().UpdateUI();
    }

    public void OnExit()
    {
        AudioManager.GetInstance().PlaySound();
        GameServerNetProcess.Instance.CloseConnect();
        Application.Quit();
    }

    public void OnNetAccountLoginResp(params object[] package)
    {
        ZPackage Package = (ZPackage)(package[0]);
        
        Cs.AccLoginResponse sjl = Cs.AccLoginResponse.Parser.ParseFrom(Package.GetArray());

        if (sjl != null)
        {
            GameServerNetProcess.Instance.AccountID = sjl.AccountId;
            ShowRecordPanel(sjl);
        }
    }

    public void OnNetRoleLoginResp(params object[] package)
    {
        ZPackage Package = (ZPackage)(package[0]);
        
        Cs.RoleLoginResponse rjl = Cs.RoleLoginResponse.Parser.ParseFrom(Package.GetArray());

        if (rjl != null)
        {
            
        }
    }

    private void ShowRecordPanel(Cs.AccLoginResponse sjl)
    {
        RecordObj.SetActive(true);

        for (int i = 0; i < sjl.List.Count; i++)
        {
            GameRecordItems[i].UpdateUI(sjl.List[i]);
            GameRecordItems[i].gameObject.SetActive(true);
        }

        if (sjl.List.Count < 3)
        {
            GameRecordItems[sjl.List.Count].gameObject.SetActive(true);
        }
    }

    private void ShowError(ErrorMsgID id)
    {
        ErrorMsg.text = global::ErrorMsg.ErrorMsgTable[id];
        ErrorMsgObj.SetActive(true);
        ErrorMsgObj.GetComponent<DOTweenAnimation>().DORestart();
    }
}
