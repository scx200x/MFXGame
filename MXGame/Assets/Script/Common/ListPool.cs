using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListPool<T> {
	public static int MaxPoolSize = 10;

	static List<List<T>> PoolLists = new List<List<T>>(MaxPoolSize);
	
	public static List<T> GetList()
	{
		lock(PoolLists)
		{
			int count = PoolLists.Count;

			if(count > 0)
			{
				List<T> pool = PoolLists[PoolLists.Count -1];
				PoolLists.RemoveAt(PoolLists.Count -1);
				return pool;
			}
			else
			{
				return new List<T>(20);
			}
		}
	}

	public static void Release(ref List<T> pool)
	{
		if(pool != null)
		{
			lock(PoolLists)
			{
				List<T> objList = pool;
				pool = null;

				if(!PoolLists.Contains(objList))
				{
					objList.Clear();

					int count = PoolLists.Count;

					if(count < MaxPoolSize)
					{
						PoolLists.Add(objList);
					}
				}
			}
		}
	}
}
