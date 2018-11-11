using System;
using System.Linq;
using System.Collections.Generic;
using PuppetMasterKit.AI;
using SpriteKit;
using PuppetMasterKit.Terrain.Map;

namespace PuppetMasterKit.Ios.Isometric.Tilemap
{
  public class TiledRegionPainter : IRegionPainter
  {
    private Dictionary<int, string> tileMapping;
    private SKTileSet tileSet;

    private const string CENTER = "Center";
    private const string UP_EDGE = "Up Edge";
    private const string UPPER_RIGHT_EDGE = "Upper Right Edge";
    private const string RIGHT_EDGE = "Right Edge";
    private const string LOWER_RIGHT_EDGE = "Lower Right Edge";
    private const string DOWN_EDGE = "Down Edge";
    private const string LOWER_LEFT_EDGE = "Lower Left Edge";
    private const string LEFT_EDGE = "Left Edge";
    private const string UPPER_LEFT_EDGE = "Upper Left Edge";
    private const string UPPER_RIGHT_CORNER = "Upper Right Corner";
    private const string LOWER_RIGHT_CORNER = "Lower Right Corner";
    private const string LOWER_LEFT_CORNER = "Lower Left Corner";
    private const string UPPER_LEFT_CORNER = "Upper Left Corner";

    /// <summary>
    /// Initializes a new instance of the <see cref="T:PuppetMasterKit.Ios.Isometric.Tilemap.TiledRegionPainter"/> class.
    /// </summary>
    /// <param name="tileMapping">Tile mapping.</param>
    /// <param name="tileSet">Tile set.</param>
    public TiledRegionPainter(Dictionary<int, string> tileMapping, SKTileSet tileSet)
    {
      this.tileMapping = tileMapping;
      this.tileSet = tileSet;
    }

    /// <summary>
    /// Paint the specified region and layer.
    /// </summary>
    /// <param name="region">Region.</param>
    /// <param name="layer">Layer.</param>
    public void Paint(Region region, TileMapLayer layer)
    {
      var tileGroup = tileMapping
          .Where(k => k.Key == region.RegionFill)
          .Select(x => tileSet
                  .TileGroups
                  .FirstOrDefault(t => t.Name == x.Value))
          .FirstOrDefault();

      var corners = GetCorners(tileGroup);
      region.TraverseRegion((row, col, type) => {
        if (type == TileType.Plain) {
          layer.SetTile(tileGroup.GetTexture(CENTER),
                                row,
                                col);
        } else {
          layer.SetTile(corners[type], row, col, null);
        }
      });
    }

    /// <summary>
    /// Gets the corners.
    /// </summary>
    /// <returns>The corners.</returns>
    /// <param name="tileGroup">Tile group.</param>
    private Dictionary<TileType, SKTexture> GetCorners(SKTileGroup tileGroup)
    {
      var dictionary = new Dictionary<TileType, SKTexture>();

      var mainTile = tileGroup.GetTexture(CENTER);

      dictionary.Add(TileType.BottomRightJoint, mainTile.BlendWithAlpha(tileGroup.GetTexture(LOWER_LEFT_CORNER)));
      dictionary.Add(TileType.BottomLeftJoint, mainTile.BlendWithAlpha(tileGroup.GetTexture(LOWER_RIGHT_CORNER)));
      dictionary.Add(TileType.BottomSide, mainTile.BlendWithAlpha(tileGroup.GetTexture(UP_EDGE)));
      dictionary.Add(TileType.TopRightJoint, mainTile.BlendWithAlpha(tileGroup.GetTexture(UPPER_LEFT_CORNER)));
      dictionary.Add(TileType.RightSide, mainTile.BlendWithAlpha(tileGroup.GetTexture(RIGHT_EDGE)));
      dictionary.Add(TileType.TopLeftJoint, mainTile.BlendWithAlpha(tileGroup.GetTexture(UPPER_RIGHT_CORNER)));
      dictionary.Add(TileType.TopSide, mainTile.BlendWithAlpha(tileGroup.GetTexture(DOWN_EDGE)));
      dictionary.Add(TileType.LeftSide, mainTile.BlendWithAlpha(tileGroup.GetTexture(LEFT_EDGE)));
      dictionary.Add(TileType.BottomLeftCorner, mainTile.BlendWithAlpha(tileGroup.GetTexture(UPPER_LEFT_EDGE)));
      dictionary.Add(TileType.BottomRightCorner, mainTile.BlendWithAlpha(tileGroup.GetTexture(UPPER_RIGHT_EDGE)));
      dictionary.Add(TileType.TopRightCorner, mainTile.BlendWithAlpha(tileGroup.GetTexture(LOWER_RIGHT_EDGE)));
      dictionary.Add(TileType.TopLeftCorner, mainTile.BlendWithAlpha(tileGroup.GetTexture(LOWER_LEFT_EDGE)));

      return dictionary;
    }
  }
}
