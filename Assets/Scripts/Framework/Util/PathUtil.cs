using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathUtil
{
    /// <summary>
    /// Unity��Դ��Ŀ¼
    /// </summary>
    public static readonly string AssetsPath = Application.dataPath;
    /// <summary>
    /// ��Ҫ��Bundle��Ŀ¼
    /// </summary>
    public static readonly string BuildResourcesPath = AssetsPath + "/BuildResources/";
    /// <summary>
    /// Bundle���Ŀ¼
    /// </summary>
    public static readonly string BundleOutPath = Application.streamingAssetsPath;
    /// <summary>
    /// ֻ��Ŀ¼
    /// </summary>
    public static readonly string ReadablePath = Application.streamingAssetsPath;
    /// <summary>
    /// �ɶ�дĿ¼
    /// </summary>
    public static readonly string ReadWritablePath = Application.persistentDataPath;
    /// <summary>
    /// Lua·��
    /// </summary>
    public static readonly string LuaPath = "Assets/BuildResources/LuaScripts";
    /// <summary>
    /// AssetBundle����·��
    /// </summary>
    public static string BundleResourcesOutPath
    {
        get
        {
            if(APPConst.GameMode == GameMode.UpdateMode)
            {
                return ReadWritablePath;
            }
            return ReadablePath;
        }
    }
    /// <summary>
    /// ��ȡUnity���·��
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetUnityPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        return path.Substring(path.IndexOf("Assets"));
    }
    /// <summary>
    /// ��ȡ��׼·��: \->/
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetStandardPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return string.Empty;
        return path.Trim().Replace("\\", "/");
    }
    public static string GetLuaPath(string name)
    {
        return string.Format("Assets/BuildResources/LuaScripts/{0}.bytes", name);
    }
    public static string GetDataPath(string name)
    {
        return string.Format("Assets/BuildResources/Data/{0}.txt", name);
    }
    public static string GetUIPath(string name)
    {
        return string.Format("Assets/BuildResources/UI/Prefab/{0}.prefab", name);
    }
    public static string GetMusicPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Music/{0}", name);
    }
    public static string GetSoundPath(string name)
    {
        return string.Format("Assets/BuildResources/Audio/Sound/{0}", name);
    }
    public static string GetModelPath(string name)
    {
        return string.Format("Assets/BuildResources/Model/Prefabs/{0}.prefab", name);
    }
    public static string GetEffectPath(string name)
    {
        return string.Format("Assets/BuildResources/Effect/Prefabs/{0}.prefab", name);
    }
    public static string GetSpritePath(string name)
    {
        return string.Format("Assets/BuildResources/Sprite/{0}", name);
    }
    public static string GetScenePath(string name)
    {
        return string.Format("Assets/BuildResources/Scene/{0}.unity", name);
    }
    public static string GetEOPath(string name)
    {
        return string.Format("Assets/BuildResources/EventObject/Prefab/{0}.prefab", name);
    }
}
