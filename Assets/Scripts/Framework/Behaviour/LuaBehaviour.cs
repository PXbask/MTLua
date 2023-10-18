using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehaviour : MonoBehaviour
{
    LuaEnv luaEnv;
    protected LuaTable scriptEnv;
    private Action LuaOnInit;
    private Action LuaUpdate;
    private Action LuaOnDestroy;

    public string LuaName;
    private void Awake()
    {
        luaEnv = Manager.Lua.LuaEnv;
        scriptEnv = luaEnv.NewTable();
        //每个脚本设置独立环境，防止脚本间全局变量，函数冲突
        LuaTable meta = luaEnv.NewTable();
        meta.Set("__index", luaEnv.Global);
        scriptEnv.SetMetaTable(meta);
        meta.Dispose();

        scriptEnv.Set("self", this);
    }
    public virtual void Init(string LuaName)
    {
        luaEnv.DoString(Manager.Lua.GetLuaScript(LuaName), LuaName, scriptEnv);
        scriptEnv.Get("OnInit", out LuaOnInit);
        scriptEnv.Get("Update", out LuaUpdate);
        scriptEnv.Get("OnDestroy", out LuaOnDestroy);

        LuaOnInit?.Invoke();
    }
    private void Update()
    {
        LuaUpdate?.Invoke();
    }
    private void OnDestroy()
    {
        LuaOnDestroy?.Invoke();
        Clear();
    }
    private void OnApplicationQuit()
    {
        Clear();
    }
    protected virtual void Clear()
    {
        LuaOnDestroy = null;
        LuaUpdate = null;
        LuaOnInit = null;
        scriptEnv?.Dispose();
        scriptEnv = null;
    }
}
