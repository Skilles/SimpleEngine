

namespace SimpleEngine
{

    public class Vector3 : Vector
    {
        public double x;
        public double y;
        public double z;

        public Vector3(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void Deconstruct(out double x, out double y, out double z)
        {
            x = this.x;
            y = this.y;
            z = this.z;
        }
    }
}
