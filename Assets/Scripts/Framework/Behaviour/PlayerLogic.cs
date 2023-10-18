using MT.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLogic : LuaBehaviour, MT.Event.IBattleable
{
    private Action<Vector2Int> onPositionChanged = null;
    private Action<int> onHurt = null;
    //TODO: from file
    int curhp = 100;
    int curatk = 100;
    int curdef = 100;

    public int Curhp { get => curhp; set => curhp = value; }
    public int Curatk { get => curatk; set => curatk = value; }
    public int Curdef { get => curdef; set => curdef = value; }

    public override void Init(string LuaName)
    {
        base.Init(LuaName);
        scriptEnv.Get("onPositionChanged", out onPositionChanged);
        scriptEnv.Get("onHurt", out onHurt);
    }

    protected override void Clear()
    {
        base.Clear();
        onPositionChanged = null;
        onHurt = null;
    }
    
    public void LoadConfig()
    {
        this.Curhp = 0;
        this.Curatk = 0;
        this.Curdef = 0;
    }

    public void OnPositionChanged(Vector2Int position)
    {
        this.transform.position = MT.Util.UnityUtil.GetTPosition(position);
        onPositionChanged?.Invoke(position);
    }

    public void OnEventProcess(MTRuntime.ExpectData ex)
    {
        this.OnPositionChanged(ex.expos);
        this.OnHurt(ex.exdamage);
    }

    #region IBattleable TODO: Lua¼àÌýÊÂ¼þ
    public void OnbeforeBattle() => Debug.Log("Player before battle");

    public void OnafterBattle() => Debug.Log("Player after battle");

    public int GetHp() => this.Curhp;

    public int GetAtk() => this.Curatk;

    public int GetDef() => this.Curdef;

    public bool CanBattle() => true;

    public int GetEntityType() => (int)MT.Event.EventObjectType.None;

    public bool CanMove() => true;

    public GameObject GetGameObject() => this.gameObject;

    public bool Belong(EventObjectType t) => false;

    public string GetAssetName() => string.Empty;

    public void OnHurt(int da)
    {
        //TODO: LUA
        Curhp -= da;
        onHurt?.Invoke(da);
    }
    #endregion

}
