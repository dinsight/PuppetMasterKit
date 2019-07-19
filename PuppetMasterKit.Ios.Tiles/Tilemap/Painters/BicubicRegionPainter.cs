using System;
using System.Collections.Generic;
using System.Linq;
using LightInject;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Ios.Tiles.Tilemap.Helpers;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Utility;
using PuppetMasterKit.Utility.Configuration;
using PuppetMasterKit.Utility.Noise;
using SpriteKit;

namespace PuppetMasterKit.Ios.Tiles.Tilemap.Painters
{
  public class BicubicRegionPainter : IRegionPainter
  {
    readonly ICoordinateMapper mapper;
    private readonly int tileSize;
    private readonly int[] startRGBA;
    private readonly int[] endRGBA;
    private readonly int gradientDimension;
    private readonly Dictionary<TileType, SKTexture> marginTextures;

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Ios.Isometric.Tilemap.BicubicRegionPainter"/> class.
    /// </summary>
    public BicubicRegionPainter(int tileSize, int[] startRGBA, int[] endRGBA, int gradientDimension = 5)
    {
      marginTextures = PrepareMarginTextures(tileSize);
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
      if (region.Type != Region.RegionType.REGION) {
        throw new ArgumentException("BicubicRegionPainter: Unsupported region");
      }
      var start = DateTime.Now;
      //generate a gradient
      var gradient = GenerateGradient(gradientDimension);
      //trace the region's contour
      var contour = region.TraceContour(true);
      //add both tiles and contour to the list of tiles to point
      var tiles = region.Tiles;
      var all = new List<GridCoord>(tiles);
      all.AddRange(contour);
      //get the min/max values for the rows and cols
      var minRow = Math.Max(0, all.Min(x => x.Row));
      var maxRow = all.Max(x => x.Row);
      var minCol = Math.Max(0, all.Min(x => x.Col));
      var maxCol = all.Max(x => x.Col);
      //so we can calculate the scale size necessary to fit the gradient
      //into the tile surface
      var width = layer.GetMap().Cols * tileSize;
      var height = layer.GetMap().Rows * tileSize;
      var bicubic = new Bicubic(gradient);
      var scaleX = (float)gradientDimension / width;
      var scaleY = (float)gradientDimension / height;

      //pass the size and buffer to the painter
      var painter = new TilePainter(tileSize, tileSize, tileSize / 2);

      region.TraverseRegion((row, col, type) => {
        //Paint with noise
        painter.SetTileContext(row, col)
               .PaintNoise(bicubic, scaleX, scaleY, startRGBA, endRGBA);

        if (type == TileType.Plain) {
          //set the tile's texture
          layer.SetTile(painter.ToTexture(), row, col);
        } else {
          if (marginTextures.ContainsKey(type)) {
            layer.SetTile(painter.ToTexture()
                            .BlendWithAlpha(marginTextures[type]),
                            row, col);
          }
        }
      });

      var end = DateTime.Now;
      System.Diagnostics.Debug.WriteLine($"Bicubic painter:{(end - start).TotalMilliseconds} ms");
    }

    /// <summary>
    /// Prepares the margin textures.
    /// </summary>
    /// <returns>The margin textures.</returns>
    /// <param name="tileSize">Tile size.</param>
    private static Dictionary<TileType, SKTexture> PrepareMarginTextures(int tileSize)
    {
      var dictionary = new Dictionary<TileType, SKTexture>();
      var margin = new TilePainter(tileSize, tileSize, tileSize / 2);
      //fill with white color
      margin.PaintSolid(new int[] { 0xff, 0xff, 0xff, 0xff });
      dictionary.Add(TileType.BottomLeftCorner, margin.PaintBottomLeftCornerAlpha().ToTexture());
      dictionary.Add(TileType.BottomRightCorner, margin.PaintBottomRightCornerAlpha().ToTexture());
      dictionary.Add(TileType.TopLeftCorner, margin.PaintTopLeftCornerAlpha().ToTexture());
      dictionary.Add(TileType.TopRightCorner, margin.PaintTopRightCornerAlpha().ToTexture());
      dictionary.Add(TileType.TopSide, margin.PaintTopSideAlpha().ToTexture());
      dictionary.Add(TileType.RightSide, margin.PaintRightSideAlpha().ToTexture());
      dictionary.Add(TileType.BottomSide, margin.PaintBottomSideAlpha().ToTexture());
      dictionary.Add(TileType.LeftSide, margin.PaintLeftSideAlpha().ToTexture());
      dictionary.Add(TileType.TopLeftJoint, margin.PaintTopLeftJointAlpha().ToTexture());
      dictionary.Add(TileType.BottomRightJoint, margin.PaintBottomRightJointAlpha().ToTexture());
      dictionary.Add(TileType.TopRightJoint, margin.PaintTopRightJointAlpha().ToTexture());
      dictionary.Add(TileType.BottomLeftJoint, margin.PaintBottomLeftJointAlpha().ToTexture());
      return dictionary;
    }

    /// <summary>
    /// Generates the gradient.
    /// </summary>
    /// <returns>The gradient.</returns>
    /// <param name="dim">Dim.</param>
    private float[,] GenerateGradient(int dim)
    {
      var random = new Random(Guid.NewGuid().GetHashCode());
      var gradient = new float[dim, dim];
      for (int i = 0; i < dim; i++) {
        for (int j = 0; j < dim; j++) {
          gradient[i, j] = random.Next(-100, 100) / 256f;
        }
      }
      return gradient;
    }
  }
}