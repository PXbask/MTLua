using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MT.Util
{
    internal static class UnityUtil
    {
        public static Vector3 GetTPosition(int x, int y) => new Vector3(x + 0.5f, -y - 0.5f);
        public static Vector3 GetTPosition(Vector2Int v) => new Vector3(v.x + 0.5f, -v.y - 0.5f);
    }
}
