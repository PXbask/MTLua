using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UObject = UnityEngine.Object;

public class ResourcesManager : MonoBehaviour
{
    public class BundleInfo
    {
        public string AssetName;
        public string BundleName;
        public List<string> Dependences;
    }
    public class BundleData
    {
        //引用计数
        public int Count;
        public AssetBundle Bundle;
        public BundleData(AssetBundle bundle)
        {
            this.Count = 1;
            this.Bundle = bundle;
        }
    }
    private Dictionary<string,BundleInfo> BundleInfos= new Dictionary<string,BundleInfo>();
    Dictionary<string,BundleData> loadedAssetBundles = new Dictionary<string, BundleData>();
    /// <summary>
    /// 解析版本文件
    /// </summary>
    public void ParseVersionFile()
    {
        //版本文件的路径
        string url = Path.Combine(PathUtil.BundleResourcesOutPath, APPConst.FILE_LIST_OUTPUT_NAME);
        string[] fileData = File.ReadAllLines(url);
        //解析文件信息
        foreach (var data in fileData)
        {
            BundleInfo bundleInfo= new BundleInfo();
            string[] info = data.Split('|');
            bundleInfo.AssetName= info[0];
            bundleInfo.BundleName= info[1];
            List<string> list= new List<string>(info.Length-2);
            for(int i = 2; i < info.Length; i++)
            {
                list.Add(info[i]);
            }
            bundleInfo.Dependences = list;
            BundleInfos.Add(bundleInfo.AssetName, bundleInfo);
            if (info[0].Contains("LuaScripts"))
            {
                Manager.Lua.LuaNames.Add(info[0]);
            }
        }
    }
    IEnumerator LoadBundleAsync(string assetName,Action<UObject> callback = null)
    {
        string bundleName = BundleInfos[assetName].BundleName;
        string bundlePath = Path.Combine(PathUtil.BundleResourcesOutPath, bundleName);
        List<string> dependence = BundleInfos[assetName].Dependences;

        BundleData bundle = GetBundleData(bundleName);
        if (bundle == null)
        {
            UObject obj = Manager.Pool.Spawn("AssetBundle", bundleName);
            if(obj != null)
            {
                AssetBundle asset = obj as AssetBundle;
                bundle = new BundleData(asset);
            }
            else
            {
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
                yield return request;
                bundle = new BundleData(request.assetBundle);
            }
            loadedAssetBundles[bundleName] = bundle;
        }
        if (dependence != null && dependence.Count > 0)
        {
            foreach (var dep in dependence)
            {
                yield return LoadBundleAsync(dep);
            }
        }
        if (assetName.EndsWith(".unity"))
        {
            callback?.Invoke(null);
            yield break;
        }
        if (callback == null)
        {
            yield break;
        }
        AssetBundleRequest bundleRequest = bundle.Bundle.LoadAssetAsync(assetName);
        yield return bundleRequest;

        callback?.Invoke(bundleRequest?.asset);
    }
    BundleData GetBundleData(string assetName)
    {
        BundleData bundle = null;
        if(loadedAssetBundles.TryGetValue(assetName,out bundle))
        {
            bundle.Count++;
            return bundle;
        }
        return null;
    }
    public void MinusOneBundleCount(string bundleName)
    {
        if(loadedAssetBundles.TryGetValue(bundleName,out BundleData bundleData))
        {
            if(bundleData.Count > 0)
            {
                bundleData.Count--;
                Debug.LogFormat("bundle:{0} 引用计数:{1}",bundleName,bundleData.Count);
            }
            if(bundleData.Count<=0)
            {
                Debug.LogFormat("[Enter Pool]bundleName:{0}", bundleName);
                Manager.Pool.UnSpawn("AssetBundle", bundleName, bundleData.Bundle);
                loadedAssetBundles.Remove(bundleName);
            }
        }
    }
    public void MinusBundleCount(string assetName)
    {
        string bundleName = BundleInfos[assetName].BundleName;
        MinusOneBundleCount(bundleName);

        var dependences = BundleInfos[assetName].Dependences;
        if (dependences != null)
        {
            foreach (var dep in dependences)
            {
                string name = BundleInfos[dep].BundleName;
                MinusOneBundleCount(name);
            }
        }
    }
#if UNITY_EDITOR
    private void EditorLoadAsset(string assetName, Action<UObject> callback = null)
    {
        UObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath<UObject>(assetName);
        if (obj == null) Debug.LogErrorFormat("asset:{0} do not found", assetName);
        callback?.Invoke(obj);
    }
#endif
    #region Load Resources
    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    private void LoadAsset(string assetName, Action<UObject> callback = null)
    {
        Debug.Log("LoadAsset");
#if UNITY_EDITOR
        if(APPConst.GameMode==GameMode.EditorMode)
            EditorLoadAsset(assetName, callback);
        else
#endif
            StartCoroutine(LoadBundleAsync(assetName, callback));
    }
    public void LoadUI(string assetName, Action<UObject> callback = null)
    {
        LoadAsset(PathUtil.GetUIPath(assetName), callback);
    }
    public void LoadData(string assetName, Action<UObject> callback = null)
    {
        LoadAsset(PathUtil.GetDataPath(assetName), callback);
    }
    public void LoadEventObject(string assetName, Action<UObject> callback)
    {
        LoadAsset(PathUtil.GetEOPath(assetName), callback);
    }
    public void LoadMusic(string assetName, Action<UObject> callback = null)
    {
        LoadAsset(PathUtil.GetMusicPath(assetName), callback);
    }
    public void LoadSound(string assetName, Action<UObject> callback = null)
    {
        LoadAsset(PathUtil.GetSoundPath(assetName), callback);
    }
    public void LoadModel(string assetName, Action<UObject> callback = null)
    {
        LoadAsset(PathUtil.GetModelPath(assetName), callback);
    }
    public void LoadEffect(string assetName, Action<UObject> callback = null)
    {
        LoadAsset(PathUtil.GetEffectPath(assetName), callback);
    }
    public void LoadScene(string assetName, Action<UObject> callback = null)
    {
        LoadAsset(PathUtil.GetScenePath(assetName), callback);
    }
    public void LoadLua(string name, Action<UObject> callback)
    {
        LoadAsset(name, callback);
    }
    public void LoadPrefab(string name, Action<UObject> callback)
    {
        LoadAsset(name, callback);
    }

    #endregion
    internal void UnLoadBundle(UObject obj)
    {
        AssetBundle ab = obj as AssetBundle;
        ab.Unload(true);
    }
}
