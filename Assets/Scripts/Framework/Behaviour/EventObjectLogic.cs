using MT.Event;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(SpriteRenderer))]
public class EventObjectLogic : LuaBehaviour, MT.Event.IEventBasic
{
    #region Func&Action
    private Func<int> getEntityType = null;
    private Func<int> getHp = null;
    private Func<int> getAtk = null;
    private Func<int> getDef = null;
    private Func<bool> getCanMove = null;
    private Func<bool> getCanBattle = null;
    private Func<bool> getEffectAtOnce = null;
    private Action beforeOverlay = null;
    private Action onOverlay = null;
    private Action afterOverlay = null;
    private Action beforeChangeFloor = null;
    private Action onChangeFloor = null;
    private Action beforeBattle = null;
    private Action afterBattle = null;
    private Action beforeGetItem = null;
    private Action onUseItem = null;
    private Action afterGetItem = null;
    private Action beforeOpenDoor = null;
    private Action afterOpenDoor = null;
    #endregion

    public string assetName;
    public Animator animator;
    public SpriteRenderer render;

    private void Start()
    {
        this.animator = GetComponent<Animator>();
        this.render = GetComponent<SpriteRenderer>();
    }

    public override void Init(string LuaName)
    {
        base.Init(LuaName);
        this.scriptEnv.Get("getEntityType", out getEntityType);
        this.scriptEnv.Get("getHp", out getHp);
        this.scriptEnv.Get("getAtk", out getAtk);
        this.scriptEnv.Get("getDef", out getDef);
        this.scriptEnv.Get("getCanMove", out getCanMove);
        this.scriptEnv.Get("getCanBattle", out getCanBattle);
        this.scriptEnv.Get("getEffectAtOnce", out getEffectAtOnce);
        this.scriptEnv.Get("beforeOverlay", out beforeOverlay);
        this.scriptEnv.Get("onOverlay", out onOverlay);
        this.scriptEnv.Get("afterOverlay", out afterOverlay);
        this.scriptEnv.Get("beforeChangeFloor", out beforeChangeFloor);
        this.scriptEnv.Get("onChangeFloor", out onChangeFloor);
        this.scriptEnv.Get("beforeBattle", out beforeBattle);
        this.scriptEnv.Get("afterBattle", out afterBattle);
        this.scriptEnv.Get("beforeGetItem", out beforeGetItem);
        this.scriptEnv.Get("onUseItem", out onUseItem);
        this.scriptEnv.Get("afterGetItem", out afterGetItem);
        this.scriptEnv.Get("beforeOpenDoor", out beforeOpenDoor);
        this.scriptEnv.Get("afterOpenDoor", out afterOpenDoor);
    }

    protected override void Clear()
    {
        base.Clear();
        getEntityType = null;
        getHp = null;
        getAtk = null;
        getDef = null;
        getCanMove = null;
        getCanBattle = null;
        getEffectAtOnce = null;
        beforeOverlay = null;
        onOverlay = null;
        afterOverlay = null;
        beforeChangeFloor = null;
        onChangeFloor = null;
        beforeBattle = null;
        afterBattle = null;
        beforeGetItem = null;
        onUseItem = null;
        afterGetItem = null;
        beforeOpenDoor = null;
        afterOpenDoor = null;
    }

    public bool Belong(EventObjectType t) => (this.GetEntityType() & (int)t) != 0;

    public GameObject GetGameObject() => this.gameObject;

    private bool CanBattleable(PlayerLogic rht, out int damage)
    {
        damage = 0;
        var lft = this;
        if (!lft.CanBattle()) return false;

        int lfthp = lft.GetHp();
        int rhthp = rht.Curhp;

        int lftatk = lft.GetAtk();
        int rhtatk = rht.Curatk;

        int lftdef = lft.GetDef();
        int rhtdef = rht.Curdef;

        bool resb;

        if (rhtatk <= lftdef)
        {
            damage = 99999;
            return false;
        }
        if (lftatk <= rhtdef)
        {
            damage = 0;
            return true;
        }
        while (true)
        {
            int res = rhtatk - lftdef;
            lfthp -= res;
            if (lfthp <= 0) {
                resb = true;
                break;
            }

            res = lftatk - rhtdef;
            rhthp -= res;
            if (rhthp <= 0) {
                resb = false;
                break;
            }
        }
        damage = rht.Curhp - rhthp;
        return resb;
    }

    internal bool CanBattleable(out int damage) => this.CanBattleable(MTRuntime.instance.Player, out damage);

    #region Callbacks
    public int GetEntityType() => getEntityType.Invoke();

    public void OnafterBattle() => afterBattle?.Invoke();

    public void OnafterGetItem() => afterGetItem?.Invoke();

    public void OnafterOpenDoor() => afterOpenDoor?.Invoke();

    public void OnafterOverlay() => afterOverlay?.Invoke();

    public void OnbeforeBattle()
    {
        Manager.Pool.UnSpawn("Enemy", this.assetName, this.gameObject);
        beforeBattle?.Invoke();
    }

    public void OnbeforeChangeFloor() => beforeChangeFloor?.Invoke();

    public void OnbeforeGetItem() => beforeGetItem?.Invoke();

    public void OnbeforeOpenDoor() => beforeOpenDoor?.Invoke();

    public void OnbeforeOverlay() => beforeOverlay?.Invoke();

    public int GetHp() => getHp.Invoke();

    public int GetAtk() => getAtk.Invoke();

    public int GetDef() => getDef.Invoke();

    public bool CanMove() => getCanMove.Invoke();

    public void OnOverlay() => onOverlay?.Invoke();

    public bool CanBattle() => getCanBattle.Invoke();

    public void OnChangeFloor() => onChangeFloor?.Invoke();

    public bool EffectAtOnce() => getEffectAtOnce.Invoke();

    public void OnUseItem() => onUseItem?.Invoke();
    #endregion

    public string GetAssetName() => this.assetName;
}
