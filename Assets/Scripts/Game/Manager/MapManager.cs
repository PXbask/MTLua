using MT.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MT.Managers
{
    public class MapManager : MT.Util.Singleton<MapManager>
    {
        private readonly Dictionary<int, MapLayout> Maps= new Dictionary<int, MapLayout>();

        public void Init()
        {
            var stageDatas = DataManager.Instance.GetAllStageDatas();

            foreach (var data in stageDatas)
            {
                var val = data.Value;
                MapLayout layout = new MapLayout(val);
                this.Maps.Add(data.Key, layout);
            }
        }

        public MapLayout GetMapData(int _level)
        {
            MapLayout res= null;
            if (!this.Maps.TryGetValue(_level, out res)) 
                UnityEngine.Debug.LogErrorFormat("Map[level={0}] not found", _level);
            return res;
        }

        public void SaveMapData(int _level, MapLayout layout)
        {
            if (Maps.ContainsKey(_level)) Maps[_level] = layout;
            else Debug.LogError($"no maplayouts has level[{_level}]");
        }
    }
}
