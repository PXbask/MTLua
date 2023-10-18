using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using XLua;

public class LuaManager : MonoBehaviour
{
    /// <summary>
    /// 所有lua文件名
    /// </summary>
    public List<string> LuaNames = new List<string>();
    /// <summary>
    /// 缓存Lua脚本内容
    /// </summary>
    private Dictionary<string, byte[]> m_LuaScript = new Dictionary<string, byte[]>();
    public LuaEnv LuaEnv;
    public void Init()
    {
        LuaEnv = new LuaEnv();
        LuaEnv.AddLoader(Loader);
#if UNITY_EDITOR
        if (APPConst.GameMode == GameMode.EditorMode)
            this.EditorLoadLuaScript();
        else
#endif
            this.LoadLuaScript();

        Manager.Event.Fire((int)GameEvent.OnLuaInit);
    }
    public byte[] Loader(ref string luaName)
    {
        return GetLuaScript(luaName);
    }
    public void StartLua(string name)
    {
        LuaEnv.DoString(string.Format("require '{0}'", name));
    }
    public byte[] GetLuaScript(string luaName)
    {
        //require ui.login.regsister
        luaName = luaName.Replace(".", "/");
        string fileName = PathUtil.GetLuaPath(luaName);
        byte[] luaScript= null;
        if(!m_LuaScript.TryGetValue(fileName, out luaScript))
        {
            Debug.LogErrorFormat("LuaScript:{0} is not found", fileName);
        }
        return luaScript;
    }
    void LoadLuaScript()
    {
        foreach (var name in LuaNames)
        {
            Manager.Resources.LoadLua(name, (UnityEngine.Object obj) =>
            {
                AddLuaScript(name, (obj as TextAsset).bytes);
                if (m_LuaScript.Count >= LuaNames.Count)
                {
                    //所有Lua加载完成
                    Manager.Event.Fire((int)GameEvent.StartLua);
                    LuaNames.Clear();
                    LuaNames = null;
                }
            });
        }
    }
#if UNITY_EDITOR
    void EditorLoadLuaScript()
    {
        string[] files = Directory.GetFiles(PathUtil.LuaPath,"*.bytes",SearchOption.AllDirectories);
        foreach (var file in files)
        {
            string fileName = PathUtil.GetStandardPath(file);
            byte[] bytes = File.ReadAllBytes(fileName);
            AddLuaScript(PathUtil.GetUnityPath(fileName), bytes);
        }
        Manager.Event.Fire((int)GameEvent.StartLua);
    }
#endif
    private void AddLuaScript(string fileName, byte[] bytes)
    {
        m_LuaScript[fileName] = bytes;
    }
    void Update()
    {
        this.LuaEnv?.Tick();
    }
    private void OnDestroy()
    {
        if(this.LuaEnv!= null)
        {
            this.LuaEnv.Dispose();
            this.LuaEnv = null;
        }
    }
}
