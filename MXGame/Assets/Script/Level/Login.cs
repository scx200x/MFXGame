using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public bool NetConnected = false;


    // Start is called before the first frame update
    void Start()
    {
        AudioManager.GetInstance().SetAolumeSource(AolumeSource);
        AudioManager.GetInstance().SetSoundSource(SoundSource);
        
        EventMsgCenter.RegisterNetMsg(NetEventName.AccountLogin,OnNetAccountLoginResp,false);
        EventMsgCenter.RegisterNetMsg(NetEventName.RoleLogin,OnNetRoleLoginResp,false);
        EventMsgCenter.RegisterGlobalMsg(EventName.Connected,Connected,false);
    }

    private void OnDisable()
    {
        EventMsgCenter.UnRegisterNetMsg(NetEventName.AccountLogin,OnNetAccountLoginResp);
        EventMsgCenter.UnRegisterNetMsg(NetEventName.RoleLogin,OnNetRoleLoginResp);
        EventMsgCenter.UnRegisterMsg(EventName.Connected,Connected);
    }

    public void SetMsgBoxEnd()
    {
        ErrorMsgObj.SetActive(false);
    }
    
    public void OnStartGame()
    {
        AudioManager.GetInstance().PlaySound();

        if (GameServerNetProcess.Instance.GetConnectStatus() == GameServerNetProcess.ConnectionStatus.Connected)
        {
            GameServerNetProcess.Instance.SendAccLoginRequest();
        }
        else
        {
            ShowError(ErrorMsgID.NetConnectNull);
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
            GameDataManager.Instance.AccountID = sjl.AccountId;

            GameDataManager.Instance.ClearAccounts();
            
            foreach (var AccountInfo in sjl.List)
            {
                GameDataManager.Instance.AddAccount(AccountInfo.RoleId,AccountInfo.Name,AccountInfo.GameTime,AccountInfo.Difficult);
            }
            
            ShowRecordPanel(sjl);
        }
    }

    public void OnNetRoleLoginResp(params object[] package)
    {
        ZPackage Package = (ZPackage)(package[0]);
        
        Cs.RoleLoginResponse rjl = Cs.RoleLoginResponse.Parser.ParseFrom(Package.GetArray());

        if (rjl != null)
        {
            GameDataManager.Instance.SetCurrentRole(rjl.RoleId);
            HeroManager.Instance.Clear();

            foreach (var heroInfo in rjl.HeroList)
            {
                HeroManager.Instance.AddHeroInfo(heroInfo);
            }

            foreach (var fightHeroTeamInfo in rjl.TeamInfoList)
            {
                HeroManager.Instance.AddTeam(fightHeroTeamInfo.TeamId,fightHeroTeamInfo);
            }

            StartCoroutine(SceneLoad("Scenes/Fight"));
        }
    }

    private IEnumerator SceneLoad(string sceneName)
    {
        yield return new WaitForSeconds(0);
        SceneManager.LoadScene(sceneName);
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

    private void Connected(params object[] objects)
    {
        NetConnected = true;
    }

    private void ShowTips(TipsID ID)
    {
        ErrorMsg.text = global::TipsMsg.TipsMsgTable[ID];
        ErrorMsgObj.SetActive(true);
        ErrorMsgObj.GetComponent<DOTweenAnimation>().DORestart();
    }

    private void ShowError(ErrorMsgID ID)
    {
        ErrorMsg.text = global::ErrorMsg.ErrorMsgTable[ID];
        ErrorMsgObj.SetActive(true);
        ErrorMsgObj.GetComponent<DOTweenAnimation>().DORestart();
    }

    void Update()
    {
        if (NetConnected)
        {
            ShowTips(TipsID.NetConnected);
            NetConnected = false;
        }
    }
}
