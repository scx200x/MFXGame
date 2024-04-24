using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ResourceInfo
{
    public LoadInfo loadInfo;
    public UnityEngine.Object LoadObj;
    public bool DontDestory;

    List<System.WeakReference> ReferenceList;

    public GameObject CloneGameObject(Vector3 pos,Quaternion quat)
    {
        return CloneObject(pos,quat) as GameObject;
    }

    public UnityEngine.Object CloneObject(Vector3 pos,Quaternion quat)
    {
        if(LoadObj != null)
        {
            UnityEngine.Object obj = UnityEngine.Object.Instantiate(LoadObj,pos,quat);
            AddReference(obj);
            return obj;
        }

        return null;
    }

    public void AddReference(UnityEngine.Object obj)
    {
        if(ReferenceList == null)
        {
            ReferenceList = new List<System.WeakReference>();
        }

        ReferenceList.Add(new System.WeakReference(obj));
    }

    public void ReleaseReference()
    {
        if(ReferenceList != null)
        {
            for(int i=ReferenceList.Count-1;i>=0;i--)
            {
                if(ReferenceList[i].Target != null)
                {
                    ResourcesManager.Destroy((UnityEngine.Object)ReferenceList[i].Target);
                }
            }

            ReferenceList.Clear();
        }
    }

    public void ReleaseReference(UnityEngine.Object obj)
    {
        if(ReferenceList != null)
        {
            for(int i=0;i<ReferenceList.Count;i++)
            {
                if((UnityEngine.Object)ReferenceList[i].Target == obj)
                {
                    ResourcesManager.Destroy((UnityEngine.Object)ReferenceList[i].Target);
                    ReferenceList.RemoveAt(i);
                    return;
                }
            }
        }
    }

    public int GetInstanceID()
    {
        if(LoadObj != null)
        {
            return LoadObj.GetInstanceID();
        }

        return 0;
    }
}

public class LoadInfo
{
    public string FileName;
    public string FilePath;
}

public class AsycLoader
{
    internal class LoaderData
    {
        public string Path;
        public ResourceRequest ResourceRequest;

        public LoaderData(string path)
        {
            this.Path = path;
            ResourceRequest = null;
        }
    }

    private int MaxSameTimeLoaderCount = 10; //max loading number

    private float UpdateDeltaTime = 1f;
    private float CurrentTime;
    private List<string> RemoveLoaderList = new List<string>();
    private Dictionary<string,LoaderData> LoaderDict = new Dictionary<string, LoaderData>();

    public void Init()
    {
        LoaderDict.Clear();
    }

    public void OnUpdate()
    {
        CurrentTime += Time.deltaTime;

        if(CurrentTime >= UpdateDeltaTime)
        {
            CurrentTime -= UpdateDeltaTime;

            Dictionary<string,LoaderData>.Enumerator itor = LoaderDict.GetEnumerator();
            RemoveLoaderList.Clear();
            int loadingCount = 0;

            while(itor.MoveNext())
            {
                if(itor.Current.Value.ResourceRequest != null)
                {
                    if(itor.Current.Value.ResourceRequest.isDone)
                    {
                        ResourcesManager.Instance.SetAsset(itor.Current.Key,itor.Current.Value.ResourceRequest.asset);
                        RemoveLoaderList.Add(itor.Current.Key);
                    }
                    else
                    {
                        loadingCount++;
                    }
                }
            }

            for(int i=0;i<RemoveLoaderList.Count;i++)
            {
                LoaderDict.Remove(RemoveLoaderList[i]);
            }

            itor = LoaderDict.GetEnumerator();

            while(itor.MoveNext() && loadingCount < MaxSameTimeLoaderCount)
            {
                if(itor.Current.Value.ResourceRequest == null)
                {
                    itor.Current.Value.ResourceRequest = Resources.LoadAsync(itor.Current.Value.Path);
                    loadingCount++;
                }
            }
        }
    }

    public void Load(LoadInfo loadInfo)
    {
        if(!LoaderDict.ContainsKey(loadInfo.FilePath))
        {
            LoaderDict[loadInfo.FilePath] = new LoaderData(loadInfo.FilePath);
        }           
    }

    public bool IsAnyLoading()
    {
        return LoaderDict.Count > 0;
    }

    public void StopLoading(string filepath)
    {
        LoaderData loaderData;

        if(LoaderDict.TryGetValue(filepath,out loaderData))
        {
            if(loaderData.ResourceRequest != null)
            {
                if(loaderData.ResourceRequest.isDone)
                {
                    Resources.UnloadAsset(loaderData.ResourceRequest.asset);
                }
            }

            LoaderDict.Remove(filepath);
        }
    }

    public void StopAll()
    {
        Dictionary<string,LoaderData>.Enumerator itor = LoaderDict.GetEnumerator();

        while(itor.MoveNext())
        {
            if(itor.Current.Value.ResourceRequest != null)
            {
                if(itor.Current.Value.ResourceRequest.isDone)
                {
                    Resources.UnloadAsset(itor.Current.Value.ResourceRequest.asset);
                }
            }
        }

        LoaderDict.Clear();
    }
}

