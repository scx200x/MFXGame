using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Hero : MonoBehaviour
{
    public HeroUIItem[] heroUIItems;
    public Text gold;
    public Text stone;
    public Text heroPageNumber;

    private int page = 1;
    private int totalPage;
    private int selectHeroId = 0;

    public void Start()
    {
        EventMsgCenter.RegisterStageMsg(EventName.HeroSelect,SelectHero);
        //StartCoroutine(Init());
    }

    public void UpdateUI()
    {
        GameDataManager.AccountInfo role = GameDataManager.Instance.GetRole();

        if (role != null)
        {
            gold.text = Utility.GetNumberToString(role.gold);
            stone.text = Utility.GetNumberToString(role.stone);
        }

        SetTotalPage();
        page = 1;
        
        SetHeroPage(page);
    }

    public void Close()
    {
        AudioManager.GetInstance().PlaySound();
        gameObject.SetActive(false);
    }

    public void NextHeroPage()
    {
        AudioManager.GetInstance().PlaySound();
        
        if (page < totalPage)
        {
            page++;
            SetHeroPage(page);
        }
    }

    public void PreHeroPage()
    {
        AudioManager.GetInstance().PlaySound();

        if (page > 1)
        {
            page--;
            SetHeroPage(page);
        }
    }

    private void SetHeroPage(int pageNumber)
    {
        Dictionary<Int32, HeroManager.HeroInfo> HeroList = HeroManager.Instance.GetHeroList();
        
        heroUIItems[0].toggle.group.SetAllTogglesOff();

        for (int i = 0; i < heroUIItems.Length; i++)
        {
            heroUIItems[i].gameObject.SetActive(false);
        }

        int start = (pageNumber - 1) * 10;
        int end = pageNumber * 10;
        int index = 0;
        
        foreach (var heroInfo in HeroList)
        {
            if (index >= start && index < end)
            {
                heroUIItems[index - start].SetHeroInfo(heroInfo.Key);
                heroUIItems[index - start].gameObject.SetActive(true);

                if (selectHeroId == 0)
                {
                    selectHeroId = heroUIItems[index - start].heroInfo.Id;
                }

                if (heroUIItems[index - start].heroInfo.Id == selectHeroId)
                {
                    heroUIItems[index - start].toggle.group.NotifyToggleOn(heroUIItems[index - start].toggle);
                }
            }
            
            index++;
        }

        heroPageNumber.text = page.ToString() + "/" + totalPage.ToString();
    }

    private void SetTotalPage()
    {
        Dictionary<Int32, HeroManager.HeroInfo> heroList = HeroManager.Instance.GetHeroList();
        totalPage = heroList.Count / 10 + 1;
    }

    public void SelectHero(params object[] objs)
    {
        /*
        HeroTableInfo heroInfo = (HeroTableInfo)objs[0];

        if (selectHeroId != heroInfo.Id)
        {
            selectHeroId = heroInfo.Id;
        }*/
        
        HeroUIItem heroInfo = (HeroUIItem)objs[0];
        LogHelper.LogMsg(heroInfo.name.ToString());
    }

    IEnumerator Init()
    {
        yield return new WaitForEndOfFrame();
        UpdateUI();
    }
}