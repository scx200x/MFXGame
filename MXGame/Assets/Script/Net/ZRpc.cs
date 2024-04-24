using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using UnityEngine;


public class ZRpc : IDisposable
{
    public ZRpc(ISocket socket)
    {
        this.m_socket = socket;
    }

    public void Dispose()
    {
        this.m_socket.Dispose();
    }

    public ISocket GetSocket()
    {
        return this.m_socket;
    }

    public bool Update(float dt)
    {
        if (!this.m_socket.IsConnected())
        {
            return false;
        }
        for (ZPackage zpackage = this.m_socket.Recv(); zpackage != null; zpackage = this.m_socket.Recv())
        {
            this.m_recvPackages++;
            //this.m_recvData += zpackage.Size();
            try
            {
                this.HandlePackage(zpackage);
            }
            catch (Exception arg)
            {
                ZLog.Log("Exception in ZRpc::HandlePackage: " + arg);
            }
        }
        this.UpdatePing(dt);
        return true;
    }

    private void UpdatePing(float dt)
    {
        this.m_pingTimer += dt;
        if (this.m_pingTimer > ZRpc.m_pingInterval)
        {
            this.m_pingTimer = 0f;
            //this.m_pkg.Clear();
            //this.m_pkg.Write(0);
            //this.m_pkg.Write(true);
            //this.SendPackage(this.m_pkg);
        }
        this.m_timeSinceLastPing += dt;
        if (this.m_timeSinceLastPing > ZRpc.m_timeout)
        {
            ZLog.LogWarning("ZRpc timeout detected");
            this.m_socket.Close();
        }
    }

    private void ReceivePing(ZPackage package)
    {
        //if (package.ReadBool())
        //{
            //this.m_pkg.Clear();
            //this.m_pkg.Write(0);
            //this.m_pkg.Write(false);
            //this.SendPackage(this.m_pkg);
        //    return;
        //}
        this.m_timeSinceLastPing = 0f;
    }

    public float GetTimeSinceLastPing()
    {
        return this.m_timeSinceLastPing;
    }

    public bool IsConnected()
    {
        return this.m_socket.IsConnected();
    }

    private void HandlePackage(ZPackage package)
    {
        int service_id = package.GetServiceID();
        if (service_id == 0)
        {
            //this.ReceivePing(package);
            return;
        }

        Cs.AccLoginResponse sjl = Cs.AccLoginResponse.Parser.ParseFrom(package.GetArray());
    }

    private void SendPackage(ZPackage pkg)
    {
        this.m_sentPackages++;
        //this.m_sentData += pkg.Size();
        this.m_socket.Send(this.m_pkg);
    }
    private ISocket m_socket;

    private ZPackage m_pkg = new ZPackage();

    private int m_sentPackages;

    private int m_sentData;

    private int m_recvPackages;

    private int m_recvData;

    private float m_pingTimer;

    private float m_timeSinceLastPing;

    private static float m_pingInterval = 1f;

    private static float m_timeout = 30f;
}
