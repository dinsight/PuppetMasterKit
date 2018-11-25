using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Terrain.Noise;
using SpriteKit;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public class LayeredRegionPainter : IRegionPainter
  {
    private const float GRAD = 7f;
    private readonly int noiseAmplitude;
    private readonly SKTileSet tileSet;
    private readonly List<string> layers;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Ios.Isometric.Tilemap.LayeredRegionPainter"/> class.
    /// </summary>
    /// <param name="noiseAmplitude">Noise amplitude.</param>
    /// <param name="layers">Layers.</param>
    /// <param name="tileSet">Tile set.</param>
    public LayeredRegionPainter(int noiseAmplitude, 
          List<string> layers, 
          SKTileSet tileSet)
    {
      this.noiseAmplitude = noiseAmplitude;
      this.tileSet = tileSet;
      this.layers = layers;
    }

    /// <summary>
    /// Paint the specified region and layer.
    /// </summary>
    /// <param name="region">Region.</param>
    /// <param name="layer">Layer.</param>
    public void Paint(Region region, TileMapLayer layer)
    {
      //float M = Math.Max( 
          //region.MaxCol - region.MinCol,
          //region.MaxRow - region.MinRow) + 1;
      var generator = new Bicubic(CreateGradient((int)GRAD));

      foreach (var item in region.Tiles) {
        var maxCol = region.Tiles.Where(c => c.Row == item.Row).Max(x => x.Col);
        var minCol = region.Tiles.Where(c => c.Row == item.Row).Min(x => x.Col);
        var maxRow = region.Tiles.Where(c => c.Col == item.Col).Max(x => x.Row);
        var minRow = region.Tiles.Where(c => c.Col == item.Col).Min(x => x.Row);

        var M = (maxCol - minCol) + 1;
        var N = (maxRow - minRow) + 1;

        var scol = (item.Col - minCol) * ((GRAD - 1) / M);
        var srow = (item.Row - minRow) * ((GRAD - 1) / N);
        var n = generator.Noise(scol, srow) * noiseAmplitude;
        SetTileValue(region, n, item.Row, item.Col);
      }
      var contour = region.TraceContour();
      var contourRegion = new Region(0);
      contour.ForEach(c => contourRegion.AddTile(c.Row, c.Col));
      var regions = Region.ExtractRegions(region);
      var tileMapping = new Dictionary<int, string>();
      var tiledRegionPainter = new TiledRegionPainter(tileMapping, tileSet);
      regions.Insert(0,contourRegion);

      foreach (var item in regions) {
        tileMapping.TryAdd(item.RegionFill, layers[item.RegionFill]);
      }

      regions.OrderBy(r=>r.RegionFill).ToList()
             .ForEach(x => tiledRegionPainter.Paint(x, layer));
    }

    /// <summary>
    /// Sets the tile value.
    /// </summary>
    /// <param name="region">Region.</param>
    /// <param name="n">N.</param>
    /// <param name="row">Row.</param>
    /// <param name="col">Col.</param>
    private void SetTileValue(Region region, float n, int row, int col) {
      var count = layers.Count;
      var step = 1f / count;
      var index = 0;
      if (n >= 1) {
        index = count - 1;
      } else if (n < 0) {
        index = 0;
      } else 
      {
        index = (int)((n) / step);
      }
      region[row, col] = index;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>The gradient.</returns>
    private float[][] CreateGradient(int dim) {
      /*
      var random = new Random(Guid.NewGuid().GetHashCode());
      var gradient = new float[dim][];
      for (int i = 0; i < dim; i++) {
        gradient[i] = new float[dim];
        for (int j = 0; j < dim; j++) {
          gradient[i][j] = random.Next(0, 250) / 256f;
        }
      }*/
      var gradient = new float[dim][];
      gradient[0] = new float[] { 0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f };
      gradient[1] = new float[] { 0.3f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.3f };
      gradient[2] = new float[] { 0.3f, 0.5f, 0.8f, 0.8f, 0.8f, 0.5f, 0.3f };
      gradient[3] = new float[] { 0.3f, 0.5f, 0.8f, 0.8f, 0.8f, 0.5f, 0.3f };
      gradient[4] = new float[] { 0.3f, 0.5f, 0.8f, 0.8f, 0.8f, 0.5f, 0.3f };
      gradient[5] = new float[] { 0.3f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.3f };
      gradient[6] = new float[] { 0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f, 0.3f };

      return gradient;
    }
  }
}
