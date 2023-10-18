using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityLogic : LuaBehaviour
{
    Action LuaOnShow;
    Action LuaOnHide;
    public override void Init(string LuaName)
    {
        base.Init(LuaName);
        scriptEnv.Get("OnShow", out LuaOnShow);
        scriptEnv.Get("OnHide", out LuaOnHide);
    }
    public void OnShow()
    {
        LuaOnShow?.Invoke();
    }
    public void OnHide()
    {
        LuaOnHide?.Invoke();
    }
    protected override void Clear()
    {
        base.Clear();
        LuaOnShow = null;
        LuaOnHide = null;
    }
}
