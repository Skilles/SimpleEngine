namespace SimpleEngine
{
    internal class LineBuffer3d : ILineBuffer<Vector3>
    {
        private readonly OrderedSet<Vector3> VertexTable = new();

        private readonly HashSet<(int end1, int end2)> LineTable = new();

        OrderedSet<Vector3> ILineBuffer<Vector3>.VertexTable => VertexTable;

        HashSet<(int, int)> ILineBuffer<Vector3>.LineTable => LineTable;

        public LineBuffer3d() { }
        private LineBuffer3d(LineBuffer3d original)
        {
            // Deep copy
            foreach (var (p1, p2) in original.LineTable)
            {
                var v1 = original.VertexTable.ElementAt(p1);
                var v2 = original.VertexTable.ElementAt(p2);
                AddLine(new Vector3(v1.x, v1.y, v1.z), new Vector3(v2.x, v2.y, v2.z));
            }
        }

        /// <summary>
        ///     Adds the line to the vertex and line tables if it doesn't already exist.
        /// </summary>
        /// <param name="x1">the first x coordinate</param>
        /// <param name="y1">the first y coordinate</param>
        /// <param name="x2">the second x coordinate</param>
        /// <param name="y2">the second y coordinate</param>
        public void AddLine(Vector3 p1, Vector3 p2)
        {
            VertexTable.Add(p1);
            VertexTable.Add(p2);
            
            LineTable.Add((VertexTable.IndexOf(p1), VertexTable.IndexOf(p2)));
        }

        /// <summary>
        ///     Reads lines from a file with the format: x1 x2 y1 y2
        /// </summary>
        /// <param name="filePath">the path of the file to load</param>
        public void ReadFromFile(string filePath)
        {
            Clear();
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

        public void Execute(Action<Vector3> function)
        {
            foreach (var p in VertexTable)
            {
                function(p);
            }
        }

        public void Execute(Action<Vector3, Vector3> function)
        {
            foreach (var (p1, p2) in LineTable)
            {
                function(VertexTable.ElementAt(p1), VertexTable.ElementAt(p2));
            }
        }

        public ILineBuffer<Vector3> MatrixCopy(double[,] matrix)
        {
            var copy = new LineBuffer3d(this);
            copy.Execute(p1 =>
            {
                // Grabs the verticies according to the line table
                var (x1, y1, z1) = p1;
                // Convert the coordinates into a vector
                double[,] p1Vec = { { x1, y1, z1, 1 } };
                // Concatenate with the VN Matrix
                p1Vec = Util.ConcatMatricies(p1Vec, matrix);
                // Set the new values
                p1.x = p1Vec[0, 0];
                p1.y = p1Vec[0, 1];
                p1.z = p1Vec[0, 2];
            });
            return copy;
        }

        public void Clear()
        {
            VertexTable.Clear();
            LineTable.Clear();
        }
    }
}
