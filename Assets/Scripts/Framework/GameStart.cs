using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStart : MonoBehaviour
{
    [Tooltip("选择开发模式")]
    public GameMode GameMode;
    [Tooltip("允许打开日志")]
    public bool OpenLog;
    private void Start()
    {
        Manager.Event.Subscribe((int)GameEvent.StartLua, StartLua);
        Manager.Event.Subscribe((int)GameEvent.GameInit, GameInit);

        APPConst.GameMode = GameMode;
        APPConst.OpenLog = OpenLog;
        DontDestroyOnLoad(this);

        if (APPConst.GameMode == GameMode.UpdateMode)
            this.gameObject.AddComponent<HotUpdate>();
        else
            Manager.Event.Fire((int)GameEvent.GameInit);
    }
    private void GameInit(object arg)
    {
        if(APPConst.GameMode != GameMode.EditorMode)
            Manager.Resources.ParseVersionFile();
        Manager.Lua.Init();
    }
    private void StartLua(object args)
    {
        Manager.Lua.StartLua("main");

        Manager.Pool.CreateGameObjectPool("UI", 600);
        Manager.Pool.CreateGameObjectPool("EventObject", 300);
        Manager.Pool.CreateGameObjectPool("Effect", 120);

        Manager.Pool.CreateAssetPool("AssetBundle", 10);
    }
    public void OnApplicationQuit()
    {
        Manager.Event.UnSubscribe((int)GameEvent.StartLua, StartLua);
        Manager.Event.UnSubscribe((int)GameEvent.GameInit, GameInit);
    }
}
