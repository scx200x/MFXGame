using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
    
    public GameServerNetProcess()
    {
        socket = new ZSocket();
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
        Queue<ZPackage> netQueue = socket.GetNetQueue();

        while (netQueue.Count > 0)
        {
            ZPackage package = netQueue.Dequeue();
            
            EventMsgCenter.SendNetMsg(package.GetServiceID(),package);
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
            ZNetPeer peer = new ZNetPeer(socket, true);
            connectionStatus = ConnectionStatus.Connected;

            SendAccLoginRequest();
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

    public ConnectionStatus GetConnectStatus()
    {
        return connectionStatus;
    }

}
