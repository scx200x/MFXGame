using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum ErrorMsgID
{
    ACCOUNT_EMPTY = 0,
    IP_EMPTY,
    PRRT_EMPTY,
}

public static class ErrorMsg
{
    public static Dictionary<ErrorMsgID, string> ErrorMsgTable;
    
    static ErrorMsg()
    {
        ErrorMsgTable = new FlexibleDictionary<ErrorMsgID, string>();
        ErrorMsgTable[ErrorMsgID.ACCOUNT_EMPTY] = "帐号为空";
        ErrorMsgTable[ErrorMsgID.IP_EMPTY] = "IP地址为空";
        ErrorMsgTable[ErrorMsgID.PRRT_EMPTY] = "端口为空";
    }
}





