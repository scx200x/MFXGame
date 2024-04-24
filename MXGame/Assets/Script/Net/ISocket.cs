using System;

public interface ISocket
{
    bool IsConnected();

    void Send(ZPackage pkg);

    ZPackage Recv();

    void Dispose();

    void Close();

    string GetEndPointString();
 
    string GetHostName();
}
