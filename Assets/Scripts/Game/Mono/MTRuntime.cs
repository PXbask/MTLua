using MT.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MTRuntime : MonoBehaviour
{
    public static MTRuntime instance;
    private PlayerLogic player;
    private MT.Mono.StageContainer stageContainer;

    private int curLevel;
    private MT.Data.StageData stageData;
    private Vector2Int playerPos = new Vector2Int(6,11);

    private int Scale => stageData.Scale;
    public PlayerLogic Player => player;

    public ItemManager bag;

    private void Awake()
    {
        instance = this;
        this.player = GameObject.Find("Player").GetComponent<PlayerLogic>();
        this.stageContainer = MT.Mono.StageContainer.instance;
        this.bag = ItemManager.Instance;
    }
    private void Start()
    {
        this.Init();
    }

    public void Init()
    {
        this.stageContainer.Init();
        this.bag.Init();

        this.curLevel = 1; //TODO:from file
        this.stageData = null;
        LoadLevel(this.curLevel, out this.stageData);
    }

    public void LoadLevel(int level, out MT.Data.StageData res)
    {
        this.stageContainer.EnterLevel(level, out res);
    }

    private void Update()
    {
        var data = this.DetectInput();
        ProcessEvent(data);
    }

    private void ProcessEvent(ExpectData vi)
    {
        bool needProcess = vi.expos != vi.lastpos;

        //检测到即将移动,则事件预处理
        if(needProcess)
        {
            if (!this.EventPreprocess(vi))
            {
                Debug.LogError("error in EventPreprocess");
                return;
            }
        }

        //事件处理,每帧执行
        if (!this.EventProcess(vi))
        {
            Debug.LogError("error in EventProcess");
            return;
        }

        //事件后处理
        if (needProcess)
        {
            if (!this.EventProprocess(vi))
            {
                Debug.LogError("error in EventProprocess");
                return;
            }
        }
    }

    #region Input
    private ExpectData DetectInput()
    {
        int damage = 0;
        Vector2Int targetPos = this.playerPos;

        if(Input.GetKeyDown(KeyCode.A))
        {
            Vector2Int _tmp = targetPos;
            if (!CanMove(_tmp.x - 1, _tmp.y, out damage))
                return new ExpectData() { lastpos =_tmp, expos = _tmp, exdamage = 0 };
            targetPos = _tmp + Vector2Int.left;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Vector2Int _tmp = targetPos;
            if (!CanMove(_tmp.x + 1, _tmp.y, out damage))
                return new ExpectData() { lastpos = _tmp, expos = _tmp, exdamage = 0 };
            targetPos = _tmp + Vector2Int.right;
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Vector2Int _tmp = targetPos;
            if (!CanMove(_tmp.x, _tmp.y - 1, out damage))
                return new ExpectData() { lastpos = _tmp, expos = _tmp, exdamage = 0 };
            targetPos = _tmp + Vector2Int.down;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Vector2Int _tmp = targetPos;
            if (!CanMove(_tmp.x, _tmp.y + 1, out damage))
                return new ExpectData() { lastpos = _tmp, expos = _tmp, exdamage = 0 };
            targetPos = _tmp + Vector2Int.up;
        }

        return new ExpectData() { lastpos = this.playerPos, expos = targetPos, exdamage = damage };
    }
    #endregion

    public bool CanMove(int x, int y, out int damage) => this.stageContainer.CanMove(x, y, out damage);

    private bool EventPreprocess(ExpectData ex) => this.stageContainer.EventPreprocess(ex);

    private bool EventProcess(ExpectData ex)
    {
        this.playerPos = ex.expos;
        this.player.OnEventProcess(ex);
        return this.stageContainer.EventProcess(ex);
    }

    private bool EventProprocess(ExpectData ex) => this.stageContainer.EventProprocess(ex);

    [XLua.LuaCallCSharp]
    public struct RuntimeData
    {
        public int playerhp;
        public int playeratk;
        public int playerdef;
        public static RuntimeData Cur()
        {
            RuntimeData res = new RuntimeData();
            res.playerhp = instance.player.Curhp;
            res.playeratk = instance.player.Curatk;
            res.playerdef = instance.player.Curdef;
            return res;
        }
    }

    [XLua.LuaCallCSharp]
    public struct ExpectData
    {
        public Vector2Int lastpos;
        public Vector2Int expos;
        public int exdamage;
    }
}
