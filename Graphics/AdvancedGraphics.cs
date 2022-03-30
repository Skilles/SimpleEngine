

namespace SimpleEngine
{
    public class AdvancedGraphics : BaseGraphics
    {
        private readonly ILineBuffer<Vector3> WorldLineBuffer; // TODO store color alongside coordinates
        private readonly ILineBuffer<Vector3> StaticLineBuffer;

        private int viewportDistance;
        private Vector2 viewportSize;
        private Vector2 viewportCenter;
        private Vector3 cameraPoint;
        private double unitS;
        private double[,] vnMatrix;

        public AdvancedGraphics(DirectBitmap bitmap) : base(bitmap)
        {
            WorldLineBuffer = new LineBuffer3d();
            StaticLineBuffer = new LineBuffer3d();
            // ScreenLineBuffer = new LineBuffer2d();
            viewportSize = new Vector2(bitmap.Width / 2, bitmap.Height / 2);
            viewportCenter = new Vector2(viewportSize.x, viewportSize.y);
            UpdateSettings(new Vector3(1, 1, 1), 1, 1);
        }

        public void UpdateSettings(Vector3 cameraPoint, double unitS, int viewportDistance)
        {
            this.cameraPoint = cameraPoint;
            this.unitS = unitS;
            this.viewportDistance = viewportDistance;
            CalculateVNMatrix();
        }

        public void UpdateViewport(Vector2 viewportSize)
        {
            this.viewportSize = viewportSize;
            viewportCenter = new Vector2(viewportSize.x, viewportSize.y);
        }

        public void DrawLine(Vector3 p1, Vector3 p2, bool _static)
        {
            if (_static)
            {
                StaticLineBuffer.AddLine(p1, p2);
            }
            else
            {
                WorldLineBuffer.AddLine(p1, p2);
            }
        }

        public void LoadFromFile(string filePath)
        {
            // Load world space coordinates
            WorldLineBuffer.ReadFromFile(filePath);
        }

        public void Clear(bool screen)
        {
            if (screen) base.Clear(Color.White);
            WorldLineBuffer.Clear();
        }

        private void CalculateVNMatrix()
        {
            var nMatrix = new[,]
            {
                {viewportDistance / unitS, 0, 0, 0},
                {0, viewportDistance / unitS, 0, 0},
                {0, 0, 1, 0},
                {0, 0, 0, 1}
            };
            var t1 = new[,]
            {
                {1, 0, 0, 0},
                {0, 1, 0, 0},
                {0, 0, 1, 0},
                {-cameraPoint.x, -cameraPoint.y, -cameraPoint.z, 1}
            };
            var t2 = new double[,]
            {
                {1, 0, 0, 0},
                {0, 0, -1, 0},
                {0, 1, 0, 0},
                {0, 0, 0, 1}
            };
            var cameraXSquared = cameraPoint.x * cameraPoint.x;
            var cameraYSquared = cameraPoint.y * cameraPoint.y;
            var cameraZSquared = cameraPoint.z * cameraPoint.z;
            var c = Math.Sqrt(cameraXSquared + cameraYSquared);
            var m1 = cameraPoint.y / c;
            var m2 = cameraPoint.x / c;
            var t3 = new [,]
            {
                {-m1, 0, m2, 0},
                {0, 1, 0, 0},
                {-m2, 0, -m1, 0},
                {0, 0, 0, 1}
            };
            var m3 = cameraPoint.z / Math.Sqrt(cameraXSquared + cameraYSquared + cameraZSquared);
            var m4 = Math.Sqrt((cameraXSquared + cameraYSquared) / (cameraXSquared + cameraYSquared + cameraZSquared));
            var t4 = new [,]
            {
                {1, 0, 0, 0},
                {0, m4, m3, 0},
                {0, -m3, m4, 0},
                {0, 0, 0, 1}
            };
            var t5 = new double[,]
            {
                {1, 0, 0, 0},
                {0, 1, 0, 0},
                {0, 0, -1, 0},
                {0, 0, 0, 1}
            };
            vnMatrix = Util.ConcatMatricies(t1, t2, t3, t4, t5, nMatrix);
        }

