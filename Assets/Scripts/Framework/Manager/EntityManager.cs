using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    Dictionary<string, GameObject> Entity = new Dictionary<string, GameObject>();
    Dictionary<string, Transform> EntityGroups = new Dictionary<string, Transform>();
    private Transform EntityParent;
    private void Awake()
    {
        EntityParent = this.transform.parent.Find("Entity");
    }
    public void SetEntityGroup(List<string> group)
    {
        foreach (var item in group)
        {
            GameObject go = new GameObject("group_" + item);
            go.transform.SetParent(EntityParent, false);
            EntityGroups.Add(item, go.transform);
        }
    }
    Transform GetEntityGroup(string group)
    {
        if (!EntityGroups.TryGetValue(group, out var go))
        {
            Debug.LogErrorFormat("Group: {0} is not found", group);
            return null;
        }
        else
        {
            return go;
        }
    }
    public void ShowEntity(string entityName, string group, string luaName)
    {
        GameObject en = null;
        if (Entity.TryGetValue(entityName, out en))
        {
            EntityLogic entityLogic = en.GetComponent<EntityLogic>();
            entityLogic.OnShow();
            return;
        }
        Manager.Resources.LoadPrefab(entityName, (UnityEngine.Object obj) =>
        {
            GameObject en = Instantiate(obj) as GameObject;
            Entity.Add(entityName, en);

            Transform parent = GetEntityGroup(group);
            en.transform.SetParent(parent, false);

            EntityLogic entityLogic = en.AddComponent<EntityLogic>();
            entityLogic.Init(luaName);
            entityLogic.OnShow();
        });
    }
}
