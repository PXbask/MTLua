using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameObjectPool : PoolBase
{
    public override Object Spawn(string name)
    {
        Object obj = base.Spawn(name);
        if (obj == null) return null;
        GameObject gameObject = obj as GameObject;
        gameObject?.SetActive(true);
        return obj;
    }
    public override void UnSpawn(string name, Object @object)
    {
        GameObject obj = @object as GameObject;
        obj.SetActive(false);
        obj.transform.SetParent(this.transform, false);
        base.UnSpawn(name, @object);
    }
    public override void Release()
    {
        base.Release();
        foreach (var obj in Objects)
        {
            long time = System.DateTime.Now.Ticks - obj.LastUseTime.Ticks;
            if (time > ReleaseTime * 10000000)
            {
                Debug.LogFormat("GameObjectPool release Time:{0}", System.DateTime.Now);
                Destroy(obj.Object);
                Manager.Resources.MinusBundleCount(assetName: obj.Name);
                Objects.Remove(obj);
                Release();
                return;
            }
        }
    }
}
 