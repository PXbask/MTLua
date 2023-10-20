using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MT.Data
{
    [Serializable]
    public struct TeleportData
    {
        public Vector2Int start;
        public Vector2Int position;
        public Vector2Int direction;
        public int level;
        
        public static TeleportData Empty
        {
            get
            {
                return new TeleportData()
                {
                    start = Vector2Int.zero,
                    position = Vector2Int.zero,
                    direction = Vector2Int.zero,
                    level = 0
                };
            }
        }
    }
}
