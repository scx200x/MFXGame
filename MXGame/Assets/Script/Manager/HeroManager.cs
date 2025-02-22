using System;
using System.Collections;
using System.Collections.Generic;
using Cs;

public class HeroManager : SingltionCreator<HeroManager>,IManagerBaseInterface
{
    public HeroManager()
    {
        heroList = new Dictionary<int, HeroInfo>();
        fightteamList = new Dictionary<int, HeroTeam>();
        
        EventMsgCenter.RegisterNetMsg(NetEventName.TeamChange,OnNetTeamChangeResp,false);
        EventMsgCenter.RegisterGlobalMsg(EventName.GameOver,OnDisable,false);
    }

    public void OnDisable(params object[] package)
    {
        EventMsgCenter.UnRegisterNetMsg(NetEventName.TeamChange,OnNetTeamChangeResp);
        
        Clear();
    }

    public class PropertyItem
    {
        public Int32 propertyId;
        public Int32 propertyValue;
    }
    
    public class HeroInfo
    {
        public Int32 heroID;
        public Int64 heroInstID;
        public Int32 level;
        public List<Int32> equipIds;
        public List<PropertyItem> PropertyList;
    }

    public class HeroTeam
    {
        public Int32 teamID;
        public List<Int64> heroList;
    }

    private Dictionary<Int32,HeroInfo> heroList;
    private Dictionary<Int32,HeroTeam> fightteamList;

    public void Clear()
    {
        heroList.Clear();
        fightteamList.Clear();
    }

    public void AddHeroInfo(Cs.HeroInfo heroInfo)
    {
        HeroInfo hero = new HeroInfo();
        hero.heroID = heroInfo.HeroId;
        hero.heroInstID = heroInfo.InstId;
        hero.level = heroInfo.Level;
        hero.equipIds = new List<int>();
        hero.PropertyList = new List<PropertyItem>();

        for (int i = 0; i < heroInfo.EquipList.Count; i++)
        {
            hero.equipIds.Add(heroInfo.EquipList[i]);
        }

        for (int i = 0; i < heroInfo.ProtList.Count; i++)
        {
            PropertyItem propertyItem = new PropertyItem();
            propertyItem.propertyId = heroInfo.ProtList[i].PropId;
            propertyItem.propertyValue = heroInfo.ProtList[i].PropVal;
            hero.PropertyList.Add(propertyItem);
        }
        
        heroList.Add(hero.heroID,hero);
    }
    
    public void AddTeam(int teamID, FightHeroTeamInfo teamInfo)
    {
        HeroTeam heroTeam = new HeroTeam();
        heroTeam.teamID = teamID;
        heroTeam.heroList = new List<long>();

        foreach (var heroID in teamInfo.HeroList)
        {
            heroTeam.heroList.Add(heroID);
        }
        
        fightteamList.Add(teamID,heroTeam);
    }

    public void OnNetTeamChangeResp(params object[] package)
    {
        ZPackage Package = (ZPackage)(package[0]);
        
        Cs.TeamHeroResponse thr = Cs.TeamHeroResponse.Parser.ParseFrom(Package.GetArray());

        if (fightteamList.ContainsKey(thr.TeamId))
        {
            fightteamList[thr.TeamId].heroList.Clear();

            foreach (var heroID in thr.HeroId)
            {
                fightteamList[thr.TeamId].heroList.Add(heroID);
            }
        }
    }
}