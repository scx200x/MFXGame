using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPool : SingltionCreateTinyUpdate<ObjectPool>{
	Dictionary<int,PoolObject> poolDict = new Dictionary<int, PoolObject>();
	
	public Dictionary<int,PoolObject> PoolDict
	{
		get
		{
			return poolDict;
		}
	}
	public override bool OnUpdate()
	{
		this.deltaTime += Time.deltaTime;

		if(this.deltaTime >= minExeDeltaTime)
		{
			base.OnUpdate();

			if(poolDict.Count > 0)
			{
				Dictionary<int,PoolObject>.Enumerator itor = poolDict.GetEnumerator();

				while(itor.MoveNext())
				{
					PoolObject current = itor.Current.Value;
					current.OnUpdate();
				}
			}

			this.deltaTime -= minExeDeltaTime;
		}

		return true;
	}

	public void InitPool(ResourceInfo info,int reserveNum,int cloneNum = 0)
	{
		if(!PoolDict.ContainsKey(info.GetInstanceID()))
		{
			int gid = info.GetInstanceID();

			PoolObject poolObject = new PoolObject(reserveNum);
			PoolDict[gid] = poolObject;

			if(cloneNum > 0 && info.LoadObj != null)
			{
				for(int i=0;i<cloneNum;i++)
				{
					GameObject go = info.CloneGameObject(Vector3.zero,Quaternion.identity);
					go.SetActive(false);
					poolObject.AddObject(go);
				}
			}
		}
	}

	public GameObject Spawn(ResourceInfo info,Vector3 pos,Quaternion quat,bool bActive)
	{
		PoolObject poolObject;

		if(PoolDict.TryGetValue(info.GetInstanceID(),out poolObject))
		{
			GameObject obj = poolObject.GetObject() as GameObject;

			if(obj == null)
			{
				obj = info.CloneGameObject(pos,quat);
			}

			obj.transform.position = pos;
			obj.transform.rotation = quat;
			obj.SetActive(bActive);
			return obj;
		}

		return null;
	}

	public void Recycle(ResourceInfo info,ref GameObject obj)
	{
		PoolObject PoolObject;

		if(obj != null)
		{
			if(PoolDict.TryGetValue(info.GetInstanceID(),out PoolObject))
			{
				obj.transform.parent = null;
				obj.SetActive(false);
				PoolObject.AddObject(obj);
			}
			else
			{
				ResourcesManager.Destroy(obj);
			}

			obj = null;
		}
	}

	public void Clean()
	{
		if(poolDict != null)
		{
			PoolObject poolObject = null;
			Dictionary<int,PoolObject>.Enumerator itor = poolDict.GetEnumerator();
			while(itor.MoveNext())
			{
				poolObject = itor.Current.Value;
				poolObject.Clean();
			}
		}
	}

}
