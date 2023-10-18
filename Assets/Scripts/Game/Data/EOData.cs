using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Data
{
    internal class EOData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string AssetName { get; set; }
        /// <summary>
        /// 组名，影响运行时存放在对象池时的父物体
        /// </summary>
        public string Group { get; set; }
        /// <summary>
        /// 相关联的Lua脚本名
        /// </summary>
        public string LuaName { get; set; }
    }
}
