using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class HeroUIItem : MonoBehaviour
{
    public Image portrait;
    public GameObject shangZheng;
    public Image edgeImage;
    public Toggle toggle;

    public HeroTableInfo heroInfo;
    
    [SerializeField] SpriteAtlas atlasProtrait;
    [SerializeField] SpriteAtlas atlasEdge;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Select(bool bEnable)
    {
        if (toggle.isOn)
        {
            AudioManager.GetInstance().PlaySound();
            object[] objs = new[] { this };
            EventMsgCenter.SendMsg(EventName.HeroSelect,objs);
        }
    }

    private string GetRankEdge(int rank)
    {
        switch (rank)
        {
            case 1:
                return "headFrame_01elite";
            case 2:
                return "headFrame_02elite";
            case 3:
                return "headFrame_03elite";
            case 4:
                return "headFrame_04elite";
            case 5:
                return "headFrame_05elite";
            default:
                return "headFrame_01elite";
        }
    }

    public void SetHeroInfo(int Id)
    {
        heroInfo = HeroTable.Instance.GetTableInfo(Id);

        int ResId = heroInfo.ResId;

        HeroResInfo heroResInfo = HeroResTable.Instance.GetTableInfo(ResId);

        if (heroResInfo != null)
        {
            portrait.sprite = atlasProtrait.GetSprite(heroResInfo.IconName);
        }
        
        edgeImage.sprite = atlasEdge.GetSprite(GetRankEdge(heroInfo.Rank));

        HeroManager.HeroTeam heroTeam = HeroManager.Instance.GetCurrentTeam();

        if (heroTeam != null)
        {
            if (heroTeam.heroList.Contains(Id))
            {
                shangZheng.SetActive(true);
            }
            else
            {
                shangZheng.SetActive(false);
            }
        }
    }
}
