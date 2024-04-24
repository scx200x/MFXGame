using Google.Protobuf;
using System;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using Object = UnityEngine.Object;


public class ZPackage
{
    public ZPackage()
    {
        this.m_writer = new BinaryWriter(this.m_stream);
        this.m_reader = new BinaryReader(this.m_stream);
    }

    public ZPackage(int service_id, byte[] data, int length)
    {
        this.m_writer = new BinaryWriter(this.m_stream);
        this.m_reader = new BinaryReader(this.m_stream);
        this.m_stream.Write(data, 0, length);
        this.service_id = service_id;
    }

    public int GetServiceID() { return this.service_id; }

    public byte[] GetArray()
    {
        this.m_writer.Flush();
        this.m_stream.Flush();
        return this.m_stream.ToArray();
        /*Cs.AccLoginRequest sr = new Cs.AccLoginRequest()
        {
            AccountId = "sjl",
        };

        return sr.ToByteArray();*/
    }

    private MemoryStream m_stream = new MemoryStream();

    private BinaryWriter m_writer;

    private BinaryReader m_reader;

    private int service_id = 0;
}
