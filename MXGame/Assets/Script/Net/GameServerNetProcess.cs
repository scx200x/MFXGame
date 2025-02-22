using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Cs;
using Google.Protobuf;
using UnityEngine;
using UnityEngine.UIElements;

public class GameServerNetProcess : SingltionCreator<GameServerNetProcess>
{
    public enum ConnectionStatus
    {
        None,
        Connecting,
        Connected,
        ErrorVersion,
        ErrorDisconnected,
        ErrorConnectFailed,
        ErrorPassword,
        ErrorAlreadyConnected,
        ErrorBanned,
        ErrorFull
    }
    
    public ZSocket socket;
    
    private const string ipAddress = "1.14.209.47";
    private const int port = 20000;
    private ConnectionStatus connectionStatus = ConnectionStatus.None;
    private string accountID;
    private Thread netThread;
    
    public GameServerNetProcess()
    {
        socket = new ZSocket();

        netThread = new Thread(ConnectGameServer);
        netThread.Start();
    }
    
    public string AccountID
    {
        set
        {
            accountID = value;
        }

        get { return accountID; }
    }

    
    // Update is called once per frame
    public void Update()
    {
        for (ZPackage zpackage = socket.Recv(); zpackage != null; zpackage = socket.Recv())
        {
            EventMsgCenter.SendNetMsg(zpackage.GetServiceID(),zpackage);
        }
    }

    public void CloseConnect()
    {
        socket.Close();
        connectionStatus = ConnectionStatus.None;
    }

    public void ConnectGameServer()
    {
        connectionStatus = ConnectionStatus.Connecting;
        
        if (socket.Connect(ipAddress, port))
        {
            EventMsgCenter.SendMsg(EventName.Connected);
            ZNetPeer peer = new ZNetPeer(socket, true);
            connectionStatus = ConnectionStatus.Connected;
        }
        else
        {
            connectionStatus = ConnectionStatus.None;
        }
        
        LogHelper.LogWarning("ZNet.m_connectionStatus = " + connectionStatus.ToString());
    }

    public void SendAccLoginRequest()
    {
        Cs.AccLoginRequest sr = new Cs.AccLoginRequest()
        {
            AccountId = "FirstAccount",
        };

        byte[] tosend = sr.ToByteArray();
        int serviceId = 1;

        socket.Send(new ZPackage(serviceId, tosend, tosend.Length));
    }

    public void SendRoleLoginRequest(long roleID)
    {
        Cs.RoleLoginRequest sr = new RoleLoginRequest()
        {
            RoleId = roleID,
        };
        
        byte[] tosend = sr.ToByteArray();
        int serviceId = 2;

        socket.Send(new ZPackage(serviceId, tosend, tosend.Length));
    }

    public void SendBeginBattleRequest(Int32 teamID, Int32 mapID)
    {
        Cs.BeginBattleRequest sr = new BeginBattleRequest()
        {
            TeamId = teamID,
            MapId = mapID
        };
        
        byte[] tosend = sr.ToByteArray();
        int serviceId = 3;

        socket.Send(new ZPackage(serviceId, tosend, tosend.Length));
    }

    public void SendEndBattleRequest(Int32 teamID, Int32 mapID, Int32 boxID)
    {
        Cs.EndBattleRequest sr = new EndBattleRequest()
        {
            TeamId = teamID,
            MapId = mapID,
            BoxId = boxID
        };
        
        byte[] tosend = sr.ToByteArray();
        int serviceId = 4;

        socket.Send(new ZPackage(serviceId, tosend, tosend.Length));
    }

    public void SendTeamHeroRequest(Int32 teamID, Int64 instID, Int32 posID)
    {
        Cs.TeamHeroRequest sr = new TeamHeroRequest()
        {
            TeamId = teamID,
            HeroId = instID,
            PosId = posID
        };
        
        byte[] tosend = sr.ToByteArray();
        int serviceId = 5;

        socket.Send(new ZPackage(serviceId, tosend, tosend.Length));
    }

    public ConnectionStatus GetConnectStatus()
    {
        return connectionStatus;
    }

}
