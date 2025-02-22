using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Security.Cryptography;
using Unity.Mathematics;
using UnityEngine;


public static class Utility
{
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
    
    public static long GetCurrentTimeMiniseconds()
    {
        return System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
    }
    
    [DllImport("user32.dll")]
    public static extern IntPtr GetForegroundWindow();
    
    [DllImport("user32.dll")]
    public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
    
    [DllImport("user32.dll")]
    public static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);
    
    [DllImport("user32.dll")]
    public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


    public static void SetResolution(int index)
    {
        RECT pRect = new RECT();
        GetWindowRect(GetForegroundWindow(), ref pRect);

        RECT pClientRect = new RECT();
        GetClientRect(GetForegroundWindow(), ref pClientRect);
        
        int[] Widths = new int[] { 1920, 1600, 1366, 1280 };
        int[] Heights = new int[] { 1080, 900, 768, 720 };

        if (index == 4)
        {
            Screen.SetResolution(Widths[0],Heights[0],true);
        }
        else
        {
#if UNITY_STANDALONE_WIN
            int edgeWidth = (pRect.Right - pRect.Left) - pClientRect.Right;
            int edgeHeight = (pRect.Bottom - pRect.Top) - pClientRect.Bottom;
            Vector2Int center = Vector2Int.FloorToInt(new Vector2((pRect.Right - pRect.Left) / 2.0f + pRect.Left - (Widths[index] + edgeWidth) / 2.0f,(pRect.Bottom - pRect.Top) / 2.0f + pRect.Top - (Heights[index] + edgeHeight) / 2.0f));
            DisplayInfo displayInfo = new DisplayInfo();
            displayInfo.width = Widths[index];
            displayInfo.height = Heights[index];
            Screen.SetResolution(Widths[index],Heights[index],false);
            Screen.MoveMainWindowTo(displayInfo, center);
#else
            Screen.SetResolution(Widths[index],Heights[index],false);
#endif

        }
    }

    public static string GetNumberToString(int value)
    {
        string vs = "";
        int index = 0;
        
        while (value > 0)
        {
            int v1 = (value % 1000) * (int)(Math.Pow(1000, index));

            if (index > 0)
            {
                vs = v1 + "," + vs;
            }
            else
            {
                vs = v1 + vs;
            }
            
            value = value - v1;
        }

        return vs;
    }

    public static string GetGameTime(Int32 seconds)
    {
        if (seconds < 0)
        {
            return "0天0时0分";
        }

        int days = seconds / (24 * 3600);  // 计算天数
        seconds %= (24 * 3600);           // 剩余的秒数
        int hours = seconds / 3600;          // 计算小时数
        seconds %= 3600;                   // 剩余的秒数
        int minutes = seconds / 60;          // 计算分钟数
        
        return $"{days}天{hours}时{minutes}分";
    }

    public static string GetRealGameTime(Int32 seconds)
    {
        if (seconds < 0)
        {
            return "";
        }

        int realDays = seconds / 60;
        int years = realDays / 365;
        realDays -= years * 365;
        
        return $"{years}年{realDays}天";
    }
}