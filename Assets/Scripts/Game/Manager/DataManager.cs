using MT.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace MT.Managers
{
    public class DataManager : MT.Util.Singleton<DataManager>, IDisposable
    {
        private string DataPath = "Assets/BuildResources/Data/";

        private Dictionary<int, StageData> stageDatas = null;
        private Dictionary<int, EOData> eoDatas = null;
        private Dictionary<int, TextureMapData> texturemapDatas = null;
        public Dictionary<string, TileMapingData> tilemapingDatas = null;
        public DataManager() { }
        public void Init()
        {
            this.LoadData();
        }

        private void LoadData()
        {
            Manager.Resources.LoadData("StageDefine", (UnityEngine.Object obj) =>
            {
                string json = (obj as UnityEngine.TextAsset).text;
                this.stageDatas = JsonConvert.DeserializeObject<Dictionary<int, StageData>>(json);
            });

            Manager.Resources.LoadData("EODefine", (UnityEngine.Object obj) =>
            {
                string json = (obj as UnityEngine.TextAsset).text;
                this.eoDatas = JsonConvert.DeserializeObject<Dictionary<int, EOData>>(json);
            });

            Manager.Resources.LoadData("TextureMapDefine", (UnityEngine.Object obj) =>
            {
                string json = (obj as UnityEngine.TextAsset).text;
                this.texturemapDatas = JsonConvert.DeserializeObject<Dictionary<int, TextureMapData>>(json);
            });
        }

#if UNITY_EDITOR_64

        public void EditorLoadData()
        {
            string json = File.ReadAllText(this.DataPath + "TileMapDefine.txt");
            this.tilemapingDatas = JsonConvert.DeserializeObject<Dictionary<string, TileMapingData>>(json);

            json = File.ReadAllText(this.DataPath + "StageDefine.txt");
            this.stageDatas = JsonConvert.DeserializeObject<Dictionary<int, StageData>>(json);
        }

        public void ExportStageData()
        {
            string json = JsonConvert.SerializeObject(this.stageDatas);
            File.WriteAllText(this.DataPath + "StageDefine.txt", json);
        }

        public void SetStageData(int level, StageData stageData) => this.stageDatas[level] = stageData;

#endif

        internal StageData GetStageData(int level) => this.stageDatas[level];

        internal EOData GetEOData(int id) => this.eoDatas[id];

        internal TextureMapData GetTMData(int id) => this.texturemapDatas[id];

        internal Dictionary<int, StageData> GetAllStageDatas() => this.stageDatas;

        internal Dictionary<int, EOData> GetAllEODatas() => this.eoDatas;

        public void Dispose()
        {
            this.tilemapingDatas = null;
            this.stageDatas = null;
        }
    }
}
