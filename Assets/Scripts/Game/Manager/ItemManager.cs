using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Progress;

namespace MT.Managers
{
    public class ItemManager : MT.Util.Singleton<ItemManager>
    {
        /// <summary>
        /// key: EOLogic.assetName val:count
        /// </summary>
        private readonly Dictionary<string, int> items = new Dictionary<string, int>();
        private readonly Dictionary<string, Action> usefuncs = new Dictionary<string, Action>();
        public ItemManager() { }
        public void Init() { }

        public bool TryGetItem(string id, out int count) => this.items.TryGetValue(id, out count);

        internal void UseItem(MT.Event.IEventBasic item)
        {
            string id = item.GetAssetName();
            if (this.TryGetItem(id, out int count))
            {
                if (count > 0)
                {
                    this.items[id] = --count;
                    item.OnUseItem();
                }
            }
            else
                Debug.LogError($"item[id = {id}] not found!");
        }

        //TODO
        internal void UseItem(int id)
        {
            var data = DataManager.Instance.GetEOData(id);
            string sid = PathUtil.GetEOPath(data.AssetName);
            if(this.usefuncs.ContainsKey(sid))
            {
                this.usefuncs[sid].Invoke();
            }
        }

        internal void PushItem(MT.Event.IEventBasic item, int val = 1)
        {
            string id = item.GetAssetName();
            if (this.TryGetItem(id, out int count))
            {
                this.items[id] += val;
            }
            else
                this.items.Add(id, val);
        }
    }
}
