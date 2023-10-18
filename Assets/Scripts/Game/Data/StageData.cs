using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MT.Data
{
    public class StageData
    {
        public string Bgm { get; set; }
        public int Scale { get; set; }
        public int Level { get; set; }
        public List<int> BackGroundLayer { get; set; }
        public List<int> EventGroundLayer { get; set; }
        public List<int> FrontGroundLayer { get; set; }
    }
}

