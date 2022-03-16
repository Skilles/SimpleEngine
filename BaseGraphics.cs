namespace SimpleEngine
{
    /// <summary>
    ///     A wrapper for a DirectBitmap that can draw lines using various algorithms.
    /// </summary>
    public abstract class BaseGraphics : IDisposable
    {
        private readonly Color _drawColor;
        private protected readonly DirectBitmap bitmap;

        public DirectBitmap Bitmap => bitmap;

        public BaseGraphics(DirectBitmap bitmap)
        {
            this.bitmap = bitmap;
            _drawColor = Color.Black;
        }

        public void Dispose()
        {
            Bitmap.Dispose();
        }

        ~BaseGraphics()
        {
            Dispose();
        }

        /// <summary>
        ///     Sets every pixel to the specified color.
        /// </summary>
        /// <param name="color"></param>
        public virtual void Clear(Color color)
        {
            var col = color.ToArgb();
            Array.Fill(Bitmap.Bits, col);
        }

        /// <summary>
        ///     Draw's a pixel at the specified coordinate using the default draw color.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DrawPixel(int x, int y)
        {
            if (x > 0 && x < Bitmap.Width && y > 0 && y < Bitmap.Height)
            {
                Bitmap.SetPixel(x, y, _drawColor);
            }
        }

        public void DrawPixel(int x, int y, Color color)
        {
            if (x > 0 && x < Bitmap.Width && y > 0 && y < Bitmap.Height)
            {
                Bitmap.SetPixel(x, y, color);
            }
        }

        public void DrawCircle(int ox, int oy, int radius, Color color)
        {
            for (int x = -radius; x < radius; x++)
            {
                int height = (int)Math.Sqrt(radius * radius - x * x);

                for (int y = -height; y < height; y++)
                    DrawPixel(x + ox, y + oy, color);
            }
        }

        private void DrawLineNaive(Vector2 p1, Vector2 p2)
        {
            int x1 = (int)p1.x;
            int y1 = (int)p1.y;
            int x2 = (int)p2.x;
            int y2 = (int)p2.y;
            float dX = x2 - x1;
            float dY = y2 - y1;
            var m = dY / dX;
            var b = (int)(y1 - m * x1);
            var x = Math.Min(x1, x2);
            var y = Math.Min(y1, y2);
            int maxc;

            if (dX == 0)
            {
                maxc = Math.Max(y1, y2);
                DrawPixel(x, y);
                while (y < maxc)
                {
                    y++;
                    DrawPixel(x, y);
                }
            }
            else if (dY == 0)
            {
                maxc = Math.Max(x1, x2);
                DrawPixel(x, y);
                while (x < maxc)
                {
                    x++;
                    DrawPixel(x, y);
                }
            }
            else if (Math.Abs(m) < 1)
            {
                maxc = Math.Max(x1, x2);
                while (x <= maxc)
                {
                    x++;
                    y = (int)(m * x + b);
                    DrawPixel(x, (int)(y + 0.5));
                }
            }
            else
            {
                maxc = Math.Max(y1, y2);
                while (y <= maxc)
                {
                    y++;
                    x = (int)((y - b) / m);
                    DrawPixel((int)(x + 0.5), y);
                }
            }
        }

        internal void DrawLineBresenham(Vector2 p1, Vector2 p2)
        {
            int x1 = (int)p1.x;
            int y1 = (int)p1.y;
            int x2 = (int)p2.x;
            int y2 = (int)p2.y;
            int x, y;
            int dx, dy;
            int incx, incy;
            int balance;

            if (x2 >= x1)
            {
                dx = x2 - x1;
                incx = 1;
            }
            else
            {
                dx = x1 - x2;
                incx = -1;
            }

            if (y2 >= y1)
            {
                dy = y2 - y1;
                incy = 1;
            }
            else
            {
                dy = y1 - y2;
                incy = -1;
            }

            x = x1;
            y = y1;

            if (dx >= dy)
            {
                dy <<= 1;
                balance = dy - dx;
                dx <<= 1;

                while (x != x2)
                {
                    DrawPixel(x, y);
                    if (balance >= 0)
                    {
                        y += incy;
                        balance -= dx;
                    }

                    balance += dy;
                    x += incx;
                }

                DrawPixel(x, y);
            }
            else
            {
                dx <<= 1;
                balance = dx - dy;
                dy <<= 1;

                while (y != y2)
                {
                    DrawPixel(x, y);
                    if (balance >= 0)
                    {
                        x += incx;
                        balance -= dy;
                    }

                    balance += dx;
                    y += incy;
                }

                DrawPixel(x, y);
            }
        }

        /// <summary>
        ///     Draw's a line to the specified coordinates with an option for the Bresenham algorithm
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="bresenham">whether to use the Bresenham algorithm</param>
        private protected void DrawLine(Vector2 p1, Vector2 p2, bool bresenham)
        {
            if (bresenham)
                DrawLineBresenham(p1, p2);
            else
                DrawLineNaive(p1, p2);
        }
    }
}
