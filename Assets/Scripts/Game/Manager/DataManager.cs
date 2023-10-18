using MT.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MT.Managers
{
    public class DataManager : MT.Util.Singleton<DataManager>
    {
        private Dictionary<int, StageData> stageDatas = null;
        private Dictionary<int, EOData> eoDatas = null;
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
        }

        internal StageData GetStageData(int level) => this.stageDatas[level];

        internal EOData GetEOData(int id) => this.eoDatas[id];

        internal Dictionary<int, StageData> GetAllStageDatas() => this.stageDatas;
    }
}
