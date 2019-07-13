using System;
using PuppetMasterKit.Utility.Subscript;

namespace PuppetMasterKit.Utility.Noise
{
  public class Perlin : INoiseGenerator
  {
    private ArraySubscript<float> gradient;

    /// <summary>
    /// 
    /// </summary>
    private ArraySubscript<float> Gradient { get => gradient; set => gradient = value; }

    public int XDim => Gradient.Cols;

    public int YDim => Gradient.Rows;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gradient"></param>
    public Perlin(float[,] gradient)
    {
      this.Gradient = new ArraySubscript<float>(gradient);
    }

    private double Lerp(float a0, float a1, float w)
    {
      //  return (1.0 - w)*a0 + w*a1;
      // as an alternative, this slightly faster equivalent formula can be used:
      return a0 + w * (a1 - a0);
    }

    // Computes the dot product of the distance and gradient vectors.
    double DotProduct(int ix, int iy, float x, float y)
    {

      // Precomputed (or otherwise) gradient vectors at each grid node
      // Compute the distance vector
      float dx = x - (float)ix;
      float dy = y - (float)iy;

      // Compute the dot-product
      return (dx * Gradient[iy,ix].Value + dy * Gradient[iy,ix].Value);
    }

    public float Noise(float x, float y)
    {

      // Determine grid cell coordinates
      int x0 = (int)x;
      int x1 = x0 + 1;
      int y0 = (int)y;
      int y1 = y0 + 1;

      // Determine interpolation weights
      // Could also use higher order polynomial/s-curve here
      //float sx = x - (float)x0;
      //float sy = y - (float)y0;
      float sx = 3 * (x - x0) * (x - x0) - 2 * (x - x0) * (x - x0) * (x - x0);
      float sy = 3 * (y - y0) * (y - y0) - 2 * (y - y0) * (y - y0) * (y - y0);

      // Interpolate between grid point gradients
      double n0, n1, ix0, ix1, value;
      n0 = DotProduct(x0, y0, x, y);
      n1 = DotProduct(x1, y0, x, y);
      ix0 = Lerp((float)n0, (float)n1, sx);
      n0 = DotProduct(x0, y1, x, y);
      n1 = DotProduct(x1, y1, x, y);
      ix1 = Lerp((float)n0, (float)n1, sx);
      value = Lerp((float)ix0, (float)ix1, sy);
      return (float)value;
    }
  }
}
