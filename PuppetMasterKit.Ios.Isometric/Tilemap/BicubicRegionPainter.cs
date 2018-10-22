using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using LightInject;
using PuppetMasterKit.AI;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Geometry;
using PuppetMasterKit.Graphics.Noise;
using PuppetMasterKit.Graphics.Sprites;
using SpriteKit;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public class BicubicRegionPainter : IRegionPainter
  {
    ICoordinateMapper mapper;
    private int tileSize;
    private int[] startRGBA;
    private int[] endRGBA;
    private int gradientDimension;
    Random random = new Random(Guid.NewGuid().GetHashCode());

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Ios.Isometric.Tilemap.BicubicRegionPainter"/> class.
    /// </summary>
    public BicubicRegionPainter(int tileSize, int[] startRGBA, int[] endRGBA, int gradientDimension = 40)
    {
      mapper = Container.GetContainer().GetInstance<ICoordinateMapper>();
      this.tileSize = tileSize;
      this.startRGBA = startRGBA;
      this.endRGBA = endRGBA;
      this.gradientDimension = gradientDimension;
    }

    /// <summary>
    /// Paint the specified region and layer.
    /// </summary>
    /// <param name="region">Region.</param>
    /// <param name="layer">Layer.</param>
    public void Paint(Region region, TileMapLayer layer)
    {
      var gradient = GenerateGradient(gradientDimension);
      var contour = region.TraceContour();
      var tiles = region.Tiles;
      var all = new List<GridCoord>(tiles);
      all.AddRange(contour);
      all.RemoveAll(x => x.Row < 0 || x.Col < 0);
      var minRow = all.Min(x => x.Row);
      var maxRow = all.Max(x => x.Row);
      var minCol = all.Min(x => x.Col);
      var maxCol = all.Max(x => x.Col);
      var width = (maxCol - minCol + 1) * tileSize;
      var height = (maxRow - minRow + 1) * tileSize;
      var bicubic = new Bicubic(gradient);
      var scaleX = (float)gradientDimension / width;
      var scaleY = (float)gradientDimension / height;

      foreach (var item in all) {
        var buffer = new byte[tileSize * (tileSize / 2) * ImageHelper.BytesPerPixel];
        var painter = new TilePainter(tileSize, tileSize, tileSize/2, buffer);

        //TO REVISE
        painter.SetTileContext(item.Row, item.Col)
               .PaintNoise(bicubic, scaleX, scaleY, startRGBA, endRGBA);

        var image = ImageHelper.GetImageFromBytes(tileSize, tileSize / 2, buffer);
        var texture = SKTexture.FromImage(image);
        layer.SetTile(texture, item.Row, item.Col);
      }
    }

    /// <summary>
    /// Generates the gradient.
    /// </summary>
    /// <returns>The gradient.</returns>
    /// <param name="dim">Dim.</param>
    private float[][] GenerateGradient(int dim)
    {
      var gradient = new float[dim][];
      for (int i = 0; i < dim; i++) {
        gradient[i] = new float[dim];
        for (int j = 0; j < dim; j++) {
          gradient[i][j] = random.Next(-100, 100) / 256f;
        }
      }
      return gradient;
    }
  }
}
