﻿namespace SimpleEngine;

/// <summary>
///     An extension of the BaseGraphics that supports various vector-based transformations using a line buffer.
/// </summary>
public class SimpleGraphics : BaseGraphics
{
    internal readonly LineBuffer2d LineBuffer;

    public SimpleGraphics(DirectBitmap bitmap) : base(bitmap)
    {
        LineBuffer = new LineBuffer2d();
    }

    public override void Clear(Color color)
    {
        base.Clear(color);
        LineBuffer.Clear();
    }

    /// <summary>
    ///     Draws every line in the line buffer.
    /// </summary>
    public void DrawFromBuffer()
    {
        LineBuffer.Execute(DrawLineBresenham);
    }

    /// <summary>
    ///     Draw's a line to the specified coordinates using the underlying line buffer
    /// </summary>
    /// <param name="x1"></param>
    /// <param name="y1"></param>
    /// <param name="x2"></param>
    /// <param name="y2"></param>
    public void DrawLine(Vector2 p1, Vector2 p2)
    {
        LineBuffer.AddLine(p1, p2);
    }

    /* Transformations */

    /// <summary>
    ///     Applies a transformation matrix to all lines in the line buffer.
    /// </summary>
    /// <param name="matrix">the transformation matrix to apply</param>
    private void ApplyMatrix(int[,] matrix)
    {
        if (matrix.Length != 9)
        {
            throw new ArgumentException("Matrix must be 3x3");
        }
        base.Clear(Color.White);
        LineBuffer.Execute((Vector2 p1, Vector2 p2) =>
        {
            // Grabs the verticies according to the line table
            int x1 = (int) p1.x;
            int y1 = (int) p1.y;
            int x2 = (int) p2.x;
            int y2 = (int) p2.y;
            // Convert the coordinates into a vector
            int[,] posVector1 = { { x1, y1, 1 } };
            // Concatanate the position with the transformation matrix
            int[,] product1 = Util.ConcatMatricies(posVector1, matrix);
            int nX1 = product1[0, 0];
            int nY1 = product1[0, 1];
            // Repeat with the second pair set of coordinates
            int[,] posVector2 = { { x2, y2, 1 } };
            int[,] product2 = Util.ConcatMatricies(posVector2, matrix);
            int nX2 = product2[0, 0];
            int nY2 = product2[0, 1];
            p1.x = nX1;
            p1.y = nY1;
            p2.x = nX2;
            p2.y = nY2;
        });

        // DirectBitmap original = bitmap.Clone();
        // // Clear the line buffer and store a copy of the old values
        // ISet<(int end1, int end2)> orig_line_table = new HashSet<(int end1, int end2)>(LineBuffer._line_table);
        // IList<(double x, double y)> orig_vertex_list = new List<(double x, double y)>(LineBuffer._vertex_list);
        // Clear(Color.White);
        // // Go through each line in the line table
        // foreach ((int end1, int end2) in orig_line_table)
        // {
        //     // Grabs the verticies according to the line table
        //     (double x1, double y1) = orig_vertex_list.ElementAt(end1);
        //     (double x2, double y2) = orig_vertex_list.ElementAt(end2);
        //     // Convert the coordinates into a vector
        //     int[,] posVector1 = { { (int)x1, (int)y1, 1 } };
        //     // Concatanate the position with the transformation matrix
        //     int[,] product1 = Util.ConcatMatricies(posVector1, matrix);
        //     int nX1 = product1[0, 0];
        //     int nY1 = product1[0, 1];
        //     // Repeat with the second pair set of coordinates
        //     int[,] posVector2 = { { (int)x2, (int)y2, 1 } };
        //     int[,] product2 = Util.ConcatMatricies(posVector2, matrix);
        //     int nX2 = product2[0, 0];
        //     int nY2 = product2[0, 1];
        //     // Finally add the new line into the line buffer
        //     LineBuffer.AddLine(nX1, nY1, nX2, nY2);
        // }
        //
        // original.Dispose();
    }

    private void ApplyMatrix(double[,] matrix)
    {
        if (matrix.Length != 9)
        {
            throw new ArgumentException("Matrix must be 3x3");
        }
        base.Clear(Color.White);
        LineBuffer.Execute((p1, p2) =>
        {
            // Grabs the verticies according to the line table
            (var x1, var y1) = p1;
            (var x2, var y2) = p2;
            // Convert the coordinates into a vector
            double[,] posVector1 = { { x1, y1, 1 } };
            // Concatanate the position with the transformation matrix
            double[,] product1 = Util.ConcatMatricies(posVector1, matrix);
            double nX1 = product1[0, 0];
            double nY1 = product1[0, 1];
            // Repeat with the second pair set of coordinates
            double[,] posVector2 = { { x2, y2, 1 } };
            double[,] product2 = Util.ConcatMatricies(posVector2, matrix);
            double nX2 = product2[0, 0];
            double nY2 = product2[0, 1];
            p1.x = nX1;
            p1.y = nY1;
            p2.x = nX2;
            p2.y = nY2;
        });
        // DirectBitmap original = bitmap.Clone();
        // ISet<(int end1, int end2)> orig_line_table = new HashSet<(int end1, int end2)>(LineBuffer._line_table);
        // IList<(double x, double y)> orig_vertex_list = new List<(double x, double y)>(LineBuffer._vertex_list);
        // Clear(Color.White);
        // foreach ((int end1, int end2) in orig_line_table)
        // {
        //     (double x1, double y1) = orig_vertex_list.ElementAt(end1);
        //     (double x2, double y2) = orig_vertex_list.ElementAt(end2);
        //     double[,] posVector1 = new double[,] { { x1, y1, 1 } };
        //     double[,] product1 = Util.ConcatMatricies(posVector1, matrix);
        //     double nX1 = product1[0, 0];
        //     double nY1 = product1[0, 1];
        //     double[,] posVector2 = new double[,] { { x2, y2, 1 } };
        //     double[,] product2 = Util.ConcatMatricies(posVector2, matrix);
        //     double nX2 = product2[0, 0];
        //     double nY2 = product2[0, 1];
        //     LineBuffer.AddLine(nX1, nY1, nX2, nY2);
        // }
        //
        // original.Dispose();
    }

