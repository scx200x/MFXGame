using UnityEngine;

enum LevelType
{
    Main = 0,
    Fight,
}

public class CityLevelElementMng : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioSource soundSource;
    public GameObject loadingPanel;

    public MainLevel mainLevel;
    public FightLevel fightLevel;

    private LevelType levelType = LevelType.Main;
    
    void Start()
    {
        AudioManager.GetInstance().SetAolumeSource(audioSource);
        AudioManager.GetInstance().SetSoundSource(soundSource);
        
        EventMsgCenter.RegisterStageMsg(EventName.LoadingOver,LoadingOver);
    }
    
    public void LoadingOver(params object[] objs)
    {
        loadingPanel.SetActive(false);

        if (levelType == LevelType.Main)
        {
            PlayMainTheme();
        }
        else
        {
            PlayFightTheme();
        }
    }

    public void PlayMainTheme()
    {
        AudioManager.Instance.SetVolumeSourceRes("Audio/vietnam-bamboo-flute-143601");
        AudioManager.Instance.PlayVolume();
    }

    public void PlayFightTheme()
    {
        AudioManager.Instance.SetVolumeSourceRes("Audio/Casual Theme #2");
        AudioManager.Instance.PlayVolume();
    }
}