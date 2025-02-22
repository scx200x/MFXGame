using System;
using System.Collections;
using System.Collections.Generic;

public class GameDataManager : SingltionCreator<GameDataManager>
{
    public class AccountInfo
    {
        public Int64 roldID;
        public string name;
        public Int32 gameTime;
        public Int32 difficult;
        public bool isUse;
    }
    
    
    private List<AccountInfo> AccountInfos;
    private string accountID;

    public GameDataManager()
    {
        AccountInfos = new List<AccountInfo>();
    }

    public string AccountID
    {
        set { accountID = value; }
        get { return accountID; }
    }

    public void ClearAccounts()
    {
        AccountInfos.Clear();
    }

    public void AddAccount(long RoldID, string Name, int GameTime, int Difficult)
    {
        AccountInfo accountInfo = new AccountInfo();

        accountInfo.roldID = RoldID;
        accountInfo.name = Name;
        accountInfo.gameTime = GameTime;
        accountInfo.difficult = Difficult;
        accountInfo.isUse = false;
        
        AccountInfos.Add(accountInfo);
    }

    public void SetCurrentRole(long RoleID)
    {
        foreach (var accountInfo in AccountInfos)
        {
            if (accountInfo.roldID == RoleID)
            {
                accountInfo.isUse = false;
            }
        }
    }
}