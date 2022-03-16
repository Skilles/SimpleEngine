using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEngine
{
    internal interface ILineBuffer<T> where T : Vector
    {
        public OrderedSet<T> VertexTable { get; }
        public HashSet<(int, int)> LineTable { get; }

        public void Dump(string fileName);

        public void ReadFromFile(string filePath);

        public void AddLine(T p1, T p2);

        public void Execute(Action<T, T> function);

        public void Clear();
    }
}
