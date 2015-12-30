using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml;
using System.IO;

public class AsyncResource
{
    public AsyncResource(ResourceRequest op, System.Object param)
    {
        asyncOperation = op;
        this.param = param;

    }
    public ResourceRequest asyncOperation;
    public System.Object param;
}
public class SingleAsyncRes
{
    public Action<AsyncResource> m_callBack;
    public AsyncResource m_res;
    public SingleAsyncRes(Action<AsyncResource> callBack, AsyncResource res)
    {
        m_callBack = callBack;
        m_res = res;
    }
}

public class GroupAsyncRes
{
    public Action<Dictionary<string, AsyncResource>> m_callBack;
    public Dictionary<string, AsyncResource> m_res;

    public GroupAsyncRes(Action<Dictionary<string, AsyncResource>> callBack, Dictionary<string, AsyncResource> res)
    {
        m_callBack = callBack;
        m_res = res;
    }
}
public class ResourceManager
{
    private static ResourceManager s_instance;
    public const bool useStreamAsset = false;
    public static ResourceManager instance
    {
        get
        {
            if (s_instance == null)
                s_instance = new ResourceManager();
            return s_instance;
        }
    }

    private Dictionary<string, Dictionary<string, Sprite>> m_allAtlas;
    private string m_atlasPath = "UI/Atlas/";
    private string[] m_commonAtlasName = new string[] { };
    private string[] m_hallAtlasName = new string[] { "AtlasMainHall" };
    private string[] m_battleAtlasName = new string[] { "AtlasHUD" };

    private List<GroupAsyncRes> m_asyncGroupLoadingRes = new List<GroupAsyncRes>();
    private List<SingleAsyncRes> m_asyncSingleLoadingRes = new List<SingleAsyncRes>();
    private ResourceManager()
    {
        m_allAtlas = new Dictionary<string, Dictionary<string, Sprite>>();
    }

    public void Init()
    {
    }

    public UnityEngine.Object GetResourceByPath(string path)
    {
        return Resources.Load(path);
    }

    public void ReleaseMemory()
    {
        Resources.UnloadUnusedAssets();
        //if (useStreamAsset)
        //    m_assetManager.ReleaseMemory();
        System.GC.Collect();
    }

    public AsyncOperation GetResourceByPathAsync<T>(string path, Action<AsyncResource> callBack, System.Object param) where T : UnityEngine.Object
    {
        if (useStreamAsset)
        {
            return null;
        }
        else
        {
            ResourceRequest resReq = Resources.LoadAsync<T>(path);
            AsyncResource ar = new AsyncResource(resReq, param);
            SingleAsyncRes singleRes = new SingleAsyncRes(callBack, ar);
            m_asyncSingleLoadingRes.Add(singleRes);
            return resReq;
        }
    }

    public UnityEngine.Object GetPrefab(AsyncOperation ao)
    {
        if (useStreamAsset)
        {
            return (ao as AssetBundleRequest).allAssets[0];
        }
        else
        {
            return (ao as ResourceRequest).asset;
        }
    }

    public void GetResourceByPathAsync(List<string> path, Action<Dictionary<string, AsyncResource>> callBack = null, System.Object param = null)
    {
        if (useStreamAsset)
        {
            return;
        }
        else
        {
            Dictionary<string, AsyncResource> item = new Dictionary<string, AsyncResource>();
            foreach (string resPath in path)
            {
                if (item.ContainsKey(resPath))
                {
                    // Debug.Log("same res: "+resPath);
                    continue;
                }
                ResourceRequest resReq = Resources.LoadAsync(resPath);
                AsyncResource ar = new AsyncResource(resReq, param);
                item.Add(resPath, ar);
            }
            GroupAsyncRes groupRes = new GroupAsyncRes(callBack, item);
            m_asyncGroupLoadingRes.Add(groupRes);
        }
    }

    public AsyncOperation GetResourceByPathAsync(string path, Action<AsyncResource> callBack = null, System.Object param = null)
    {
        if (useStreamAsset)
        {
            return null;
        }
        else
        {
            ResourceRequest resReq = Resources.LoadAsync(path);
            AsyncResource ar = new AsyncResource(resReq, param);
            SingleAsyncRes groupRes = new SingleAsyncRes(callBack, ar);
            m_asyncSingleLoadingRes.Add(groupRes);
            return resReq;
        }
    }

    public void Update()
    {
        //List<GroupAsyncRes> toDelete = new List<GroupAsyncRes>();

        for (int i = 0; i < m_asyncGroupLoadingRes.Count; ++i)
        {
            bool bDone = true;
            foreach (string key in m_asyncGroupLoadingRes[i].m_res.Keys)
            {
                if (!m_asyncGroupLoadingRes[i].m_res[key].asyncOperation.isDone)
                {
                    bDone = false;
                    break;
                }

            }
            if (bDone)
            {
                if (m_asyncGroupLoadingRes[i].m_callBack != null)
                    m_asyncGroupLoadingRes[i].m_callBack(m_asyncGroupLoadingRes[i].m_res);
                //toDelete.Add(m_asyncGroupLoadingRes[i]);
                m_asyncGroupLoadingRes.RemoveAt(i);
                i--;
            }
        }
        for (int i = 0; i < m_asyncSingleLoadingRes.Count; i++)
        {
            SingleAsyncRes sr = m_asyncSingleLoadingRes[i];
            if (sr.m_res.asyncOperation.isDone)
            {
                if (sr.m_callBack != null)
                {
                    sr.m_callBack(sr.m_res);
                }
                m_asyncSingleLoadingRes.RemoveAt(i);
                i--;
            }

        }
        //foreach (var item in toDelete)
        //{
        //    m_asyncGroupLoadingRes.Remove(item);
        //}

    }

    public T GetResourceByPath<T>(string path) where T : UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }
    //获取UI图的方法
    public Sprite GetUISprite(string atlasName, string spriteName)
    {
        if (m_allAtlas.ContainsKey(atlasName))
        {
            Dictionary<string, Sprite> tmp = m_allAtlas[atlasName];
            if (tmp.ContainsKey(spriteName))
                return tmp[spriteName];
        }
        return null;
    }

    public class ColumnType
    {
        public static int ResourceId = 0;
        public static int ResourcePath = 1;
        public static int BundlePath = 2;
    };
}
