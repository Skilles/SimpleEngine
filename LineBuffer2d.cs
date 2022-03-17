using System.Linq;

namespace SimpleEngine
{
    
    internal class LineBuffer2d : ILineBuffer<Vector2>
    {

        public readonly OrderedSet<Vector2> VertexTable = new();

        public readonly HashSet<(int end1, int end2)> LineTable = new();

        OrderedSet<Vector2> ILineBuffer<Vector2>.VertexTable => VertexTable;

        HashSet<(int, int)> ILineBuffer<Vector2>.LineTable => LineTable;

        
        public void AddLine(Vector2 p1, Vector2 p2)
        {
            if (!VertexTable.Add(p1) || !VertexTable.Add(p2)) return;
            

            LineTable.Add((VertexTable.IndexOf(p1), VertexTable.IndexOf(p2)));
        }

        
        public void ReadFromFile(string filePath)
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                double[] coords = Array.ConvertAll(line.Split(' '), double.Parse);
                AddLine(new Vector2(coords[0], coords[1]), new Vector2(coords[2], coords[3]));
            }
        }

        
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
