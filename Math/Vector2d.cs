

namespace SimpleEngine
{
    public interface Vector {}

    public class Vector2 : Vector, IComparable<Vector2>
    {
        public double x;
        public double y;

        public Vector2(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public void Deconstruct(out double x, out double y)
        {
            x = this.x;
            y = this.y;
        }

        public override string ToString()
        {
            return $"Vector2({x}, {y})";
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Vector2) obj);
        }


        protected bool Equals(Vector2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public int CompareTo(Vector2? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var xComparison = x.CompareTo(other.x);
            if (xComparison != 0) return xComparison;
            return y.CompareTo(other.y);
        }
    }


}
