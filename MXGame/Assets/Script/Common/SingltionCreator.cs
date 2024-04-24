using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingltionCreator<T> where T : class,new() {
	private static T instance;

	public static T Instance
	{
		get
		{
			if(instance == null)
			{
				instance = SingltionManager.Create(typeof(T).FullName) as T;
			}

			return instance;
		}
	}

	public static void Release()
	{
		instance = null;
	}
}

public class SingltionCreateTinyUpdate<T> : SingltionCreator<T> where T : class,new()
{
	protected float minExeDeltaTime = 1f;
	protected float deltaTime;

	public virtual bool OnUpdate()
	{
		return true;
	}
}

public class SingltionManager
{
	static Dictionary<string,object> SingltionList = new Dictionary<string, object>();

	public static object Create(string objType)
	{
		object singltionOBJ = null;

		if(!SingltionList.ContainsKey(objType))
		{
			singltionOBJ = System.Activator.CreateInstance(System.Type.GetType(objType));
			SingltionList.Add(objType,singltionOBJ);
		}

		return singltionOBJ;
	}

	public static void Release(string objType)
	{
		if(SingltionList.ContainsKey(objType))
		{
			SingltionList.Remove(objType);
		}
	}
}
