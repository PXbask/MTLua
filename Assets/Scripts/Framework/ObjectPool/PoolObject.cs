using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject
{
    /// <summary>
    /// 具体对象
    /// </summary>
    public Object Object;
    public string Name;
    public System.DateTime LastUseTime;
    public PoolObject(string Name,Object obj)
    {
        this.Name = Name;
        this.Object = obj;
        LastUseTime = System.DateTime.Now;
    }
}
