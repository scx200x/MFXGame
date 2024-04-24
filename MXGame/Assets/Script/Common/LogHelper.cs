using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LogHelper {
	public static void LogError(object message)
	{
		Debug.LogError(message);
	}

	public static void LogWarning(object message)
	{
		Debug.LogWarning(message);
	}

	public static void LogMsg(object message)
	{
		Debug.Log(message);
	}
}
