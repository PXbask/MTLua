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
        }
    }
}
