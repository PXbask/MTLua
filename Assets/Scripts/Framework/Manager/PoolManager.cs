using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    Transform Parent;
    Dictionary<string,PoolBase> Pools = new Dictionary<string,PoolBase>();
    private void Awake()
    {
        Parent = this.transform.parent.Find("Pool");
    }
    /// <summary>
    /// 创建对象池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="poolName"></param>
    /// <param name="releaseTime"></param>
    private void CreatePool<T>(string poolName, float releaseTime) where T : PoolBase
    {
        if(!Pools.TryGetValue(poolName, out PoolBase pool))
        {
            GameObject go = new GameObject(poolName);
            go.transform.SetParent(Parent);
            pool = go.AddComponent<T>();
            pool.Init(releaseTime);
            Pools.Add(poolName, pool);
        }
    }
    public void CreateAssetPool(string poolName,float releasetime)
    {
        this.CreatePool<AssetPool>(poolName, releasetime);
    }
    public void CreateGameObjectPool(string poolName, float releasetime)
    {
        this.CreatePool<GameObjectPool>(poolName, releasetime);
    }
    /// <summary>
    /// 取出对象
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public Object Spawn(string poolName,string assetName)
    {
        PoolBase pool = null;
        if(Pools.TryGetValue(poolName,out pool))
        {
            return pool.Spawn(assetName);
        }
        return null;
    }
    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="poolName"></param>
    /// <param name="assetName"></param>
    /// <param name="object"></param>
    public void UnSpawn(string poolName,string assetName,Object @object)
    {
        PoolBase pool = null;
        if(Pools.TryGetValue(poolName,out pool))
        {
            pool.UnSpawn(assetName, @object);
        }
    }
}