        public void DrawFromBuffer()
        {
            // Create a copy of the buffer with the VN matrix applied (WCS -> ECS)
            var vnCopy = WorldLineBuffer.MatrixCopy(vnMatrix);
            var svnCopy = StaticLineBuffer.MatrixCopy(vnMatrix);
            // Perspective projection with Global World Space Values (ECS -> 2d)
            var vSx = (int) viewportSize.x;
            var vSy = (int) viewportSize.y;
            var vCx = viewportCenter.x;
            var vCy = viewportCenter.y;
            vnCopy.Execute((p1, p2) =>
            {
                var xS1 = p1.x / p1.z * vSx + vCx;
                var yS1 = p1.y / p1.z * vSy + vCy;
                var xS2 = p2.x / p2.z * vSx + vCx;
                var yS2 = p2.y / p2.z * vSy + vCy;
                DrawLineBresenham(new Vector2(xS1, yS1), new Vector2(xS2, yS2));
            });
            svnCopy.Execute((p1, p2) =>
            {
                var xS1 = p1.x / p1.z * vSx + vCx;
                var yS1 = p1.y / p1.z * vSy + vCy;
                var xS2 = p2.x / p2.z * vSx + vCx;
                var yS2 = p2.y / p2.z * vSy + vCy;
                DrawLineBresenham(new Vector2(xS1, yS1), new Vector2(xS2, yS2));
            });
        }

        private void ApplyMatrix(double[,] matrix)
        {
            if (matrix.Length != 16)
            {
                throw new ArgumentException("Matrix must be 4x4");
            }
            base.Clear(Color.White);
            WorldLineBuffer.Execute(p1 =>
            {
                // Grabs the verticies according to the line table
                var (x1, y1, z1) = p1;
                // Convert the coordinates into a vector
                double[,] p1Vec = {{x1, y1, z1, 1}};
                // Concatanate the position with the transformation matrix
                p1Vec = Util.ConcatMatricies(p1Vec, matrix);
                p1.x = p1Vec[0, 0];
                p1.y = p1Vec[0, 1];
                p1.z = p1Vec[0, 2];
            });
        }

        public void Translate(double tx, double ty, double tz)
        {
            ApplyMatrix(new[,]
            {
                {1, 0, 0, 0},
                {0, 1, 0, 0},
                {0, 0, 1, 0},
                {tx, ty, tz, 1}
            });
        }

        public void Scale(double sx, double sy, double sz)
        {
            ApplyMatrix(new[,]
            {
                {sx, 0, 0, 0},
                {0, sy, 0, 0},
                {0, 0, sz, 0},
                {0, 0, 0, 1}
            });
        }

        public void Scale(double sx, double sy, double sz, double Cx, double Cy, double Cz)
        {
            var matrix = Util.ConcatMatricies(
                new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0},
                    {-Cx, -Cy, -Cz, 1}
                },
                new[,]
                {
                    {sx, 0, 0, 0},
                    {0, sy, 0, 0},
                    {0, 0, sz, 0},
                    {0, 0, 0, 1}
                },
                new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0},
                    {Cx, Cy, Cz, 1}
                });
            ApplyMatrix(matrix);
        }

        public void Rotate(double angle, Axis axis)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            var matrix = axis switch
            {
                Axis.Z => new[,] {{cos, sin, 0, 0}, {-sin, cos, 0, 0}, {0, 0, 1, 0}, {0, 0, 0, 1}},
                Axis.Y => new[,] {{cos, 0, -sin, 0}, {0, 1, 0, 0}, {sin, 0, cos, 0}, {0, 0, 0, 1}},
                Axis.X => new[,] {{1, 0, 0, 0}, {0, cos, sin, 0}, {0, -sin, cos, 0}, {0, 0, 0, 1}},
            };
            ApplyMatrix(matrix);
        }
        public void Rotate(double angle, Axis axis, double Cx, double Cy, double Cz)
        {
            var cos = Math.Cos(angle);
            var sin = Math.Sin(angle);
            var rMatrix = axis switch
            {
                Axis.Z => new[,] { { cos, sin, 0, 0 }, { -sin, cos, 0, 0 }, { 0, 0, 1, 0 }, { 0, 0, 0, 1 } },
                Axis.Y => new[,] { { cos, 0, -sin, 0 }, { 0, 1, 0, 0 }, { sin, 0, cos, 0 }, { 0, 0, 0, 1 } },
                Axis.X => new[,] { { 1, 0, 0, 0 }, { 0, cos, sin, 0 }, { 0, -sin, cos, 0 }, { 0, 0, 0, 1 } },
            };
            var matrix = Util.ConcatMatricies(new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0},
                    {-Cx, -Cy, -Cz, 1}
                },
                rMatrix,
                new[,]
                {
                    {1, 0, 0, 0},
                    {0, 1, 0, 0},
                    {0, 0, 1, 0},
                    {Cx, Cy, Cz, 1}
                });
            ApplyMatrix(matrix);
        }
    }

    public enum Axis
    {
        X, Y, Z
    }
}
