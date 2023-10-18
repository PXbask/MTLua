using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLogic : LuaBehaviour
{
    public string SceneName;
    Action LuaOnActive;
    Action LuaOnInActive;
    Action LuaOnEnter;
    Action LuaOnQuit;
    public override void Init(string LuaName)
    {
        base.Init(LuaName);
        scriptEnv.Get("OnActive", out LuaOnActive);
        scriptEnv.Get("OnInActive", out LuaOnInActive);
        scriptEnv.Get("OnEnter", out LuaOnEnter);
        scriptEnv.Get("OnQuit", out LuaOnQuit);
    }
    public void OnActive()
    {
        LuaOnActive?.Invoke();
    }
    public void OnInActive()
    {
        LuaOnInActive?.Invoke();
    }
    public void OnEnter()
    {
        LuaOnEnter?.Invoke();
    }
    public void OnQuit()
    {
        LuaOnQuit?.Invoke();
    }
    protected override void Clear()
    {
        base.Clear();
        LuaOnActive = null;
        LuaOnInActive = null;
        LuaOnEnter = null;
        LuaOnQuit = null;
    }
}
