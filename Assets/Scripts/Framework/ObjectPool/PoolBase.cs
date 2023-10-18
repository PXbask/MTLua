using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolBase : MonoBehaviour
{
    /// <summary>
    /// 自动释放时间
    /// </summary>
    protected float ReleaseTime;
    /// <summary>
    /// 单位：毫微秒
    /// </summary>
    protected long LastReleaseTime;
    protected List<PoolObject> Objects;
    private void Start()
    {
        LastReleaseTime = System.DateTime.Now.Ticks;
    }
    public void Init(float time)
    {
        this.ReleaseTime= time;
        Objects = new List<PoolObject>();
    }
    /// <summary>
    /// 取出对象
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public virtual Object Spawn(string name)
    {
        foreach (var obj in Objects)
        {
            if (obj.Name.Equals(name))
            {
                Objects.Remove(obj);
                return obj.Object;
            }
        }
        return null;
    }
    /// <summary>
    /// 回收对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="object"></param>
    public virtual void UnSpawn(string name,Object @object)
    {
        PoolObject obj = new PoolObject(name, @object);
        Objects.Add(obj);
    }
    public virtual void Release()
    {

    }
    private void Update()
    {
        long time = System.DateTime.Now.Ticks - LastReleaseTime;
        if (time >= ReleaseTime * 10000000)
        {
            LastReleaseTime= System.DateTime.Now.Ticks;
            this.Release();
        }
    }
}
