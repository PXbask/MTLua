using MT.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Util
{
    [XLua.LuaCallCSharp]
    internal static class LuaUtil
    {
        public static MTRuntime.RuntimeData GetRuntimeData() => MTRuntime.RuntimeData.Cur(); 

        public static class Player
        {
            private static readonly PlayerLogic ins = MTRuntime.instance.Player;
            private static readonly ItemManager bag = MT.Managers.ItemManager.Instance;
            public static void Dohurt(int hurt)
            {
                ins.OnHurt(hurt);
            }
            public static void Sethp(int v)
            {
                ins.Curhp = v;
            }
            public static void Setatk(int v)
            {
                ins.Curatk = v;
            }
            public static void Setdef(int v)
            {
                ins.Curdef = v;
            }
            public static bool HasItem(int eid) => bag.HasItem(eid);
            public static void UseItem(int eid) => bag.UseItem(eid);
        }
    }
}
