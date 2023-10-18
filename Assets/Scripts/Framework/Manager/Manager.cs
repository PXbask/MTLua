using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
	private static ResourcesManager _resources;

	public static ResourcesManager Resources
    {
		get { return _resources; }
	}
	private static LuaManager _lua;

	public static LuaManager Lua
	{
		get { return _lua; }
	}
	private static UIManager _ui;

	public static UIManager UI
	{
		get { return _ui; }
	}
	private static EntityManager _entity;

    public static EntityManager Entity
	{
		get { return _entity; }
	}
    private static PXSceneManager _scene;

    public static PXSceneManager Scene
	{
		get { return _scene; }
	}
	private static SoundManager _sound;

	public static SoundManager Sound
	{
		get { return _sound; }
		set { _sound = value; }
	}
	private static EventManager _event;

	public static EventManager Event
	{
		get { return _event; }
		set { _event = value; }
	}
	private static PoolManager _pool;

	public static PoolManager Pool
	{
		get { return _pool; }
		set { _pool = value; }
	}
    private static EventObjectManager _eo;

    public static EventObjectManager EO
    {
        get { return _eo; }
        set { _eo = value; }
    }


    private void Awake()
    {
        _resources= gameObject.AddComponent<ResourcesManager>();
		_lua= gameObject.AddComponent<LuaManager>();
		_ui = gameObject.AddComponent<UIManager>();
		_entity= gameObject.AddComponent<EntityManager>();
		_scene = gameObject.AddComponent<PXSceneManager>();
		_sound= gameObject.AddComponent<SoundManager>();
		_event = gameObject.AddComponent<EventManager>();
		_pool = gameObject.AddComponent<PoolManager>();
		_eo = gameObject.AddComponent<EventObjectManager>();

		MT.Managers.DataManager.Instance.Init();
        MT.Managers.MapManager.Instance.Init();
    }
}
