using System;
using System.Collections.Generic;
using System.Linq;
using LightInject;
using PuppetMasterKit.AI.Configuration;
using PuppetMasterKit.Graphics.Sprites;
using PuppetMasterKit.Terrain;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Terrain.Noise;
using SpriteKit;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
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
      var start = DateTime.Now;
      //generate a gradient
      var gradient = GenerateGradient(gradientDimension);
      //trace the region's contour
      var contour = region.TraceContour();
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

      foreach (var item in tiles) {
        //Paint with noise
        painter.SetTileContext(item.Row, item.Col)
               .PaintNoise(bicubic, scaleX, scaleY, startRGBA, endRGBA);
        //set the tile's texture
        layer.SetTile(painter.ToTexture(), item.Row, item.Col);
      }

      for (int index = 0; index < contour.Count; index++) {
        if (contour[index].Row < 0 || contour[index].Col < 0)
          continue;
        var c = contour[index];
        //get the prev and next tiles. Make sure to wrap around when the 
        //one of the ends of the list is reached
        var p = index == 0 ? contour[contour.Count - 1] : contour[index - 1];
        var n = index == contour.Count - 1 ? contour[0] : contour[index + 1];

        painter.SetTileContext(c.Row, c.Col)
               .PaintNoise(bicubic, scaleX, scaleY, startRGBA, endRGBA);

        if (p.Col==c.Col && c.Col==n.Col && p.Row < c.Row && c.Row < n.Row) { //l
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.BottomSide]),
                        c.Row, c.Col);
        }
        if (p.Col == c.Col && p.Row > c.Row && n.Col == c.Col && n.Row < c.Row) { //r
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.TopSide]),
                        c.Row, c.Col);
        }
        if (p.Row == c.Row && p.Col < c.Col && n.Row == c.Row && n.Col > c.Col) { //t
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.RightSide]),
                        c.Row, c.Col);
        }
        if (p.Row == c.Row && p.Col > c.Col && n.Row == c.Row && n.Col < c.Col) { //b
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.LeftSide]),
                        c.Row, c.Col);
        }
        if (p.Col == c.Col && p.Row < c.Row && n.Row == c.Row && n.Col > c.Col) { //tlc
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.BottomRightCorner]),
                        c.Row, c.Col);
        }
        if (p.Row == c.Row && p.Col < c.Col && n.Col == c.Col && n.Row < c.Row) { //trc
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.TopRightCorner]),
                        c.Row, c.Col);
        }
        if (p.Row == c.Row && p.Col > c.Col && n.Col == c.Col && n.Row > c.Row) { //blc *
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.BottomLeftCorner]),
                        c.Row, c.Col);
        }
        if (p.Col == c.Col && p.Row > c.Row && n.Row == c.Row && n.Col < c.Col) { //brc
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.TopLeftCorner]),
                        c.Row, c.Col);
        }
        if (p.Col == c.Col && p.Row < c.Row && n.Row == c.Row && n.Col < c.Col) { //blj
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.BottomLeftJoint]),
                        c.Row, c.Col);
        }
        if (p.Row==c.Row && p.Col > c.Col && n.Col == c.Col && n.Row < c.Row) { //brj
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.TopLeftJoint]),
                        c.Row, c.Col);
        }
        if (p.Col == c.Col && p.Row > c.Row && n.Row == c.Row && n.Col > c.Col) { //trj
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.TopRightJoint]),
                        c.Row, c.Col);
        }
        if (p.Row == c.Row && p.Col < c.Col && n.Col == c.Col && n.Row > c.Row) { //tlj
          layer.SetTile(painter.ToTexture()
                        .BlendWithAlpha(marginTextures[TileType.BottomRightJoint]),
                        c.Row, c.Col);
        }
      }

      var end = DateTime.Now;
      System.Diagnostics.Debug.WriteLine($"Bicubic painter:{(end-start).TotalMilliseconds} ms");
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
      dictionary.Add(TileType.BottomSide, margin.PaintBottomSideAlpha().ToTexture());
      dictionary.Add(TileType.LeftSide, margin.PaintLeftSideAlpha().ToTexture());
      dictionary.Add(TileType.RightSide, margin.PaintRightSideAlpha().ToTexture());
      dictionary.Add(TileType.TopLeftJoint, margin.PaintTopLeftJointAlpha().ToTexture());
      dictionary.Add(TileType.TopRightJoint, margin.PaintTopRightJointAlpha().ToTexture());
      dictionary.Add(TileType.BottomLeftJoint, margin.PaintBottomLeftJointAlpha().ToTexture());
      dictionary.Add(TileType.BottomRightJoint, margin.PaintBottomRightJointAlpha().ToTexture());
      return dictionary;
    }

    /// <summary>
    /// Generates the gradient.
    /// </summary>
    /// <returns>The gradient.</returns>
    /// <param name="dim">Dim.</param>
    private float[][] GenerateGradient(int dim)
    {
      var random = new Random(Guid.NewGuid().GetHashCode());
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
