using MT.Data;
using MT.Event;
using MT.Managers;
using MT.Mono;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEditor.PlayerSettings;

namespace MT.Model
{
    public class MapLayout
    {
        private int level;
        private Data.StageData stageData;
        private Layout<int> backgroundLayer;
        private Layout<int> eventgroundLayer;
        private Layout<int> foregroundLayer;

        private Layout<MT.Event.IEventBasic> eventLayer;

        private readonly int DEFUALT_LEVEL = -1;

        private int Scale => stageData.Scale;
        public StageData StageData
        {
            get => stageData;
            set => stageData = value;
        }

        public MapLayout(StageData data)
        {
            this.StageData = data;
            var ints1 = data.BackGroundLayer;
            var ints2 = data.EventGroundLayer;
            var ints3 = data.FrontGroundLayer;

            this.level = DEFUALT_LEVEL;
            this.backgroundLayer = new Layout<int>(Load_BEF_Layer, Scale);
            this.eventgroundLayer = new Layout<int>(Load_BEF_Layer, Scale);
            this.foregroundLayer = new Layout<int>(Load_BEF_Layer, Scale);
            this.eventLayer = new Layout<Event.IEventBasic>(Load_Event_Layer, Scale);

            this.backgroundLayer.ReLoad(ints1);
            this.eventgroundLayer.ReLoad(ints2);
            this.foregroundLayer.ReLoad(ints3);
        }

        internal void SaveCurrentLevel()
        {
            if (this.level == DEFUALT_LEVEL) return;
            MapManager.Instance.SaveMapData(this.level, this);
        }

        /// <summary>
        /// Load eventLayer
        /// </summary>
        internal void EnterLevel()
        {
            this.eventLayer.ReLoad(this.eventgroundLayer.Data);
        }

        private int[,] Load_BEF_Layer(object data, int scale)
        {
            List<int> _data = data as List<int>;
            if (_data == null) throw new Exception("invaild loader");

            int[,] res = new int[scale, scale];
            for (int i = 0; i < _data.Count; i++)
            {
                res[i % scale, i / scale] = _data[i];
            }

            return res;
        }

        private Event.IEventBasic[,] Load_Event_Layer(object data, int scale)
        {
            IEventBasic[] _data = data as IEventBasic[];
            if (_data == null) throw new Exception("invaild loader");

            var res = new Event.IEventBasic[scale, scale];
            for (int i = 0; i < _data.Length; i++)
            {
                res[i % scale, i / scale] = _data[i];
            }

            return res;
        }

        public void LoadEventBlock(ref BlockSlot[,] slots)
        {
            List<IEventBasic> eos = new List<IEventBasic>();
            for (int i = 0; i < slots.GetLength(0); i++)
            {
                for (int j = 0; j < slots.GetLength(1); j++)
                {
                    BlockSlot target = slots[j, i];
                    //event
                    IEventBasic eo = Manager.EO.GetEventObject(this.eventgroundLayer[j, i]);
                    target.ResetEventObject(eo.GetGameObject());
                    eos.Add(eo);
                }
            }

            this.eventLayer.ReLoad(eos.ToArray());
        }

        internal void LoadBackBlock(ref BlockSlot[,] slots)
        {
            for (int i = 0; i < slots.GetLength(0); i++)
            {
                for (int j = 0; j < slots.GetLength(1); j++)
                {
                    BlockSlot target = slots[j, i];
                    var data = DataManager.Instance.GetTMData(this.backgroundLayer[j, i]);
                    Manager.Resources.LoadSprite(data.AssetName, (Sprite obj) =>
                    {
                        target.ResetBackGround(obj);
                    });
                }
            }
        }

        internal void LoadForeBlock(ref BlockSlot[,] slots)
        {

        }

        internal IEventBasic GetEventObject(int x, int y) => this.eventLayer[x, y];

        internal void ReplaceEvent(int x, int y, int v)
        {
            int eid = this.eventgroundLayer[x, y];
            IEventBasic eo = this.GetEventObject(x, y);
            Manager.Pool.UnSpawn("EventObject", eo.GetAssetName(), eo.GetGameObject());

            IEventBasic neo = Manager.EO.GetEventObject(v);
            this.eventgroundLayer.Set(x, y, v);
            this.eventLayer.Set(x, y, neo);
            MT.Mono.StageContainer.instance.GetBlockSlot(x, y).ResetEventObject(neo.GetGameObject());
        }
    }
}

