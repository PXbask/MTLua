using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Model
{
    internal class Item
    {
        public int id;
        public MT.Data.EOData eodata;
        private Action onUse = null;

        public Item(MT.Data.EOData eodata)
        {
            this.id = eodata.ID;
            this.eodata= eodata;
        }

        public Item SetAbility(Action action)
        {
            this.onUse = action;
            return this;
        }

        public void Fire() => this.onUse?.Invoke();
    }
}
