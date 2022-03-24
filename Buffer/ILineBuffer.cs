using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleEngine
{
    /// <summary>
    ///     Stores verticies and edges in geometric tables to avoid unneccessary processing. Stores line coordinates as doubles to preserve detail when resizing/rotating.
    /// </summary>
    internal interface ILineBuffer<T> where T : Vector
    {
        public OrderedSet<T> VertexTable { get; }

        public HashSet<(int, int)> LineTable { get; }

        /// <summary>
        ///     Dumps the current lines to a file in the format: x1 x2 y1 y2
        /// </summary>
        /// <param name="fileName"></param>
        public void Dump(string fileName);

        /// <summary>
        ///     Reads lines from a file with the format: x1 x2 y1 y2
        /// </summary>
        /// <param name="filePath">the path of the file to load</param>
        public void ReadFromFile(string filePath);

        /// <summary>
        ///     Adds the line to the vertex and line tables if it doesn't already exist.
        /// </summary>
        /// <param name="p1">the first point</param>
        /// <param name="p2">the second point</param>
        public void AddLine(T p1, T p2);


        /// <summary>
        ///     Executes a function on every vertex.
        /// </summary>
        /// <param name="function">A function that takes one vector</param>
        public void Execute(Action<T> function);

        /// <summary>
        ///     Executes a function on every line.
        /// </summary>
        /// <param name="function">A function that takes two vectors</param>
        public void Execute(Action<T, T> function);


        public void Clear();
    }
}
