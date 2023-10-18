using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MT.Data;
using MT.Model;
using System;
using MT.Managers;
using Unity.VisualScripting;

namespace MT.Mono
{
    public class StageContainer : MonoBehaviour
    {
        private int level;
        private BlockSlot[,] slots;
        public MapLayout map;
        private MT.Data.StageData stageData;

        public static StageContainer instance = null;
        private readonly int DEFAULT_LEVEL = -1;

        public int Scale => stageData.Scale;

        private void Awake()
        {
            //this.BuildMap();
            instance = this;
        }

        internal void Init()
        {
            this.level = DEFAULT_LEVEL;
            this.map = null;
        }

        private void RegisterSlots()
        {
            if (this.slots == null)
            {
                int _scale = Scale;
                this.slots = new BlockSlot[_scale, _scale];
                for (int i = 0; i < transform.childCount; i++)
                {
                    slots[i % _scale, i / _scale] = transform.GetChild(i).GetComponent<BlockSlot>();
                }
            }
        }

        public BlockSlot GetBlockSlot(int row, int col) => this.slots[row, col];

        public void EnterLevel(int level, out StageData res)
        {
            if (this.map != null)
                this.map.SaveCurrentLevel();
            this.level = level;

            this.map = Managers.MapManager.Instance.GetMapData(level);
            //this.map.EnterLevel();
            res = map.StageData;
            this.stageData = res;

            if(this.slots == null)
                RegisterSlots();
            this.Refresh();
        }

        private void Refresh()
        {
            this.RefreshBlockSlots();
        }

        private void RefreshBlockSlots()
        {
            this.map.LoadEventBlock(ref this.slots);
        }

#if UNITY_EDITOR_64
        private void BuildMap()
        {
            Transform parent = GameObject.Find("Map").transform;
            GameObject obj = Resources.Load<GameObject>(Gobal.GameConsts.BLOCKSLOT_PATH);

            for (int i = 0; i < Gobal.GameConsts.MAP_SCALE; i++)
            {
                for (int j = 0; j < Gobal.GameConsts.MAP_SCALE; j++)
                {
                    GameObject _o = Instantiate(obj, parent);
                    _o.name = $"slot({j},{i})";
                    var c = _o.GetComponent<BlockSlot>().SetPos(new Vector2Int(j, i));
                }
            }
        }
#endif
        internal bool CanMove(int x, int y, out int damage)
        {
            damage = 0;
            int scale = Scale;
            if (x < 0 || y < 0 || x >= scale || y >= scale) return false;

            var target = map.GetEventObject(x, y);
            if (!target.CanMove()) return false;
            if (target.Belong(Event.EventObjectType.Empty)) return true;
            if (target.Belong(Event.EventObjectType.Stair)) return true;
            if (target.Belong(Event.EventObjectType.Enemy))
            {
                var enemy = target as EventObjectLogic;
                return enemy.CanBattleable(out damage);
            }
            if (target.Belong(Event.EventObjectType.Item)) return true;
            if (target.Belong(Event.EventObjectType.Door)) return true;
            return false;
        }

        internal bool EventPreprocess(MTRuntime.ExpectData expect)
        {
            Vector2Int expos = expect.expos;
            try
            {
                var target = map.GetEventObject(expos.x, expos.y);
                if (target.Belong(Event.EventObjectType.Empty))
                {
                    target.OnbeforeOverlay();
                }
                if (target.Belong(Event.EventObjectType.Stair))
                {
                    target.OnbeforeChangeFloor();
                }
                if (target.Belong(Event.EventObjectType.Enemy))
                {
                    target.OnbeforeBattle();
                }
                if (target.Belong(Event.EventObjectType.Item))
                {
                    target.OnbeforeGetItem();
                }
                if (target.Belong(Event.EventObjectType.Door))
                {
                    target.OnbeforeOpenDoor();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            return true;
        }

        internal bool EventProcess(MTRuntime.ExpectData expect)
        {
            Vector2Int expos = expect.expos;
            try
            {
                var target = map.GetEventObject(expos.x, expos.y);
                if (target.Belong(Event.EventObjectType.Empty))
                {
                    target.OnOverlay();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            return true;
        }

        internal bool EventProprocess(MTRuntime.ExpectData expect)
        {
            Vector2Int expos = expect.expos;
            try
            {
                var target = map.GetEventObject(expos.x, expos.y);
                if (target.Belong(Event.EventObjectType.Empty))
                {
                    target.OnafterOverlay();
                }
                if (target.Belong(Event.EventObjectType.Stair))
                {
                    target.OnChangeFloor();
                }
                if (target.Belong(Event.EventObjectType.Enemy))
                {
                    target.OnafterBattle();
                    this.ReplaceEO(expos.x, expos.y, Gobal.GameConsts.EMPTY_BLOCK_EOID);
                }
                if (target.Belong(Event.EventObjectType.Item))
                {
                    if (target.EffectAtOnce())
                    {
                        target.OnUseItem();
                    }
                    else
                    {
                        MTRuntime.instance.bag.PushItem(target);
                    }
                    target.OnafterGetItem();
                    this.ReplaceEO(expos.x, expos.y, Gobal.GameConsts.EMPTY_BLOCK_EOID);
                }
                if (target.Belong(Event.EventObjectType.Door))
                {
                    target.OnafterOpenDoor();
                    this.ReplaceEO(expos.x, expos.y, Gobal.GameConsts.EMPTY_BLOCK_EOID);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
            return true;
        }

        private void ReplaceEO(int x, int y, int v)
        {
            this.map.ReplaceEvent(x, y, v);
        }
    }
}

