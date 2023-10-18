using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// UI·Ö×é
    /// </summary>
    Dictionary<string,Transform> UIGroups = new Dictionary<string,Transform>();
    Dictionary<string, UILogic> activeUIs = new Dictionary<string, UILogic>();
    private Transform UIParent;
    private void Awake()
    {
        UIParent = this.transform.parent.Find("UI");
    }
    public void SetUIGroup(List<string> group)
    {
        foreach (var item in group)
        {
            GameObject go = new GameObject("group_" + item);
            go.transform.SetParent(UIParent, false);
            UIGroups.Add(item, go.transform);
        }
    }
    Transform GetUIGroup(string group)
    {
        if (!UIGroups.TryGetValue(group, out var go))
        {
            Debug.LogErrorFormat("Group: {0} is not found", group);
            return null;
        }
        else
        {
            return go;
        }
    }
    public void OpenUI(string uiName,int sortlayer,string luaName)
    {
        GameObject ui = null;
        string uipath = PathUtil.GetUIPath(uiName);
        Object uiObj = Manager.Pool.Spawn("UI", uipath);
        if (uiObj!=null)
        {
            ui = uiObj as GameObject;
            ui.GetComponent<Canvas>().sortingOrder = sortlayer;
            UILogic uILogic = ui.GetComponent<UILogic>();
            this.activeUIs.Add(uiName, uILogic);
            ui.transform.SetParent(UIParent, false);
            uILogic.OnOpen();
            return;
        }
        Manager.Resources.LoadUI(uiName, (UnityEngine.Object obj) =>
        {
            ui = Instantiate(obj) as GameObject;
            ui.transform.SetParent(UIParent, false);
            ui.GetComponent<Canvas>().sortingOrder = sortlayer;
            UILogic uiLogic = ui.AddComponent<UILogic>();
            this.activeUIs.Add(uiName, uiLogic);
            uiLogic.assetName = uipath;
            uiLogic.Init(luaName);
            uiLogic.OnOpen();
        });
    }
    public void CloseUI(string uiName)
    {
        if(this.activeUIs.TryGetValue(uiName, out UILogic res))
        {
            res.OnClose();
            string uipath = PathUtil.GetUIPath(uiName);
            Manager.Pool.UnSpawn("UI", uipath, res.gameObject);
            this.activeUIs.Remove(uiName);
        }

    }
    public void CloseAllActives()
    {
        foreach (var item in this.activeUIs)
        {
            item.Value.OnClose();
            string uipath = PathUtil.GetUIPath(item.Key);
            Manager.Pool.UnSpawn("UI", uipath, item.Value.gameObject);
        }
        this.activeUIs.Clear();
    }
}