public class ResourcesManager : SingltionCreateTinyUpdate<ResourcesManager>
{
    public Dictionary<string,ResourceInfo> LoadedResDict;
    public Dictionary<string,ResourceInfo> LoadingResDict;
    public Dictionary<string,List<LoadComplete>> LoadCompleteCallBackDict;

    public delegate void LoadComplete(params object[] obj);

    private AsycLoader Loader;

    public static void Destroy(UnityEngine.Object obj)
    {
        UnityEngine.Object.Destroy(obj);
    }
    
    public static void DestroyImmediate(UnityEngine.Object obj)
    {
        UnityEngine.Object.DestroyImmediate(obj);
    }

    public void Init()
    {
        Loader.Init();
        LoadingResDict = new Dictionary<string, ResourceInfo>();
        LoadedResDict = new Dictionary<string, ResourceInfo>(); 
        LoadCompleteCallBackDict = new Dictionary<string, List<LoadComplete>>();
    }
    
    public void SetAsset(string filePath,UnityEngine.Object asset)
    {
        ResourceInfo ResourceInfo;

        if(LoadingResDict.TryGetValue(filePath,out ResourceInfo))
        {
            ResourceInfo.LoadObj = asset;
            LoadedResDict.Add(filePath, ResourceInfo);
            LoadingResDict.Remove(filePath);
            
            List<LoadComplete> CallBackList;

            if(LoadCompleteCallBackDict.TryGetValue(filePath,out CallBackList))
            {
                for(int i=0;i<CallBackList.Count;i++)
                {
                    CallBackList[i](asset);
                }
            }
            
            CallBackList.Clear();
            LoadCompleteCallBackDict.Remove(filePath);
        }
    }
    
    public ResourceInfo LoadAsset(string filePath,bool dontDestory = false)
    {
        ResourceInfo resourceInfo;
        
        if (!LoadedResDict.ContainsKey(filePath))
        {
            resourceInfo = new ResourceInfo();
            resourceInfo.DontDestory = dontDestory;
            resourceInfo.loadInfo = new LoadInfo();
            resourceInfo.loadInfo.FilePath = filePath;
            resourceInfo.LoadObj = Resources.Load(filePath);
            if (resourceInfo.LoadObj == null)
                Debug.LogError("can't find resources  path:" + filePath);
            LoadedResDict.Add(filePath, resourceInfo);
        }
        else
        {
            LoadedResDict.TryGetValue(filePath, out resourceInfo);
        }
        
        return resourceInfo;
    }

    public void LoadAssetAsyc(string filePath,LoadComplete callBack = null, bool dontDestory = false)
    {
        LoadInfo loadInfo;
        ResourceInfo ResourceInfo;

        if (!LoadedResDict.TryGetValue(filePath,out ResourceInfo))
        {
            loadInfo = new LoadInfo();
            loadInfo.FilePath = filePath;
            
            ResourceInfo = new ResourceInfo();
            ResourceInfo.loadInfo = loadInfo;
            ResourceInfo.DontDestory = dontDestory;
            LoadingResDict.Add(filePath,ResourceInfo);
            Loader.Load(loadInfo);
            
            if(callBack != null)
            {
                List<LoadComplete> callBackList;
                if(LoadCompleteCallBackDict.TryGetValue(filePath,out callBackList))
                {
                    callBackList.Add(callBack);
                }
                else
                {
                    LoadCompleteCallBackDict[filePath] = new List<LoadComplete>();
                    LoadCompleteCallBackDict[filePath].Add(callBack);
                }
            }
        }
        else
        {
            if(callBack != null)
            {
                callBack(ResourceInfo.LoadObj);
            }
        }
    }

    public bool IsAnyAsyncLoading()
    {
        return Loader.IsAnyLoading();
    }

    public ResourceInfo GetFile(string filePath)
    {
        ResourceInfo ResourceInfo;
        
        if(LoadedResDict.TryGetValue(filePath,out ResourceInfo))
        {
            return ResourceInfo;
        }

        return null;
    }

    public void DestroyAssets()
    {
        Dictionary<string,List<LoadComplete>>.Enumerator itor1 = LoadCompleteCallBackDict.GetEnumerator();

        while(itor1.MoveNext())
        {
            itor1.Current.Value.Clear();
        }

        LoadCompleteCallBackDict.Clear();
        
        Loader.StopAll();

        if(LoadedResDict.Count > 0)
        {
            List<string> RemoveResoureceInfoList = new List<string>();

            Dictionary<string,ResourceInfo>.Enumerator itor2 = LoadedResDict.GetEnumerator();

            while(itor2.MoveNext())
            {
                if(itor2.Current.Value.LoadObj != null && !itor2.Current.Value.DontDestory)
                {
                    itor2.Current.Value.ReleaseReference();
                    itor2.Current.Value.LoadObj = null;
                    RemoveResoureceInfoList.Add(itor2.Current.Key);
                }
            }

            for(int i=0;i<RemoveResoureceInfoList.Count;i++)
            {
                LoadedResDict.Remove(RemoveResoureceInfoList[i]);
            }

            RemoveResoureceInfoList.Clear();
        }
    }
    
    public override bool OnUpdate()
    {
        Loader.OnUpdate();
        return base.OnUpdate();
    }

    public void OnClean()
    {
        Resources.UnloadUnusedAssets();
        GC.Collect();
    }

}