    /// <summary>
    ///     Applies a transformation matrix to every pixel.
    /// </summary>
    /// <param name="matrix">the transformation matrix to apply</param>
    private void ApplyMatrixPerPixel(int[,] matrix)
    {
        if (matrix.Length != 9)
        {
            throw new ArgumentException("Matrix must be 3x3");
        }
        var original = bitmap.Clone();
        Clear(Color.White);
        for (int x = 0; x < bitmap.Width; x++)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                int[,] posVector = { { x, y, 1 } };
                int[,] product = Util.ConcatMatricies(posVector, matrix);
                int nX = product[0, 0];
                int nY = product[0, 1];
                DrawPixel(nX, nY, original.GetPixel(x, y));
            }
        }

        original.Dispose();
    }

    private void ApplyMatrixPerPixel(double[,] matrix)
    {
        if (matrix.Length != 9)
        {
            throw new ArgumentException("Matrix must be 3x3");
        }
        var original = bitmap.Clone();
        Clear(Color.White);
        for (int x = 0; x < bitmap.Width; x++)
        {
            for (int y = 0; y < bitmap.Height; y++)
            {
                double[,] posVector = { { x, y, 1 } };
                double[,] product = Util.ConcatMatricies(posVector, matrix);
                int nX = (int)product[0, 0];
                int nY = (int)product[0, 1];
                DrawPixel(nX, nY, original.GetPixel(x, y));
            }
        }

        original.Dispose();
    }

    /// <summary>
    ///     Moves the image by the specified offsets.
    /// </summary>
    /// <param name="Tx">the amount to move in the x direction</param>
    /// <param name="Ty">the amount to move in the y direction</param>
    public void Translate(int Tx, int Ty)
    {
        ApplyMatrix(new[,] {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { Tx, Ty, 1 }
        });
    }

    /// <summary>
    ///     Resizes the image by the specified ratios. The origin is 0,0.
    /// </summary>
    /// <param name="Sx">the factor to resize in the x direction</param>
    /// <param name="Sy">the factor to resize in the y direction</param>
    public void Scale(double Sx, double Sy)
    {
        ApplyMatrix(new[,] {
            { Sx, 0, 0 },
            { 0, Sy, 0 },
            { 0, 0, 1 }
        });
    }

    /// <summary>
    ///     Resizes the image by the specified ratios at the specified origin.
    /// </summary>
    /// <param name="Sx">the factor to resize in the x direction</param>
    /// <param name="Sy">the factor to resize in the y direction</param>
    /// <param name="Cx">the x coordinate of the origin</param>
    /// <param name="Cy">the y coordinate of the origin</param>
    public void Scale(double Sx, double Sy, double Cx, double Cy)
    {
        var transMatrix = Util.ConcatMatricies(
            new[,] { 
                { 1, 0, 0 }, 
                { 0, 1, 0 }, 
                { -Cx, -Cy, 1 } 
            },
            new[,] { 
                { Sx, 0, 0 }, 
                { 0, Sy, 0 }, 
                { 0, 0, 1 } 
            },
            new[,] { 
                { 1, 0, 0 }, 
                { 0, 1, 0 }, 
                { Cx, Cy, 1 } 
            });
        ApplyMatrix(transMatrix);
    }

    /// <summary>
    ///     Rotates the image by the specified angle. The origin is 0,0.
    /// </summary>
    /// <param name="angle">the angle in degrees</param>
    public void Rotate(double angle)
    {
        var cos = Math.Cos(angle * Math.PI / 180);
        var sin = Math.Sin(angle * Math.PI / 180);
        ApplyMatrix(new[,] { 
            { cos, -sin, 0 }, 
            { sin, cos, 0 }, 
            { 0, 0, 1 } 
        });
    }

    /// <summary>
    ///     Rotates the image by the specified angle at the specified origin.
    /// </summary>
    /// <param name="angle">the angle in degrees</param>
    /// <param name="Cx"></param>
    /// <param name="Cy"></param>
    public void Rotate(double angle, int Cx, int Cy)
    {
        var cos = Math.Cos(Math.PI * angle / 180);
        var sin = Math.Sin(Math.PI * angle / 180);
        var transMatrix = Util.ConcatMatricies(
            new double[,] {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { -Cx, -Cy, 1 }
            },
            new[,] {
                { cos, -sin, 0 },
                { sin, cos, 0 },
                { 0, 0, 1 } 
            },
            new double[,] {
                { 1, 0, 0 },
                { 0, 1, 0 },
                { Cx, Cy, 1 }
            });
        ApplyMatrix(transMatrix);
    }
}