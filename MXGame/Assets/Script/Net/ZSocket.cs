using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.ComponentModel;
using Unity.VisualScripting;
using Google.Protobuf;
using System.IO;

public class ZSocket : IDisposable, ISocket
{
    public ZSocket()
    {
        this.m_socket = ZSocket.CreateSocket();
    }

    public static Socket CreateSocket()
    {
        return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            NoDelay = true
        };
    }
    
    public static IPEndPoint GetEndPoint(string host, int port)
    {
        return new IPEndPoint(Dns.GetHostEntry(host).AddressList[0], port);
    }


    public bool Connect(string host, int port)
    {
        ZLog.Log(string.Concat(new object[]
        {
            "Connecting to ",
            host,
            " : ",
            port
        }));

        if (this.m_socket == null)
        {
            this.m_socket = ZSocket.CreateSocket();
        }

        IPEndPoint endPoint = ZSocket.GetEndPoint(host, port);
        this.m_socket.BeginConnect(endPoint, null, null).AsyncWaitHandle.WaitOne(3000, true);
        
        if (!this.m_socket.Connected)
        {
            return false;
        }
        try
        {
            this.m_endpoint = (this.m_socket.RemoteEndPoint as IPEndPoint);
        }
        catch
        {
            this.Close();
            return false;
        }
        
        this.BeginReceive();

        ZLog.Log(" connected");
        return true;
    }

    private void BeginReceive()
    {
        this.m_socket.BeginReceive(this.m_recvSizeBuffer, 0, this.m_recvSizeBuffer.Length, SocketFlags.None, new AsyncCallback(this.PkgSizeReceived), this.m_socket);
    }

    private void Disconnect()
    {
        if (this.m_socket != null)
        {
            try
            {
                this.m_socket.Disconnect(true);
            }
            catch
            {
            }
        }
    }

    private void PkgSizeReceived(IAsyncResult res)
    {
        int num;
        try
        {
            num = this.m_socket.EndReceive(res);
        }
        catch (Exception)
        {
            this.Disconnect();
            return;
        }
        this.m_totalRecv += num;
        if (num != 6)
        {
            this.Disconnect();
            return;
        }
        Int16 num2 = IPAddress.HostToNetworkOrder( BitConverter.ToInt16(this.m_recvSizeBuffer, 0));
        if (num2 <= 0)
        {
            ZLog.LogError("Invalid pkg size " + num2);
            return;
        }

        Int32 num3 = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(this.m_recvSizeBuffer, 2));
        if (num3 <= 0)
        {
            ZLog.LogError("Invalid pkg size " + num2);
            return;
        }
        this.m_lastRecvPkgSize = num3;
        this.m_recvOffset = 0;

        if (this.m_recvBuffer == null)
        {
            this.m_recvBuffer = new byte[ZSocket.m_maxRecvBuffer];
        }

        this.m_socket.BeginReceive(this.m_recvBuffer, this.m_recvOffset, this.m_lastRecvPkgSize, SocketFlags.None, new AsyncCallback(this.PkgReceived), this.m_socket);
    }

    private void PkgReceived(IAsyncResult res)
    {
        int num;
        try
        {
            num = this.m_socket.EndReceive(res);
        }
        catch (Exception)
        {
            this.Disconnect();
            return;
        }

        this.m_totalRecv += num;
        this.m_recvOffset += num;

        if (this.m_recvOffset < this.m_lastRecvPkgSize)
        {
            int size = this.m_lastRecvPkgSize - this.m_recvOffset;
            if (this.m_recvBuffer == null)
            {
                this.m_recvBuffer = new byte[ZSocket.m_maxRecvBuffer];
            }
            this.m_socket.BeginReceive(this.m_recvBuffer, this.m_recvOffset, size, SocketFlags.None, new AsyncCallback(this.PkgReceived), this.m_socket);
            return;
        }

        Int32 service_id = IPAddress.HostToNetworkOrder(BitConverter.ToInt32(this.m_recvBuffer, 8));
        int startIndex = 18; // 子数组开始的位置（索引）
        int length = this.m_lastRecvPkgSize - 18; // 子数组的长度
        byte[] subArray = new byte[length];
        Array.Copy(this.m_recvBuffer, startIndex, subArray, 0, length);

        ZPackage item = new ZPackage(service_id, subArray, length);

       // Cs.AccLoginResponse sjl = Cs.AccLoginResponse.Parser.ParseFrom(item.GetArray());
       
        this.m_mutex.WaitOne();
        this.m_pkgQueue.Enqueue(item);
        this.m_mutex.ReleaseMutex();
        this.BeginReceive();
    }

    public void Dispose()
    {
        ZLog.Log("Disposing socket");
    }


    public void Close()
    {
        if (this.m_socket != null)
        {
            try
            {
                if (this.m_socket.Connected)
                {
                    this.m_socket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception)
            {
            }
            this.m_socket.Close();
        }

        this.m_socket = null;
        this.m_endpoint = null;
    }

    public bool IsConnected()
    {
        return this.m_socket != null && this.m_socket.Connected;
    }

    public void Send(ZPackage pkg)
    {
        if (pkg.GetArray().Length == 0)
        {
            return;
        }

        if (this.m_socket == null || !this.m_socket.Connected)
        {
            return;
        }

        //以下是协议内容。不需要改动。
        byte[] array = pkg.GetArray();
        short request_type = 1;
        byte[] msg_type_byte = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(request_type));
        short serviceid_type = 2; //2表示service_id
        byte[] serviceid_type_byte = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(serviceid_type));

        byte[] serviceid_val_byte = BitConverter.GetBytes(IPAddress.NetworkToHostOrder(pkg.GetServiceID()));
        short request_data = 4; //4表示这是pb数据
        short request_data_htos = IPAddress.NetworkToHostOrder(request_data);
        Int32 length = IPAddress.NetworkToHostOrder(array.Length + 2 + 4 + 2 + 4);

        var byteslist = new List<byte>();
        byteslist.AddRange(msg_type_byte);
        byteslist.AddRange(BitConverter.GetBytes(length));
        byteslist.AddRange(serviceid_type_byte);
        byteslist.AddRange(serviceid_val_byte);
        byteslist.AddRange(BitConverter.GetBytes(request_data_htos));
        byteslist.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(array.Length)));

        byte[] bytes = byteslist.ToArray();

        this.m_sendMutex.WaitOne();
        if (!this.m_isSending)
        {
            if (array.Length > 10485760)
            {
                ZLog.LogError("Too big data package: " + array.Length);
            }
            try
            {
                this.m_totalSent += bytes.Length;
                this.m_socket.BeginSend(bytes, 0, bytes.Length, SocketFlags.None, new AsyncCallback(this.PkgSent), null);
                this.m_isSending = true;
                this.m_sendQueue.Enqueue(array);
                goto IL_DA;
            }
            catch (Exception arg)
            {
                ZLog.Log("Handled exception in ZSocket:Send:" + arg);
                this.Disconnect();
                goto IL_DA;
            }
        }
        this.m_sendQueue.Enqueue(bytes);
        this.m_sendQueue.Enqueue(array);
    IL_DA:
        this.m_sendMutex.ReleaseMutex();
    }

    private void PkgSent(IAsyncResult res)
    {
        this.m_sendMutex.WaitOne();
        if (this.m_sendQueue.Count > 0 && this.IsConnected())
        {
            byte[] array = this.m_sendQueue.Dequeue();
            try
            {
                this.m_totalSent += array.Length;
                this.m_socket.BeginSend(array, 0, array.Length, SocketFlags.None, new AsyncCallback(this.PkgSent), null);
                goto IL_86;
            }
            catch (Exception arg)
            {
                ZLog.Log("Handled exception in pkgsent:" + arg);
                this.m_isSending = false;
                this.Disconnect();
                goto IL_86;
            }
        }
        this.m_isSending = false;
    IL_86:
        this.m_sendMutex.ReleaseMutex();
    }

    public ZPackage Recv()
    {
        if (this.m_socket == null)
        {
            return null;
        }
        if (this.m_pkgQueue.Count == 0)
        {
            return null;
        }

        ZPackage result = null;
        this.m_mutex.WaitOne();

        if (this.m_pkgQueue.Count > 0)
        {
            result = this.m_pkgQueue.Dequeue();
        }

        this.m_mutex.ReleaseMutex();
        return result;
    }

    public string GetEndPointString()
    {
        if (this.m_endpoint != null)
        {
            return this.m_endpoint.ToString();
        }

        return "None";
    }

    public string GetHostName()
    {
        if (this.m_endpoint != null)
        {
            return this.m_endpoint.Address.ToString();
        }

        return "None";
    }

    public Queue<ZPackage> GetNetQueue()
    {
        return m_pkgQueue;
    }
    
    private Socket m_socket;

    private Mutex m_mutex = new Mutex();

    private Mutex m_sendMutex = new Mutex();

    private static int m_maxRecvBuffer = 10485760;

    private byte[] m_recvBuffer;

    private byte[] m_recvSizeBuffer = new byte[6];

    private Queue<ZPackage> m_pkgQueue = new Queue<ZPackage>();

    private bool m_isSending;

    private IPEndPoint m_endpoint;

    private int m_totalSent;

    private int m_totalRecv;

    private int m_recvOffset;

    private int m_lastRecvPkgSize;

    private Queue<byte[]> m_sendQueue = new Queue<byte[]>();
}
