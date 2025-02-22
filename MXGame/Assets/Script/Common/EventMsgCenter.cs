using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventName
{
	None = 0,
	LoadingOver = 1,
	GameOver = 2,
	Connected = 3,
}

public enum NetEventName
{
	AccountLogin = 1,
	RoleLogin = 2,
	EndBattle = 4,
	TeamChange = 5,
}

public delegate void BoardcastCallBack(params object[] objs);

public struct EventNameComparer : IEqualityComparer<EventName>
{
	public bool Equals(EventName a,EventName b)
	{
		return a == b;
	}

	public int GetHashCode(EventName obj)
	{
		return (int)obj;
	}
}

public class BaseMsg<T,K,G> : SingltionCreator<K>
where K : class,new()
where G : IEqualityComparer<T>,new()
{
	private Dictionary<T,LinkedList<BoardcastCallBack>> globalMsgTable = new Dictionary<T, LinkedList<BoardcastCallBack>>(new G());
	private Dictionary<T,LinkedList<BoardcastCallBack>> stageMsgTable = new Dictionary<T, LinkedList<BoardcastCallBack>>(new G());
	private Dictionary<NetEventName,LinkedList<BoardcastCallBack>> netMsgTable = new Dictionary<NetEventName, LinkedList<BoardcastCallBack>>();

	protected void BaseRegisterGlobalMsg(T type,BoardcastCallBack callBack,bool addFirst = false)
	{
		RegisterMsg(globalMsgTable,type,callBack,addFirst);
	}

	protected void BaseRegisterStageMsg(T type,BoardcastCallBack callBack,bool addFirst = false)
	{
		RegisterMsg(stageMsgTable,type,callBack,addFirst);
	}

	protected void BaseRegisterNetMsg(NetEventName type, BoardcastCallBack callBack, bool addFirst = false)
	{
		RegisterNetMsg(netMsgTable,type,callBack,addFirst);
	}

	protected void BaseUnRegisterMsg(T type,BoardcastCallBack callBack)
	{
		UnRegisterMsg(globalMsgTable,type,callBack);
		UnRegisterMsg(stageMsgTable,type,callBack);
	}
	
	protected void BaseUnRegisterNetMsg(NetEventName type,BoardcastCallBack callBack)
	{
		UnRegisterNetMsg(netMsgTable,type,callBack);
	}

	protected void BaseUnRegisterAllGlobalMsg()
	{
		Dictionary<T,LinkedList<BoardcastCallBack>>.Enumerator itor = globalMsgTable.GetEnumerator();

		while(itor.MoveNext())
		{
			itor.Current.Value.Clear();
		}

		globalMsgTable.Clear();
	}

	protected void BaseUnRegisterAllStageMsg()
	{
		Dictionary<T,LinkedList<BoardcastCallBack>>.Enumerator itor = stageMsgTable.GetEnumerator();

		while(itor.MoveNext())
		{
			itor.Current.Value.Clear();
		}

		stageMsgTable.Clear();

		BaseUnRegisterAllNetMsg();
	}
	
	protected void BaseUnRegisterAllNetMsg()
	{
		Dictionary<NetEventName,LinkedList<BoardcastCallBack>>.Enumerator itor = netMsgTable.GetEnumerator();

		while(itor.MoveNext())
		{
			itor.Current.Value.Clear();
		}

		netMsgTable.Clear();
	}

	protected void BaseSendMsg(T type,params object[] objs)
	{
		SendMsg(globalMsgTable,type,objs);
		SendMsg(stageMsgTable,type,objs);
	}

	protected void BaseSendNetMsg(int type, params object[] objs)
	{
		SendNetMsg(netMsgTable,type,objs);
	}

	private static void RegisterNetMsg(Dictionary<NetEventName, LinkedList<BoardcastCallBack>> table, NetEventName type,
		BoardcastCallBack callBack, bool addFirst = false)
	{
		LinkedList<BoardcastCallBack> list;

		if(!table.TryGetValue(type,out list))
		{
			list = new LinkedList<BoardcastCallBack>();
			table.Add(type,list);
		}

		if(!list.Contains(callBack))
		{
			if(addFirst && list.Count > 0)
			{
				list.AddFirst(callBack);
			}
			else
			{
				list.AddLast(callBack);
			}
		}
	}

	private static void RegisterMsg(Dictionary<T,LinkedList<BoardcastCallBack>> table,T type,BoardcastCallBack callBack,bool addFirst = false)
	{
		LinkedList<BoardcastCallBack> list;

		if(!table.TryGetValue(type,out list))
		{
			list = new LinkedList<BoardcastCallBack>();
			table.Add(type,list);
		}

		if(!list.Contains(callBack))
		{
			if(addFirst && list.Count > 0)
			{
				list.AddFirst(callBack);
			}
			else
			{
				list.AddLast(callBack);
			}
		}
	}

	private static void UnRegisterMsg(Dictionary<T,LinkedList<BoardcastCallBack>> table,T type,BoardcastCallBack callback)
	{
		LinkedList<BoardcastCallBack> list;
		if(table.TryGetValue(type,out list))
		{
			list.Remove(callback);

			if(list.Count <= 0) table.Remove(type);
		}
	}
	
	private static void UnRegisterNetMsg(Dictionary<NetEventName,LinkedList<BoardcastCallBack>> table,NetEventName type,BoardcastCallBack callback)
	{
		LinkedList<BoardcastCallBack> list;
		if(table.TryGetValue(type,out list))
		{
			list.Remove(callback);

			if(list.Count <= 0) table.Remove(type);
		}
	}


	private static void SendMsg(Dictionary<T,LinkedList<BoardcastCallBack>> table,T type,params object[] objs)
	{
		LinkedList<BoardcastCallBack> list;

		if(table.TryGetValue(type,out list))
		{
			List<BoardcastCallBack> callBacks = ListPool<BoardcastCallBack>.GetList();
			callBacks.AddRange(list);

			for(int i=0;i<callBacks.Count;i++)
			{
				if(callBacks[i] != null)
				{
					callBacks[i](objs);
				}
			}

			ListPool<BoardcastCallBack>.Release(ref callBacks);
		}
	}
	
	private static void SendNetMsg(Dictionary<NetEventName,LinkedList<BoardcastCallBack>> table,int type,params object[] objs)
	{
		LinkedList<BoardcastCallBack> list;

		if(table.TryGetValue((NetEventName)type,out list))
		{
			List<BoardcastCallBack> callBacks = ListPool<BoardcastCallBack>.GetList();
			callBacks.AddRange(list);

			for(int i=0;i<callBacks.Count;i++)
			{
				if(callBacks[i] != null)
				{
					callBacks[i](objs);
				}
			}

			ListPool<BoardcastCallBack>.Release(ref callBacks);
		}
	}
}

