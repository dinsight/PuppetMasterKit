using System;
using PuppetMasterKit.Utility;

namespace PuppetMasterKit.Terrain.Noise
{
  public class Bicubic : INoiseGenerator
  {
    private I2DSubscript<float?> gradient;

    public int XDim => gradient.Cols;

    public int YDim => gradient.Rows;

    /// <summary>
    /// Array initialization
    /// </summary>
    /// <param name="gradient"></param>
    public Bicubic(float[,] gradient)
    {
      this.gradient = new ArraySubscript<float>(gradient);
    }

    /// <summary>
    /// Subscript function initialization
    /// </summary>
    /// <param name="gradient">Gradient.</param>
    public Bicubic(I2DSubscript<float?> gradient)
    {
      this.gradient = gradient;
    }

    /// <summary>
    /// Cubics the interpolate.
    /// </summary>
    /// <returns>The interpolate.</returns>
    /// <param name="p">P.</param>
    /// <param name="x">The x coordinate.</param>
    private float CubicInterpolate(float[] p, float x)
    {
      return (float)(p[1] + 0.5 * x * (p[2] - p[0] 
                          + x * (2.0 * p[0] - 5.0 * p[1] + 4.0 * p[2] - p[3] 
                          + x * (3.0 * (p[1] - p[2]) + p[3] - p[0]))));
    }

    /// <summary>
    /// Interpolate.
    /// </summary>
    /// <returns>The interpolate.</returns>
    /// <param name="p">P.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    private float BicubicInterpolate(float[][] p, float x, float y)
    {
      var arr = new float[4];
      arr[0] = CubicInterpolate(p[0], x);
      arr[1] = CubicInterpolate(p[1], x);
      arr[2] = CubicInterpolate(p[2], x);
      arr[3] = CubicInterpolate(p[3], x);
      return CubicInterpolate(arr, y);
    }

    /// <summary>
    /// Clamp
    /// </summary>
    /// <returns>The clamp.</returns>
    /// <param name="x">The x coordinate.</param>
    int Clamp(int x)
    {
      if (x < 0)
        return 0;
      else if (x > gradient.Rows - 1)
        return gradient.Cols - 1;
      return x;
    }

    private int currXc = -1;
    private int currYc = -1;
    private float[][] p = null;

    /// <summary>
    /// Noise the specified x and y.
    /// </summary>
    /// <returns>The noise.</returns>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public float Noise(float x, float y)
    {
      int xc = (int)x;
      int yc = (int)y;

      if (xc != currXc || yc != currYc || p == null) {
        currXc = xc;
        currYc = yc;
        p = new float[][]{
          new float[]{gradient[Clamp(yc-1),Clamp(xc-1)].Value,
                      gradient[Clamp(yc-1),Clamp(xc)].Value,
                      gradient[Clamp(yc-1),Clamp(xc+1)].Value,
                      gradient[Clamp(yc-1),Clamp(xc+2)].Value },
          new float[]{gradient[Clamp(yc),Clamp(xc-1)].Value,
                      gradient[Clamp(yc),Clamp(xc)].Value,
                      gradient[Clamp(yc),Clamp(xc+1)].Value,
                      gradient[Clamp(yc),Clamp(xc+2)].Value },
          new float[]{gradient[Clamp(yc+1),Clamp(xc-1)].Value,
                      gradient[Clamp(yc+1),Clamp(xc)].Value,
                      gradient[Clamp(yc+1),Clamp(xc+1)].Value,
                      gradient[Clamp(yc+1),Clamp(xc+2)].Value },
          new float[]{gradient[Clamp(yc+2),Clamp(xc-1)].Value,
                      gradient[Clamp(yc+2),Clamp(xc)].Value,
                      gradient[Clamp(yc+2),Clamp(xc+1)].Value,
                      gradient[Clamp(yc+2),Clamp(xc+2)].Value },
        };
      }
      return BicubicInterpolate(p, x - xc, y - yc);
    }
  }
}
