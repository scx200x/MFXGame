using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FightLevel : MonoBehaviour
{
    public List<PortraitItem> portraitItems;
    public RoundBar roundBar;
    public Text LevelNameTxt;
    public Text roundNumberTxt;
    public Scrollbar nuqiBar;
    public Text nuqiTxt;
    public AudioSource audioSource;
    public AudioSource soundSource;
    public Transform actorPos;
    public Transform monsterPos;
    
    private int roundNumber;
    private int maxNuqi;
    private int nuqiNumber;

    public int NuqiNumber
    {
        set
        {
            nuqiNumber = value;
        }

        get
        {
            return nuqiNumber;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.GetInstance().SetAolumeSource(audioSource);
        AudioManager.GetInstance().SetSoundSource(soundSource);
        
        EventMsgCenter.RegisterNetMsg(NetEventName.EndBattle,OnNetEndBattleResp,false);
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevelName()
    {
        
    }

    public void SetLevelRound()
    {
        
    }

    public void RoundOver()
    {
        roundNumber++;
        roundNumberTxt.text = string.Format("剩余回合:{0}", roundNumber);
        roundBar.SetArrowPosition(roundNumber -1);
    }

    public void Exit()
    {
        AudioManager.GetInstance().PlaySound();
    }

    public void Run()
    {
        AudioManager.GetInstance().PlaySound();
    }

    public void NuqiChange(int nuqi)
    {
        NuqiNumber += nuqi;
        Mathf.Clamp(nuqiNumber, 0, maxNuqi);
        nuqiBar.size = (float)NuqiNumber / maxNuqi;
        nuqiTxt.text = string.Format("{0}/{1}", NuqiNumber, maxNuqi);
    }

    public void CreateActors()
    {
        
    }

    public void CreateMonsters()
    {
        
    }
    
    private void Clear()
    {
        EventMsgCenter.UnRegisterNetMsg(NetEventName.EndBattle,OnNetEndBattleResp);
    }

    public void OnNetEndBattleResp(params object[] package)
    {
        ZPackage Package = (ZPackage)(package[0]);
        
        Cs.EndBattleResponse ebr = Cs.EndBattleResponse.Parser.ParseFrom(Package.GetArray());
    }
}