public class EventMsgCenter :  BaseMsg<EventName,EventMsgCenter,EventNameComparer>{

	private static readonly Object[] emptyParams = new Object[] {};

	public static void SendMsg(EventName type,params object[] objs)
	{
		EventMsgCenter.Instance.BaseSendMsg(type,objs);
	}

	public static void SendMsg(EventName type)
	{
		EventMsgCenter.Instance.BaseSendMsg(type,emptyParams);
	}
	
	public static void SendNetMsg(int type,params object[] objs)
	{
		EventMsgCenter.Instance.BaseSendNetMsg(type,objs);
	}

	public static void RegisterGlobalMsg(EventName type,BoardcastCallBack callBack,bool addFirst = false)
	{
		EventMsgCenter.Instance.BaseRegisterGlobalMsg(type,callBack,addFirst);
	}

	public static void RegisterStageMsg(EventName type,BoardcastCallBack callBack,bool addFirst = false)
	{
		EventMsgCenter.Instance.BaseRegisterStageMsg(type,callBack,addFirst);
	}

	public static void RegisterNetMsg(NetEventName type, BoardcastCallBack callBack, bool addFirst = false)
	{
		EventMsgCenter.Instance.BaseRegisterNetMsg(type,callBack,addFirst);
	}

	public static void UnRegisterMsg(EventName type,BoardcastCallBack callBack)
	{
		EventMsgCenter.Instance.BaseUnRegisterMsg(type,callBack);
	}
	
	public static void UnRegisterNetMsg(NetEventName type,BoardcastCallBack callBack)
	{
		EventMsgCenter.Instance.BaseUnRegisterNetMsg(type,callBack);
	}

	public static void UnRegiserAllGlobalMsg()
	{
		EventMsgCenter.Instance.BaseUnRegisterAllGlobalMsg();
	}

	public static void UnRegiserAllStageMsg()
	{
		EventMsgCenter.Instance.BaseUnRegisterAllStageMsg();
	}
	
	public static void UnRegiserAllNetMsg()
	{
		EventMsgCenter.Instance.BaseUnRegisterAllNetMsg();
	}

}
