using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolTimeObjectItem
{
	public long lastUseTime;
	public long instanceID;
	public Object Object;
}

public abstract class BasePoolObject
{
	protected int reserveNum;

	public abstract void AddObject(Object obj);
	public abstract int GetNum();
	public abstract void OnUpdate();
	public abstract void Release(int count);
	public abstract void Clean();
}

public class PoolObject : BasePoolObject{
	public List<Object> poolObjectList;

	public PoolObject(int reserverNum)
	{
		this.reserveNum = reserverNum;
	}


	public override void AddObject(Object obj)
	{
		if(poolObjectList == null)
		{
			poolObjectList = ListPool<Object>.GetList();
		}

		if(obj != null)
		{
			poolObjectList.Add(obj);
		}
	}

	public Object GetObject()
	{
		if(poolObjectList != null)
		{
			if(poolObjectList.Count > 0)
			{
				Object obj = poolObjectList[poolObjectList.Count -1];
				poolObjectList.RemoveAt(poolObjectList.Count -1);

				return obj;
			}
		}

		return null;
	}

	public override int GetNum()
	{
		if(poolObjectList != null) return poolObjectList.Count;

		return 0;
	}

	public override void OnUpdate()
	{
		if(poolObjectList != null)
		{
			for(int i=poolObjectList.Count-1;i>=0;i--)
			{
				if(poolObjectList[i] == null)
				{
					poolObjectList.RemoveAt(i);
				}
			}

			int num = GetNum();

			if(num > reserveNum)
			{
				int releaseNum = num - reserveNum;
				Release(releaseNum);
			}

			if(poolObjectList.Count <=0) ListPool<Object>.Release(ref poolObjectList);
		}
	}

	public override void Release(int count)
	{
		while(poolObjectList.Count > 0 && count >0)
		{
			int index = poolObjectList.Count -1;
			ResourcesManager.Destroy(poolObjectList[index]);
			poolObjectList.RemoveAt(index);
			count--;
		}
	}

	public override void Clean()
	{
		if(poolObjectList != null)
		{
			ListPool<Object>.Release(ref poolObjectList);
		}
	}

}

public class PoolTimeObject : BasePoolObject
{
	Dictionary<long,PoolTimeObjectItem> poolTimeObjectItemDict;

	public PoolTimeObject(int reserverNum)
	{
		poolTimeObjectItemDict = new Dictionary<long, PoolTimeObjectItem>();
		this.reserveNum = reserverNum;
	}

	public override void AddObject(Object obj)
	{
		if(poolTimeObjectItemDict.ContainsKey(obj.GetInstanceID()))
		{
			if(obj != null)
			{
				PoolTimeObjectItem poolTimeObjectItem = new PoolTimeObjectItem();
				poolTimeObjectItem.lastUseTime = Utility.GetCurrentTimeMiniseconds();
				poolTimeObjectItem.Object = obj;
				poolTimeObjectItem.instanceID = obj.GetInstanceID();
				poolTimeObjectItemDict.Add(obj.GetInstanceID(),poolTimeObjectItem);
			}
		}
	}

	public Object GetObject(long id)
	{
		if(poolTimeObjectItemDict != null)
		{
			PoolTimeObjectItem poolTimeObjectItem;
			if(poolTimeObjectItemDict.TryGetValue(id,out poolTimeObjectItem))
			{
				poolTimeObjectItem.lastUseTime = Utility.GetCurrentTimeMiniseconds();
				return poolTimeObjectItem.Object;
			}
		}

		return null;
	}

	public override int GetNum()
	{
		if(poolTimeObjectItemDict != null)
		return poolTimeObjectItemDict.Count;

		return 0;
	}

	public override void OnUpdate()
	{
		if(poolTimeObjectItemDict != null)
		{
			int num = GetNum();

			if(num > reserveNum)
			{
				int releaseNum = num - reserveNum;
				Release(releaseNum);
			}
		}
	}

	public override void Release(int count)
	{
		List<PoolTimeObjectItem> items = ListPool<PoolTimeObjectItem>.GetList();

		Dictionary<long,PoolTimeObjectItem>.Enumerator itor = poolTimeObjectItemDict.GetEnumerator();

		while(itor.MoveNext())
		{
			items.Add(itor.Current.Value);
		}

		items.Sort(Compare);

		for(int i=items.Count-1;i>=0;i--)
		{
			ResourcesManager.Destroy(items[i].Object);
			poolTimeObjectItemDict.Remove(items[i].instanceID);
			items[i].Object = null;
			count--;
		}

		ListPool<PoolTimeObjectItem>.Release(ref items);
	}

	private int Compare(PoolTimeObjectItem itme1,PoolTimeObjectItem item2)
	{
		if(itme1.lastUseTime > item2.lastUseTime) return 1;
		else if(itme1.lastUseTime < item2.lastUseTime) return -1;

		return 0;
	}

	public override void Clean()
	{
		Dictionary<long,PoolTimeObjectItem>.Enumerator itor = poolTimeObjectItemDict.GetEnumerator();

		while(itor.MoveNext())
		{
			ResourcesManager.Destroy(itor.Current.Value.Object);
		}

		poolTimeObjectItemDict.Clear();
	}
}
