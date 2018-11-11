using System;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Terrain.Noise;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public class LayeredRegionPainter : IRegionPainter
  {
    private const int GRAD = 5;
    private readonly int noiseAmplitude;

    public LayeredRegionPainter(int noiseAmplitude)
    {
      this.noiseAmplitude = noiseAmplitude;
    }

    /// <summary>
    /// Paint the specified region and layer.
    /// </summary>
    /// <param name="region">Region.</param>
    /// <param name="layer">Layer.</param>
    public void Paint(Region region, TileMapLayer layer)
    {
      var M = Math.Max( 
          region.MaxCol - region.MinCol,
          region.MaxRow - region.MinRow) ;
      var perlin = new Perlin(CreateGradient(GRAD+1));
      var contour = region.TraceContour();
      foreach (var item in region.Tiles) {
        var scol = (item.Col-region.MinCol) * (GRAD / M);
        var srow = (item.Row-region.MinRow) * (GRAD / M);
        var n = perlin.Noise(scol, srow) * noiseAmplitude;
        //SetTile(output, item.Row, item.Col, n);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>The gradient.</returns>
    private float[][] CreateGradient(int dim) {
      var random = new Random(Guid.NewGuid().GetHashCode());
      var gradient = new float[dim][];
      for (int i = 0; i < dim; i++) {
        gradient[i] = new float[dim];
        for (int j = 0; j < dim; j++) {
          gradient[i][j] = random.Next(0, 250) / 256f;
        }
      }
      return gradient;
    }
  }
}
