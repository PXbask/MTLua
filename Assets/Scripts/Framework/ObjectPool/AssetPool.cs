using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPool : PoolBase
{
    public override Object Spawn(string name)
    {
        return base.Spawn(name);
    }
    public override void UnSpawn(string name, Object @object)
    {
        base.UnSpawn(name, @object);
    }
    public override void Release()
    {
        base.Release();
        foreach (var asset in Objects)
        {
            if (System.DateTime.Now.Ticks - asset.LastUseTime.Ticks > ReleaseTime * 10000000)
            {
                Debug.LogFormat("AssetPool release Time:{0} unload ab:{1}", System.DateTime.Now, asset.Name);
                Manager.Resources.UnLoadBundle(asset.Object);
                Objects.Remove(asset);
                Release();
                return;
            }
        }
    }
}
