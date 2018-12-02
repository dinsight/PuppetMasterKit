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
    private static readonly Dictionary<SKTileGroup, Dictionary<TileType, List<SKTexture>>> cornersCache
        = new Dictionary<SKTileGroup, Dictionary<TileType, List<SKTexture>>>();

    private const string MASKS = "Masks";
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
    private const string CUL_DE_SAC_BOTTOM = "Cul De Sac Bottom";
    private const string CUL_DE_SAC_TOP = "Cul De Sac Top";
    private const string CUL_DE_SAC_LEFT = "Cul De Sac Left";
    private const string CUL_DE_SAC_RIGHT = "Cul De Sac Right";


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
      if (region.Type == Region.RegionType.REGION) {
        PaintRegion(region, layer);
      } else if (region.Type == Region.RegionType.PATH) {
        PaintPath(region, layer);
      } else {
        throw new ArgumentException($"Cannot paint region of type {region.Type}");
      }
    }

    /// <summary>
    /// Paints the region.
    /// </summary>
    /// <param name="region">Region.</param>
    /// <param name="layer">Layer.</param>
    private void PaintRegion(Region region, TileMapLayer layer)
    {
      var random = new Random(Guid.NewGuid().GetHashCode());
      var tileGroup = tileMapping
          .Where(k => k.Key == region.RegionFill)
          .Select(x => tileSet
                  .TileGroups
                  .FirstOrDefault(t => t.Name == x.Value))
          .FirstOrDefault();

      var masks = tileSet.TileGroups.First(x => x.Name == MASKS);
      var corners = GetCorners(tileGroup, masks);
      region.TraverseRegion((row, col, type) => {
        if (type == TileType.Plain) {
          layer.SetTile(tileGroup.GetRandomTexture(CENTER),
                                row,
                                col);
        } else if (type != TileType.Unknown) {
          var index = random.Next(0, corners[type].Count);
          layer.SetTile(corners[type][index], row, col, null);
        }
      });
    }

    /// <summary>
    /// Paints the path.
    /// </summary>
    /// <param name="path">Path.</param>
    /// <param name="layer">Layer.</param>
    private void PaintPath(Region path, TileMapLayer layer)
    {
      /*
      path.TraverseRegion((row, col, type) => {

        var tileGroup = tileMapping
      .Where(k => k.Key == MapCodes.PATH)
        //.Where(k => k.Key == path[row, col])
        .Select(x => tileSet
                .TileGroups
                .FirstOrDefault(t => t.Name == x.Value))
        .FirstOrDefault();

        //var corners = GetCorners(tileGroup);

        if (type == TileType.Plain) {
          layer.SetTile(tileGroup.GetRandomTexture(CENTER),
                                row,
                                col);
        } else {
          //layer.SetTile(corners[type], row, col, null);
        }
      });*/
    }

    /// <summary>
    /// Gets the corners.
    /// </summary>
    /// <returns>The corners.</returns>
    /// <param name="tileGroup">Tile group.</param>
    /// <param name="masks">Masks.</param>
    private static Dictionary<TileType, List<SKTexture>> GetCorners(SKTileGroup tileGroup, SKTileGroup masks)
    {
      if (cornersCache.ContainsKey(tileGroup)) {
        return cornersCache[tileGroup];
      }
      var dictionary = new Dictionary<TileType, List<SKTexture>>();
      var mainTile = tileGroup.GetRandomTexture(CENTER);

      dictionary.Add(TileType.BottomRightJoint, masks.GetTextures(LOWER_LEFT_CORNER).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.BottomLeftJoint, masks.GetTextures(LOWER_RIGHT_CORNER).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.BottomSide, masks.GetTextures(UP_EDGE).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.TopRightJoint, masks.GetTextures(UPPER_LEFT_CORNER).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.RightSide, masks.GetTextures(RIGHT_EDGE).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.TopLeftJoint, masks.GetTextures(UPPER_RIGHT_CORNER).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.TopSide, masks.GetTextures(DOWN_EDGE).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.LeftSide, masks.GetTextures(LEFT_EDGE).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.BottomLeftCorner, masks.GetTextures(UPPER_LEFT_EDGE).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.BottomRightCorner, masks.GetTextures(UPPER_RIGHT_EDGE).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.TopRightCorner, masks.GetTextures(LOWER_RIGHT_EDGE).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.TopLeftCorner, masks.GetTextures(LOWER_LEFT_EDGE).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.CulDeSacTop, masks.GetTextures(CUL_DE_SAC_TOP).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.CulDeSacBottom, masks.GetTextures(CUL_DE_SAC_BOTTOM).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.CulDeSacLeft, masks.GetTextures(CUL_DE_SAC_LEFT).Select(x => mainTile.BlendWithAlpha(x)).ToList());
      dictionary.Add(TileType.CulDeSacRight, masks.GetTextures(CUL_DE_SAC_RIGHT).Select(x => mainTile.BlendWithAlpha(x)).ToList());

      cornersCache.Add(tileGroup, dictionary);
      return dictionary;
    }
  }
}
