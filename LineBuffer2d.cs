using System.Linq;

namespace SimpleEngine
{
    /// <summary>
    ///     Stores verticies and edges in geometric tables to avoid unneccessary processing. Stores line coordinates as doubles to preserve detail when resizing/rotating.
    /// </summary>
    internal class LineBuffer2d : ILineBuffer<Vector2>
    {

        public readonly OrderedSet<Vector2> VertexTable = new();

        public readonly HashSet<(int end1, int end2)> LineTable = new();

        OrderedSet<Vector2> ILineBuffer<Vector2>.VertexTable => VertexTable;

        HashSet<(int, int)> ILineBuffer<Vector2>.LineTable => LineTable;

        /// <summary>
        ///     Adds the line to the vertex and line tables if it doesn't already exist.
        /// </summary>
        /// <param name="x1">the first x coordinate</param>
        /// <param name="y1">the first y coordinate</param>
        /// <param name="x2">the second x coordinate</param>
        /// <param name="y2">the second y coordinate</param>
        public void AddLine(Vector2 p1, Vector2 p2)
        {
            if (!VertexTable.Add(p1) || !VertexTable.Add(p2)) return;
            

            LineTable.Add((VertexTable.IndexOf(p1), VertexTable.IndexOf(p2)));
        }

        /// <summary>
        ///     Reads lines from a file with the format: x1 x2 y1 y2
        /// </summary>
        /// <param name="filePath">the path of the file to load</param>
        public void ReadFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                double[] coords = Array.ConvertAll(line.Split(' '), double.Parse);
                AddLine(new Vector2(coords[0], coords[1]), new Vector2(coords[2], coords[3]));
            }
        }

        /// <summary>
        ///     Dumps the current lines to a file in the format: x1 x2 y1 y2
        /// </summary>
        /// <param name="fileName"></param>
        public void Dump(string fileName)
        {
            string[] lines = new string[LineTable.Count];
            int i = 0;
            foreach ((int end1, int end2) in LineTable)
            {
                var p1 = VertexTable.ElementAt(end1);
                var p2 = VertexTable.ElementAt(end2);
                lines[i++] = p1.x + " " + p1.y + " " + p2.x + " " + p2.y;
            }
            File.WriteAllLinesAsync(fileName, lines);
        }

        public void Execute(Action<Vector2, Vector2> function)
        {
            foreach ((int end1, int end2) in LineTable)
            {
                var p1 = VertexTable.ElementAt(end1);
                var p2 = VertexTable.ElementAt(end2);
                function(p1, p2);
            }
        }

        public void Clear()
        {
            VertexTable.Clear();
            LineTable.Clear();
        }
    }
}
