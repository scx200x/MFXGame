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
}