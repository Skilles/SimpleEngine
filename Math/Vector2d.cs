

namespace SimpleEngine
{
    public interface Vector {}

    public class Vector2 : Vector
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
    }


}
