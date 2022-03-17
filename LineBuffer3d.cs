namespace SimpleEngine
{
    internal class LineBuffer3d : ILineBuffer<Vector3>
    {
        private readonly OrderedSet<Vector3> VertexTable = new();

        private readonly HashSet<(int end1, int end2)> LineTable = new();

        OrderedSet<Vector3> ILineBuffer<Vector3>.VertexTable => VertexTable;

        HashSet<(int, int)> ILineBuffer<Vector3>.LineTable => LineTable;

        /// <summary>
        ///     Adds the line to the vertex and line tables if it doesn't already exist.
        /// </summary>
        /// <param name="x1">the first x coordinate</param>
        /// <param name="y1">the first y coordinate</param>
        /// <param name="x2">the second x coordinate</param>
        /// <param name="y2">the second y coordinate</param>
        public void AddLine(Vector3 p1, Vector3 p2)
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
                AddLine(new Vector3(coords[0], coords[1], coords[2]), new Vector3(coords[3], coords[4], coords[5]));
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
                lines[i++] = p1.x + " " + p1.y + " " + p1.z + " " + p2.x + " " + p2.y + " " + p2.z;
            }
            File.WriteAllLinesAsync(fileName, lines);
        }

        public void Execute(Action<Vector3, Vector3> function)
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
