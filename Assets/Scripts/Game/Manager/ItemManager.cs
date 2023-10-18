using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using XLua;

namespace MT.Managers
{
    public class ItemManager : MT.Util.Singleton<ItemManager>
    {
        /// <summary>
        /// key: EOLogic.assetName val:count
        /// </summary>
        private readonly Dictionary<int, int> itemCounts = new Dictionary<int, int>();
        private readonly Dictionary<int, Model.Item> itemOrigins= new Dictionary<int, Model.Item>();
        public ItemManager() { }
        public void Init()
        {
            this.LoadItemData();
            Manager.Event.Subscribe((int)GameEvent.OnLuaInit, this.RegisterItems);
        }

        ~ItemManager()
        {
            Manager.Event.UnSubscribe((int)GameEvent.OnLuaInit, this.RegisterItems);
        }

        private void LoadItemData()
        {
            //TODO: from file
            this.itemCounts.Clear();
        }

        private void RegisterItems(object n)
        {
            this.itemOrigins.Clear();

            XLua.LuaEnv env = Manager.Lua.LuaEnv;
            LuaTable scriptEnv = env.NewTable();
            LuaTable meta = env.NewTable();
            meta.Set("__index", env.Global);
            scriptEnv.SetMetaTable(meta);
            meta.Dispose();

            foreach (var item in DataManager.Instance.GetAllEODatas())
            {
                Action onUse = null;
                env.DoString(Manager.Lua.GetLuaScript(item.Value.LuaName), item.Value.LuaName, scriptEnv);
                scriptEnv.Get("onUseItem", out onUse);
                if (onUse != null)
                    this.itemOrigins.Add(item.Key, new Model.Item(item.Value).SetAbility(onUse));
            }
        }

        internal bool HasItem(MT.Event.IEventBasic item) => this.itemCounts.ContainsKey(item.GetItemID()) && this.itemCounts[item.GetItemID()] > 0;

        internal bool HasItem(int id) => this.itemCounts.ContainsKey(id) && this.itemCounts[id] > 0;

        internal void UseItem(MT.Event.IEventBasic item)
        {
            int id = item.GetItemID();
            if (this.itemCounts.TryGetValue(id, out int count))
            {
                if (count > 0)
                {
                    this.itemCounts[id] = --count;
                    item.OnUseItem();
                }
            }
            else
                Debug.LogError($"item[id = {id}] not found!");
        }

        internal void UseItem(int id)
        {
            if (this.itemCounts.TryGetValue(id, out int count))
            {
                if (count > 0)
                {
                    this.itemCounts[id] = --count;
                    this.itemOrigins[id].Fire();
                }
            }
            else
                Debug.LogError($"item[id = {id}] not found!");
        }

        internal void PushItem(MT.Event.IEventBasic item, int val = 1)
        {
            int id = item.GetItemID();
            if (this.itemCounts.TryGetValue(id, out int count))
            {
                this.itemCounts[id] += val;
            }
            else
                this.itemCounts.Add(id, val);
        }

        internal void PushItem(int eid, int val = 1)
        {
            if (this.itemCounts.TryGetValue(eid, out int count))
            {
                this.itemCounts[eid] += val;
            }
            else
                this.itemCounts.Add(eid, val);
        }
    }
}
