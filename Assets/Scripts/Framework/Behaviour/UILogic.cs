using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class UILogic : LuaBehaviour
{
    public string assetName;
    Action LuaOnOpen;
    Action LuaOnClose;
    public override void Init(string LuaName)
    {
        base.Init(LuaName);
        scriptEnv.Get("OnOpen",out LuaOnOpen);
        scriptEnv.Get("OnClose",out LuaOnClose);
    }
    public void OnOpen()
    {
        LuaOnOpen?.Invoke();
    }
    public void OnClose()
    {
        LuaOnClose?.Invoke();
    }
    protected override void Clear()
    {
        base.Clear();
        LuaOnOpen = null;
        LuaOnClose = null;
    }
}
