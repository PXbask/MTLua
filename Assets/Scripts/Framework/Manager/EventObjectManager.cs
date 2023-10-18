using MT.Event;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventObjectManager : MonoBehaviour
{
    private readonly Dictionary<string, Transform> EOGroup= new Dictionary<string, Transform>();
    private Transform uiparent;

    private void Awake()
    {
        uiparent = this.transform.parent.Find("EventObject");
    }

    Transform GetEOGroup(string group)
    {
        if (!EOGroup.TryGetValue(group, out var go))
        {
            Debug.LogErrorFormat("[EventObjectManager] Group: {0} is not found", group);
            return null;
        }
        else
            return go;
    }

    internal MT.Event.IEventBasic GetEventObject(int eoid)
    {
        var data = MT.Managers.DataManager.Instance.GetEOData(eoid);
        return GetEventObject(data.AssetName, data.Group, data.LuaName);
    }

    private MT.Event.IEventBasic GetEventObject(string eoName, string grouptag, string luaName)
    {
        GameObject obj = null;
        string eopath = PathUtil.GetEOPath(eoName);
        Object eobj = Manager.Pool.Spawn("EventObject", eopath);
        if (eobj != null)
        {
            obj = eobj as GameObject;
            return obj.GetComponent<MT.Event.IEventBasic>();
        }
        Manager.Resources.LoadEventObject(eoName, (UnityEngine.Object objc) =>
        {
            obj = Instantiate(objc) as GameObject;
            EventObjectLogic ieb = obj.GetComponent<EventObjectLogic>();
            ieb.assetName = eopath;
            ieb.Init(luaName);
        });
        return obj.GetComponent<MT.Event.IEventBasic>();
    }
}
