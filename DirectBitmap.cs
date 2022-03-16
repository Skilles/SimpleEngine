using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SimpleEngine;

/// <summary>
///     A custom bitmap implementation that uses direct memory access for very fast pixel calls.
/// </summary>
public class DirectBitmap : IDisposable

{
    public DirectBitmap(int width, int height)
    {
        Width = width;
        Height = height;
        Bits = new int[width * height];
        BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
        Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
    }

    private DirectBitmap(DirectBitmap original)
    {
        Width = original.Width;
        Height = original.Height;
        Bits = (int[])original.Bits.Clone();
        BitsHandle = GCHandle.Alloc(Bits, GCHandleType.Pinned);
        Bitmap = new Bitmap(Width, Height, Width * 4, PixelFormat.Format32bppPArgb, BitsHandle.AddrOfPinnedObject());
    }

    public Bitmap Bitmap { get; }
    public int[] Bits { get; }
    public bool Disposed { get; private set; }
    public int Height { get; }
    public int Width { get; }

    protected GCHandle BitsHandle { get; }

    public void Dispose()
    {
        if (Disposed) return;
        Disposed = true;
        Bitmap.Dispose();
        BitsHandle.Free();
    }

    ~DirectBitmap()
    {
        Dispose();
    }

    public void SetPixel(int x, int y, Color color)
    {
        var index = x + y * Width;
        var col = color.ToArgb();

        Bits[index] = col;
    }

    public Color GetPixel(int x, int y)
    {
        var index = x + y * Width;
        var col = Bits[index];
        var result = Color.FromArgb(col);

        return result;
    }

    int[,] To2DArray()
    {
        int[,] result = new int[Width, Height];
        // Convert the 1D pixel array into 2D
        for (int y = 0; y < Height; y++)
        {
            int pY = y * Width;
            for (int x = 0; x < Width; x++)
            {
                result[x, y] = Bits[x + pY];
            }
        }
        return result;
    }

    public DirectBitmap Clone()
    {
        return new DirectBitmap(this);
    }

    public Bitmap ToGrayscale()
    {
        var result = new Bitmap(this.Width, this.Height, PixelFormat.Format8bppIndexed);

        BitmapData data = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

        // Copy the bytes from the image into a byte array
        byte[] bytes = new byte[data.Height * data.Stride];
        Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                var c = this.GetPixel(x, y);
                var rgb = (byte)((c.B + c.G + c.R) / 3);

                bytes[y * data.Stride + x] = rgb;
            }
        }

        // Copy the bytes from the byte array into the image
        Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);

        result.UnlockBits(data);

        return result;
    }

    public Bitmap ToTensorFormat()
    {
        var result = new Bitmap(this.Width, this.Height, PixelFormat.Format8bppIndexed);

        BitmapData data = result.LockBits(new Rectangle(0, 0, result.Width, result.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

        // Copy the bytes from the image into a byte array
        byte[] bytes = new byte[data.Height * data.Stride];
        Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

        for (int y = 0; y < this.Height; y++)
        {
            for (int x = 0; x < this.Width; x++)
            {
                var c = this.GetPixel(x, y);
                if (c.Name.Equals("ffffffff")) {
                    c = Color.Black;
                } else
                {
                    c = Color.White;
                }
                var rgb = (byte)((c.B + c.G + c.R) / 3);

                bytes[y * data.Stride + x] = rgb;
            }
        }

        // Copy the bytes from the byte array into the image
        Marshal.Copy(bytes, 0, data.Scan0, bytes.Length);

        result.UnlockBits(data);

        return result;
    }
}