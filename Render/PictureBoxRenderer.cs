namespace SimpleEngine;

/// <summary>
///     A wrapper for a PictureBox that uses SimpleGraphics to draw and update the box's image.
/// </summary>
public class PictureBoxRenderer
{
    private readonly DirectBitmap bitmap;

    private readonly PictureBox box;

    private readonly SimpleGraphics gfx;

    private readonly AdvancedGraphics aGfx;

    public PictureBoxRenderer(PictureBox box)
    {
        bitmap = new DirectBitmap(box.Width, box.Height);
        gfx = new SimpleGraphics(bitmap);
        aGfx = new AdvancedGraphics(bitmap);
        this.box = box;
    }

    ~PictureBoxRenderer()
    {
        gfx.Dispose();
        aGfx.Dispose();
    }

    public void DrawLinesFromFile(string filePath)
    {
        gfx.Clear(Color.White);
        gfx.LineBuffer.ReadFromFile(filePath);
        Update(false);
    }

    public void DrawFromFile3d(string filePath)
    {
        gfx.Clear(Color.White);
        aGfx.LoadFromFile(filePath);
        Update(true);
    }

    public void SaveToFile(String filePath)
    {
        gfx.LineBuffer.Dump(filePath);
    }

    /// <summary>
    ///     Clears the image by setting all pixels to black.
    /// </summary>
    public void Clear(Color color)
    {
        gfx.Clear(color);
        aGfx.Clear(false);
        box.Image?.Dispose();
        box.Image = (Bitmap)bitmap.Bitmap.Clone();
    }

    /// <summary>
    ///     Refreshes the image in the form.
    /// </summary>
    public void Update(bool advanced = false)
    {
        if (advanced)
        {
            aGfx.DrawFromBuffer();
        }
        else
        {
            gfx.DrawFromBuffer();
        }

        box.Image?.Dispose();
        box.Image = (Bitmap)bitmap.Bitmap.Clone();
    }

    public void DrawPixel(int x, int y, Color color)
    {
        gfx.DrawPixel(x, y);
    }

    public void DrawPixel(int x, int y, int size, Color color)
    {
        if (size > 1)
        {
            gfx.DrawCircle(x, y, size - 1, color);
        }
        else
        {
            gfx.DrawPixel(x, y, color);
        }
    }

    public void DrawPixelAndUpdate(int x, int y, int size, Color color)
    {
        DrawPixel(x, y, size, color);
        Update(false);
    }

    public void DrawLine(Vector2 p1, Vector2 p2, Color color)
    {
        gfx.DrawLine(p1, p2);
    }

    // Line that is unaffected by 3d transformations
    public void DrawLine(Vector3 p1, Vector3 p2, Color color, bool _static = true)
    {
        aGfx.DrawLine(p1, p2, _static);
    }

    public void DrawGraph()
    {
        var originV = new Vector3(0, 0, 0);
        var xV = new Vector3(50, 0, 0);
        var yV = new Vector3(0, 50, 0);
        var zV = new Vector3(0, 0, 50);
        DrawLine(originV, xV, Color.Red);
        DrawLine(originV, yV, Color.Green);
        DrawLine(originV, zV, Color.Blue);
    }

    public DirectBitmap GetBitmap()
    {
        return gfx.Bitmap;
    }

    public SimpleGraphics GetGraphics()
    {
        return gfx;
    }

    public AdvancedGraphics Get3dGraphics()
    {
        return aGfx;
    }

    public void Update3dRenderer(Vector3 cameraPoint, double unitS, int viewportDistance, Vector2 viewportSize)
    {
        aGfx.UpdateSettings(cameraPoint, unitS, viewportDistance);
        aGfx.UpdateViewport(viewportSize);
    }
}