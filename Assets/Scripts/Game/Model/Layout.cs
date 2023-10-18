using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Model
{
    public class Layout<T>
    {
        public delegate T[,] LayoutLoader(object data, int scale);
        private event LayoutLoader OnLayoutLoading;

        private int scale;
        private T[,] data;
        public T[,] Data
        {
            get => this.data;
            set => this.data = value;
        }
        public Layout() { }
        public Layout(LayoutLoader loader, int scale)
        {
            this.scale = scale;
            this.OnLayoutLoading = loader;
        }
        public int Scale => this.scale;

        public T this[int x, int y] => this.data[x, y];

        public void ReLoad(object newdata)
        {
            this.data = this.OnLayoutLoading.Invoke(newdata, scale);
        }

        public void Set(int x, int y, T v)
        {
            this.data[x, y] = v;
        }
    }
}
