using System;
using System.Linq;
using System.Collections.Generic;
using SpriteKit;
using PuppetMasterKit.Terrain.Map;
using PuppetMasterKit.Ios.Tiles.Tilemap.Helpers;

namespace PuppetMasterKit.Ios.Tiles.Tilemap.Painters
{
  public class TiledRegionPainter : IRegionPainter
  {
    private Dictionary<int, string> tileMapping;
    private SKTileSet tileSet;
    private readonly Random randomTexture;
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
    /// <param name="randomSeed">Tile set.</param>
    public TiledRegionPainter(Dictionary<int, string> tileMapping, SKTileSet tileSet, int randomSeed)
    {
      this.tileMapping = tileMapping;
      this.tileSet = tileSet;
      this.randomTexture = new Random(randomSeed);
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
        //if (region.RegionFill == 1) {
          if (type == TileType.Plain || (region.RegionFill != 1 && region.RegionFill != 2) ||
            row == 0 || col == 0 || row == layer.GetMap().Rows - 1 || col == layer.GetMap().Cols - 1) {
            layer.SetTile(tileGroup.GetRandomTexture(CENTER, randomTexture),
                                  row,
                                  col);
          } else if (type != TileType.Unknown) {
            var index = random.Next(0, corners[type].Count);
            layer.SetTile(corners[type][index], row, col, null);
          }
        //}
      }, false);
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
    private Dictionary<TileType, List<SKTexture>> GetCorners(SKTileGroup tileGroup, SKTileGroup masks)
    {
      if (cornersCache.ContainsKey(tileGroup)) {
        return cornersCache[tileGroup];
      }
      var dictionary = new Dictionary<TileType, List<SKTexture>>();
      var mainTile = tileGroup.GetRandomTexture(CENTER, randomTexture);

      dictionary.Add(TileType.BottomRightJoint, GetCornerTextures(LOWER_LEFT_CORNER, tileGroup, masks));
      dictionary.Add(TileType.BottomLeftJoint, GetCornerTextures(LOWER_RIGHT_CORNER, tileGroup, masks));
      dictionary.Add(TileType.BottomSide, GetCornerTextures(UP_EDGE, tileGroup, masks));
      dictionary.Add(TileType.TopRightJoint, GetCornerTextures(UPPER_LEFT_CORNER, tileGroup, masks));
      dictionary.Add(TileType.RightSide, GetCornerTextures(RIGHT_EDGE, tileGroup, masks));
      dictionary.Add(TileType.TopLeftJoint, GetCornerTextures(UPPER_RIGHT_CORNER, tileGroup, masks));
      dictionary.Add(TileType.TopSide, GetCornerTextures(DOWN_EDGE, tileGroup, masks));
      dictionary.Add(TileType.LeftSide, GetCornerTextures(LEFT_EDGE, tileGroup, masks));
      dictionary.Add(TileType.BottomLeftCorner, GetCornerTextures(UPPER_LEFT_EDGE, tileGroup, masks));
      dictionary.Add(TileType.BottomRightCorner, GetCornerTextures(UPPER_RIGHT_EDGE, tileGroup, masks));
      dictionary.Add(TileType.TopRightCorner, GetCornerTextures(LOWER_RIGHT_EDGE, tileGroup, masks));
      dictionary.Add(TileType.TopLeftCorner, GetCornerTextures(LOWER_LEFT_EDGE, tileGroup, masks));
      dictionary.Add(TileType.CulDeSacTop, GetCornerTextures(CUL_DE_SAC_TOP, tileGroup, masks));
      dictionary.Add(TileType.CulDeSacBottom, GetCornerTextures(CUL_DE_SAC_BOTTOM, tileGroup, masks));
      dictionary.Add(TileType.CulDeSacLeft, GetCornerTextures(CUL_DE_SAC_LEFT, tileGroup, masks));
      dictionary.Add(TileType.CulDeSacRight, GetCornerTextures(CUL_DE_SAC_RIGHT, tileGroup, masks));

      cornersCache.Add(tileGroup, dictionary);
      return dictionary;
    }

    /// <summary>
    /// Gets the corner textures.
    /// </summary>
    /// <returns>The corner textures.</returns>
    /// <param name="ruleName">Rule name.</param>
    /// <param name="tileGroup">Tile group.</param>
    /// <param name="masks">Masks.</param>
    private List<SKTexture> GetCornerTextures(string ruleName, SKTileGroup tileGroup, SKTileGroup masks)
    {
      var tiles = tileGroup.GetTextures(ruleName);
      if (tiles.Count > 0) {
        return tiles;
      }
      var mainTile = tileGroup.GetRandomTexture(CENTER, randomTexture);
      return masks.GetTextures(ruleName).Select(x => mainTile.BlendWithAlpha(x)).ToList();
    }
  }
}